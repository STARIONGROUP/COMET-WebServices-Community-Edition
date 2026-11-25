#!/usr/bin/env bash
# Verifies a locally built COMET WebServices Docker image against a PostgreSQL server that only accepts TLS
# connections, mirroring the TLS checks of .github/workflows/CodeQuality.yml, before the image is pushed.
#
# Usage (Linux, macOS or Git Bash on Windows):
#   ./docker-verify-tls.sh <image> [path-to-ecss-10-25-annexc-integration-tests]
# Example:
#   ./docker-verify-tls.sh stariongroup/comet-webservices-community-edition:11.0.0-RC4 ../ecss-10-25-annexc-integration-tests
#
# With the integration test repository given, the database is seeded from its Data folder and the API is left
# running on http://localhost:5000, so that the integration test suite can be run against it afterwards.
set -euo pipefail

# stop Git Bash on Windows rewriting "/CN=..." and "/etc/..." into Windows paths, for docker and openssl only:
# curl is a Windows program too and needs the conversion for /dev/null and file paths (no effect elsewhere)
docker()  { MSYS_NO_PATHCONV=1 command docker "$@"; }
openssl() { MSYS_NO_PATHCONV=1 command openssl "$@"; }

step() { printf '\n\033[1;36m==> %s\033[0m\n' "$*"; }
ok()   { printf '\033[1;32mOK:\033[0m %s\n' "$*"; }
fail() { printf '\033[1;31mFAIL:\033[0m %s\n' "$*"; exit 1; }
trap 'printf "\033[1;31mFAIL:\033[0m line %s: %s\n" "$LINENO" "$BASH_COMMAND"' ERR

IMAGE="${1:?usage: $0 <image> [integration-tests-repo]}"
SUITE=""
if [ -n "${2:-}" ]; then
  # absolute, because the script changes directory below
  SUITE="$(cd "$2" && pwd)" || fail "integration test repository '$2' not found"
  [ -d "$SUITE/Data" ] || fail "'$2' has no Data folder; is it the ecss-10-25-annexc-integration-tests repository?"
fi
DB_IMAGE=stariongroup/cdp4-test-database-community-edition:4.2.0
API_URL=http://localhost:5000

step "Image under test: $IMAGE"
docker image inspect "$IMAGE" --format 'built {{.Created}}' || fail "image $IMAGE not found locally"

step "Creating throw-away CA, wrong CA and server certificate (CN/SAN comet-db)"
TLS="$(mktemp -d)"; cd "$TLS"; echo "in $TLS"
# Docker Desktop on Windows needs the Windows form of the path for a bind mount
SOURCE_DIR="$(pwd -W 2>/dev/null || pwd)"
openssl req -quiet -x509 -newkey rsa:2048 -nodes -days 1 -subj "/CN=COMET Local Test CA" -keyout ca.key -out ca.crt
openssl req -quiet -x509 -newkey rsa:2048 -nodes -days 1 -subj "/CN=Wrong CA" -keyout wrong-ca.key -out wrong-ca.crt
# the API reaches the database as "comet-db" on the docker network, so that is the name the certificate must carry
openssl req -quiet -newkey rsa:2048 -nodes -subj "/CN=comet-db" -keyout server.key -out server.csr
printf "subjectAltName=DNS:comet-db\n" > san.ext
openssl x509 -req -in server.csr -CA ca.crt -CAkey ca.key -CAcreateserial -days 1 -out server.crt -extfile san.ext 2>&1
printf "local all all trust\nhostssl all all all scram-sha-256\n" > pg_hba.conf

step "Removing containers and volume of a previous run"
docker rm -f comet-api comet-db 2>/dev/null || true
docker volume rm comet-db-tls 2>/dev/null || true
docker network create comet 2>/dev/null || echo "network comet already exists"
docker volume create comet-db-tls

step "Copying certificates into volume comet-db-tls (key owned by uid 70, mode 600)"
# PostgreSQL requires the key to be owned by its user (uid 70) with mode 600, which a bind mount from Windows
# or macOS cannot provide: copy the files into a named volume instead
docker run --rm -v comet-db-tls:/tls -v "$SOURCE_DIR:/src:ro" alpine sh -c '
  cp /src/ca.crt /src/wrong-ca.crt /src/server.crt /src/server.key /src/pg_hba.conf /tls/ &&
  chown 70:70 /tls/server.crt /tls/server.key /tls/pg_hba.conf && chmod 600 /tls/server.key &&
  chmod 644 /tls/ca.crt /tls/wrong-ca.crt && ls -ln /tls'

