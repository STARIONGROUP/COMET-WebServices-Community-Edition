#!/usr/bin/env bash
# Creates a test CA and a PostgreSQL server certificate in ./certs for docker-compose-db-tls.yml.
# For a test installation only: for production, use certificates from your organisation's CA.
#
# Usage (Linux, macOS or Git Bash on Windows):
#   ./docker-create-test-certificates.sh [host name, default comet-db] [days valid, default 365]
#
# Creates:
#   certs/ca.crt        the CA certificate: goes to the WebServices (Backtier:RootCertificate)
#   certs/ca.key        the CA private key: keep it out of the deployment, it is only needed to issue certificates
#   certs/server.crt    the server certificate, valid for the host name: goes to PostgreSQL
#   certs/server.key    the server private key: goes to PostgreSQL
#   certs/pg_hba.conf   only allows encrypted (hostssl) network connections: goes to PostgreSQL
set -euo pipefail

# stop Git Bash on Windows rewriting "/CN=..." into a Windows path (no effect elsewhere)
export MSYS_NO_PATHCONV=1

HOST_NAME="${1:-comet-db}"
DAYS="${2:-365}"
CERTS_DIR="$(cd "$(dirname "$0")" && pwd)/certs"

# Docker Desktop creates empty folders in place of missing bind-mount sources when the compose file is started
# before the certificates exist
if [[ -d "$CERTS_DIR/ca.crt" ]]; then
  echo "$CERTS_DIR/ca.crt is a folder, created by Docker because docker-compose-db-tls.yml was started before the"
  echo "certificates existed. Stop the containers, remove the folder $CERTS_DIR and run this script again"
  exit 1
fi

if [[ -e "$CERTS_DIR/server.key" ]]; then
  echo "$CERTS_DIR already contains certificates; remove the folder first to create new ones"
  exit 1
fi

mkdir -p "$CERTS_DIR"
cd "$CERTS_DIR"

openssl req -quiet -x509 -newkey rsa:2048 -nodes -days "$DAYS" -subj "/CN=COMET Database Test CA" -keyout ca.key -out ca.crt
openssl req -quiet -newkey rsa:2048 -nodes -subj "/CN=$HOST_NAME" -keyout server.key -out server.csr
printf "subjectAltName=DNS:%s\n" "$HOST_NAME" > san.cnf
openssl x509 -req -in server.csr -CA ca.crt -CAkey ca.key -CAcreateserial -days "$DAYS" -out server.crt -extfile san.cnf 2>&1
rm server.csr san.cnf ca.srl

# the local line is used by the initialisation scripts of the database image, over the socket inside the container
printf "local   all  all  trust\nhostssl all  all  all  scram-sha-256\n" > pg_hba.conf

# the WebServices image runs as a non-root user that must be able to read the CA certificate
chmod 644 ca.crt
chmod 600 ca.key server.key

echo "Created a test CA and a server certificate for '$HOST_NAME', valid for $DAYS days, in $CERTS_DIR:"
ls -l "$CERTS_DIR"
