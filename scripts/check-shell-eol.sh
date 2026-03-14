#!/usr/bin/env bash
set -euo pipefail

source "$(dirname "$0")/common.sh"

ensure_repo_root

mapfile -t shell_files < <(git ls-files '*.sh')

if (( ${#shell_files[@]} == 0 )); then
  exit 0
fi

files_with_crlf=()
for shell_file in "${shell_files[@]}"; do
  if [[ ! -f "$shell_file" ]]; then
    continue
  fi

  if LC_ALL=C grep -q $'\r' "$shell_file"; then
    files_with_crlf+=("$shell_file")
  fi
done

if (( ${#files_with_crlf[@]} > 0 )); then
  printf 'CRLF detected in shell scripts:\n' >&2
  printf ' - %s\n' "${files_with_crlf[@]}" >&2
  exit 1
fi