step "Starting PostgreSQL (TLS only, data on tmpfs in RAM)"
docker run -d --network comet --name comet-db --shm-size 2g \
  -e POSTGRES_PASSWORD=postgres \
  --tmpfs /var/lib/postgresql/data:rw \
  -v comet-db-tls:/etc/postgresql-tls:ro \
  "$DB_IMAGE" postgres \
    -c max_locks_per_transaction=1024 \
    -c ssl=on \
    -c ssl_cert_file=/etc/postgresql-tls/server.crt \
    -c ssl_key_file=/etc/postgresql-tls/server.key \
    -c ssl_min_protocol_version=TLSv1.2 \
    -c hba_file=/etc/postgresql-tls/pg_hba.conf
for i in $(seq 60); do
  docker exec comet-db pg_isready -U postgres >/dev/null 2>&1 && break
  [ "$i" -eq 60 ] && { docker logs comet-db; fail "PostgreSQL did not become ready"; }
  echo "[$i/60] waiting for PostgreSQL..."; sleep 3
done
ok "PostgreSQL is ready"

API_ENV=(-e Backtier__HostName=comet-db
         -e Backtier__IsDbSeedEnabled=true -e Backtier__IsDbRestoreEnabled=true
         -e Changelog__CollectChanges=true)

step "Starting API container (Backtier:SslMode VerifyFull)"
docker run -d --network comet --name comet-api -p 127.0.0.1:5000:5000 \
  -v comet-db-tls:/etc/comet-tls:ro "${API_ENV[@]}" \
  -e Backtier__SslMode=VerifyFull -e Backtier__RootCertificate=/etc/comet-tls/ca.crt \
  "$IMAGE"
for i in $(seq 60); do
  [ "$(curl -s -o /dev/null -w '%{http_code}' "$API_URL/healthz" || true)" = 200 ] && break
  [ "$i" -eq 60 ] && { docker logs comet-api; fail "API did not answer /healthz"; }
  echo "[$i/60] waiting for API /healthz..."; sleep 2
done
ok "API answers $API_URL/healthz"

if [ -n "$SUITE" ]; then
  step "Seeding the database with $SUITE/Data"
  ( cd "$SUITE/Data" && rm -f Data.zip &&
    if command -v zip >/dev/null; then zip -qr Data.zip . ; else /c/Windows/System32/tar.exe -a -c -f Data.zip --exclude Data.zip * ; fi &&
    curl -sS --fail --form file=@Data.zip "$API_URL/Data/Exchange" ) || { docker logs comet-api 2>&1 | tail -30; fail "seeding failed"; }
  echo
  for i in $(seq 60); do
    docker logs comet-api 2>&1 | grep -q "ready to accept requests" && break
    [ "$i" -eq 60 ] && { docker logs comet-api 2>&1 | tail -30; fail "API did not report ready after seeding"; }
    echo "[$i/60] waiting for API to accept requests..."; sleep 2
  done
  ok "API is ready to accept requests"
else
  echo "no integration-test repo given: database not seeded, so the TLS checks below will fail"
fi

step "Checking the API log for a verified TLS connection"
docker logs comet-api > "$TLS/api.log" 2>&1
grep -E "ready to accept requests|encrypted using TLS|NOT encrypted|not fully verified" "$TLS/api.log" | sort -u || true
grep -q "is encrypted using TLS" "$TLS/api.log" || fail "no TLS confirmation in the log"
if grep -qE "NOT encrypted|not fully verified" "$TLS/api.log"; then fail "connection not encrypted or not verified"; fi
ok "API connects to PostgreSQL over verified TLS"

# negative checks: each must stop the container with exit code 1 and the expected message
check() {
  expected="$1"; shift
  step "Negative check: expecting exit code 1 and '$expected'"
  set +e
  out=$(MSYS_NO_PATHCONV=1 timeout 180 docker run --rm --name comet-api-check --network comet -v comet-db-tls:/etc/comet-tls:ro "${API_ENV[@]}" \
          -e Kestrel__Endpoints__Http__Url=http://0.0.0.0:5099 "$@" "$IMAGE" 2>&1)
  code=$?
  set -e
  docker rm -f comet-api-check >/dev/null 2>&1 || true
  if [ $code -eq 1 ] && grep -q "$expected" <<<"$out"; then
    grep -m1 "$expected" <<<"$out"
    ok "$expected"
  else
    echo "$out" | tail -20
    fail "expected exit code 1 and '$expected', got exit code $code"
  fi
}
check "is not trusted by Backtier:RootCertificate" -e Backtier__SslMode=VerifyFull -e Backtier__RootCertificate=/etc/comet-tls/wrong-ca.crt
check "only accepts encrypted connections"           -e Backtier__SslMode=Disable

trap - ERR
step "All TLS checks passed"
echo "comet-api is still running on $API_URL for the integration tests."
echo "Follow its log with:  docker logs -f comet-api"
echo "Clean up with:        docker rm -f comet-api comet-db && docker volume rm comet-db-tls"
