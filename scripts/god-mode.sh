#!/usr/bin/env bash
set -euo pipefail

source "$(dirname "$0")/common.sh"

dry_run="false"

if [[ "${1:-}" == "--dry-run" ]]; then
  dry_run="true"
fi

if [[ -n "$(git status --porcelain)" ]]; then
  echo "Working tree must be clean before running god-mode" >&2
  exit 1
fi

start_ref="$(git rev-parse HEAD)"

file_hash() {
  git ls-files '*.cs' '*.csproj' '*.json' '*.http' '*.yml' '*.yaml' '*.sh' '*.ps1' | xargs -r sha256sum | sha256sum | cut -d' ' -f1
}

run_tests() {
  set +e
  dotnet test Aida.ParallelChange.sln -c Release
  local status=$?
  set -e
  return "$status"
}

if ! run_tests; then
  ./scripts/play-alert.sh
  exit 1
fi

last_green="true"
previous_hash="$(file_hash)"

while true; do
  current_hash="$(file_hash)"
  if [[ "$current_hash" != "$previous_hash" ]]; then
    if run_tests; then
      last_green="true"
    else
      if [[ "$last_green" == "true" ]]; then
        ./scripts/play-alert.sh
        if [[ "$dry_run" == "false" ]]; then
          git reset --hard "$start_ref"
          git clean -fd
        fi
        exit 1
      fi
      last_green="false"
    fi

    previous_hash="$current_hash"
  fi

  sleep "$AIDA_WATCH_INTERVAL_SECONDS"
done
