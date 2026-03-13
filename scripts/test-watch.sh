#!/usr/bin/env bash
set -euo pipefail

source "$(dirname "$0")/common.sh"

file_hash() {
  git ls-files '*.cs' '*.csproj' '*.json' '*.http' '*.yml' '*.yaml' '*.sh' '*.ps1' | xargs -r sha256sum | sha256sum | cut -d' ' -f1
}

previous_hash=""

while true; do
  current_hash="$(file_hash)"

  if [[ "$current_hash" != "$previous_hash" ]]; then
    set +e
    dotnet test Aida.ParallelChange.sln -c Release
    status=$?
    set -e

    if (( status != 0 )); then
      ./scripts/play-alert.sh
    fi

    previous_hash="$current_hash"
  fi

  sleep "$AIDA_WATCH_INTERVAL_SECONDS"
done
