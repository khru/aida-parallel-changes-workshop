#!/usr/bin/env bash
set -euo pipefail

source "$(dirname "$0")/common.sh"

ensure_repo_root
ensure_docker_available

export TESTCONTAINERS_RYUK_DISABLED="${TESTCONTAINERS_RYUK_DISABLED:-true}"

if [[ "${ACT:-false}" == "true" ]] && [[ -z "${TESTCONTAINERS_HOST_OVERRIDE:-}" ]]; then
  export TESTCONTAINERS_HOST_OVERRIDE="host.docker.internal"
fi

dotnet test Aida.ParallelChange.sln -c Release --filter "TestCategory=NarrowIntegration"
