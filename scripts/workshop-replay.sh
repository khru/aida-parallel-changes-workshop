#!/usr/bin/env bash
set -euo pipefail

branches=(
  "main"
  "workshop/initial-state"
  "workshop/expand"
  "workshop/migrate"
  "workshop/contract"
)

dry_run="false"
auto="false"
delay_seconds="2"

while [[ $# -gt 0 ]]; do
  case "$1" in
    --dry-run)
      dry_run="true"
      shift
      ;;
    --auto)
      auto="true"
      shift
      ;;
    --delay)
      delay_seconds="$2"
      shift 2
      ;;
    *)
      echo "Unknown option: $1" >&2
      exit 1
      ;;
  esac
done

if [[ -n "$(git status --porcelain)" ]]; then
  echo "Working tree must be clean before replay" >&2
  exit 1
fi

initial_ref="$(git rev-parse --abbrev-ref HEAD)"
if [[ "$initial_ref" == "HEAD" ]]; then
  initial_ref="$(git rev-parse HEAD)"
fi

restore_ref() {
  git checkout "$initial_ref" >/dev/null 2>&1 || true
}

trap restore_ref EXIT

previous_branch=""

for branch in "${branches[@]}"; do
  if ! git show-ref --verify --quiet "refs/heads/$branch"; then
    continue
  fi

  if [[ -z "$previous_branch" ]]; then
    range="$(git rev-list --reverse "$branch")"
  else
    base="$(git merge-base "$previous_branch" "$branch")"
    range="$(git rev-list --reverse "${base}..${branch}")"
  fi

  while IFS= read -r commit; do
    [[ -z "$commit" ]] && continue
    message="$(git show -s --format='%s' "$commit")"
    printf '%s %s %s\n' "$branch" "$commit" "$message"

    if [[ "$dry_run" == "false" ]]; then
      git checkout --detach "$commit" >/dev/null
      if [[ "$auto" == "true" ]]; then
        sleep "$delay_seconds"
      else
        read -r -p "Press enter to continue: " _
      fi
    fi
  done <<< "$range"

  previous_branch="$branch"
done
