#!/usr/bin/env bash
set -euo pipefail

cleanup() {
  ./scripts/down.sh >/dev/null 2>&1 || true
}

trap cleanup EXIT

dotnet restore Aida.ParallelChange.sln
dotnet build Aida.ParallelChange.sln -c Release
dotnet test Aida.ParallelChange.sln -c Release
./scripts/up.sh
./scripts/smoke.sh
