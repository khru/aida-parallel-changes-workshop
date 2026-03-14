#!/usr/bin/env bash
set -euo pipefail

source "$(dirname "$0")/common.sh"

ensure_repo_root

dotnet test Aida.ParallelChange.sln -c Release --filter "TestCategory!=NarrowIntegration"
