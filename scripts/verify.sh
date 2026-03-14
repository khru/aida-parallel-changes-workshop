#!/usr/bin/env bash
set -euo pipefail

source "$(dirname "$0")/common.sh"

ensure_repo_root

cleanup() {
  "$AIDA_REPO_ROOT/scripts/down.sh" >/dev/null 2>&1 || true
}

trap cleanup EXIT

ensure_docker_available

dotnet restore Aida.ParallelChange.sln
dotnet build Aida.ParallelChange.sln -c Release
"$AIDA_REPO_ROOT/scripts/check-shell-eol.sh"
"$AIDA_REPO_ROOT/scripts/test.sh"
dotnet test Aida.ParallelChange.sln -c Release --filter "TestCategory=NarrowIntegration"
"$AIDA_REPO_ROOT/scripts/coverage.sh"
"$AIDA_REPO_ROOT/scripts/mutation.sh"
"$AIDA_REPO_ROOT/scripts/up.sh"
"$AIDA_REPO_ROOT/scripts/smoke.sh"
