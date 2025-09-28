#!/bin/bash

# Exit on error
set -e

# Ensure version is passed
if [ -z "$1" ]; then
  echo "Usage: $0 <version>"
  echo "Example: $0 x.y.z"
  exit 1
fi

VERSION="$1"

ECHO "Pull latest version of mcr.microsoft.com/dotnet/sdk:9.0"

docker pull mcr.microsoft.com/dotnet/sdk:9.0

echo "Building local COMET-Webservices Docker image for version: $VERSION"

docker build \
  -f Dockerfile \
  -t stariongroup/comet-webservices-community-edition:latest \
  -t stariongroup/comet-webservices-community-edition:$VERSION \
  .

echo "COMET-Webservices Docker Build complete."
echo "Tags: latest, $VERSION"