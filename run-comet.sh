#!/usr/bin/env bash
set -euo pipefail

# Full rebuild of the whole solution
dotnet clean COMET-WebServices.sln -c Release
dotnet restore COMET-WebServices.sln
dotnet build COMET-WebServices.sln --no-restore -c Release

# Runtime config
export ASPNETCORE_ENVIRONMENT=Production
export ASPNETCORE_URLS=http://localhost:5000
export Logging__LogLevel__Default=Warning
export Logging__LogLevel__Microsoft=Warning

# Backtier: connection to the dockerized test PostgreSQL
export Backtier__HostName=localhost
export Backtier__Port=5432

# Allow the integration-test seed/restore endpoints
export Backtier__IsDbSeedEnabled=true
export Backtier__IsDbRestoreEnabled=true

export Changelog__CollectChanges=true

dotnet run -c Release \
  --project CometServer/CometServer.csproj \
  --no-launch-profile