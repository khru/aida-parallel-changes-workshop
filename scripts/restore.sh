#!/usr/bin/env bash
set -euo pipefail

source "$(dirname "$0")/common.sh"

ensure_repo_root

mapfile -t project_files < <(git ls-files '*.csproj')

if (( ${#project_files[@]} == 0 )); then
  echo "No project files were found to restore." >&2
  exit 1
fi

for project_file in "${project_files[@]}"; do
  dotnet restore "$project_file"
done
