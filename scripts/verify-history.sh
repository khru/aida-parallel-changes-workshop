#!/usr/bin/env bash
set -euo pipefail

branches=(
  "main"
  "workshop/initial-state"
  "workshop/expand"
  "workshop/migrate"
  "workshop/contract"
)

full="false"

if [[ "${1:-}" == "--full" ]]; then
  full="true"
fi

if [[ -n "$(git status --porcelain)" ]]; then
  echo "Working tree must be clean before verify-history" >&2
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
    git checkout --detach "$commit" >/dev/null
    echo "Checking $branch $commit"

    dotnet restore Aida.ParallelChange.sln >/dev/null
    dotnet build Aida.ParallelChange.sln -c Release >/dev/null
    dotnet test Aida.ParallelChange.sln -c Release >/dev/null

    if [[ "$full" == "true" ]] && [[ -f scripts/up.sh ]] && [[ -f scripts/smoke.sh ]] && [[ -f scripts/down.sh ]]; then
      ./scripts/down.sh >/dev/null 2>&1 || true
      ./scripts/up.sh >/dev/null
      ./scripts/smoke.sh >/dev/null
      ./scripts/down.sh >/dev/null
    fi
  done <<< "$range"

  previous_branch="$branch"
done
