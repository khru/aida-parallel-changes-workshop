#!/usr/bin/env bash
set -euo pipefail

source "$(dirname "$0")/common.sh"

ensure_repo_root

ensure_plan_is_local_only() {
  if git ls-files --error-unmatch plan.md >/dev/null 2>&1; then
    echo "plan.md is local-only and must not be tracked by git." >&2
    return 1
  fi
}

ensure_generated_artifacts_are_not_tracked() {
  local tracked_file
  local tracked_generated=()

  while IFS= read -r tracked_file; do
    case "$tracked_file" in
      artifacts/*|TestResults/*|StrykerOutput/*|.stryker-tmp/*|*/bin/*|*/obj/*|coverage.json|coverage*.json|*/coverage.json|*/coverage*.json)
        tracked_generated+=("$tracked_file")
        ;;
    esac
  done < <(git ls-files)

  if (( ${#tracked_generated[@]} > 0 )); then
    echo "Generated artifacts must not be tracked by git:" >&2
    printf ' - %s\n' "${tracked_generated[@]}" >&2
    return 1
  fi
}

ensure_plan_is_local_only
ensure_generated_artifacts_are_not_tracked

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
