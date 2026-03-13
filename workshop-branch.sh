#!/usr/bin/env bash
set -euo pipefail

branches=(
  "workshop/initial-state"
  "workshop/expand"
  "workshop/migrate"
  "workshop/contract"
)

repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$repo_root"

git rev-parse --is-inside-work-tree >/dev/null 2>&1 || {
  echo "This script must run inside the workshop repository." >&2
  exit 1
}

current_branch() {
  git branch --show-current
}

ensure_clean() {
  if [[ -n "$(git status --porcelain)" ]]; then
    echo "Working tree is not clean. Commit or stash your changes before switching branches." >&2
    exit 1
  fi
}

print_branches() {
  local current
  current="$(current_branch)"

  echo "Workshop branches:"
  for i in "${!branches[@]}"; do
    local marker=" "
    if [[ "${branches[$i]}" == "$current" ]]; then
      marker="*"
    fi
    printf "  %s %s. %s\n" "$marker" "$((i + 1))" "${branches[$i]}"
  done
}

find_index() {
  local branch="$1"
  for i in "${!branches[@]}"; do
    if [[ "${branches[$i]}" == "$branch" ]]; then
      echo "$i"
      return 0
    fi
  done
  echo "-1"
}

checkout_branch() {
  local target="$1"
  ensure_clean
  git checkout "$target"
  echo "Now on $target"
}

next_branch() {
  local current index next
  current="$(current_branch)"
  index="$(find_index "$current")"

  if [[ "$index" == "-1" ]]; then
    checkout_branch "${branches[0]}"
    return
  fi

  next=$((index + 1))
  if (( next >= ${#branches[@]} )); then
    echo "Already at the last workshop branch: ${branches[$index]}"
    return
  fi

  checkout_branch "${branches[$next]}"
}

prev_branch() {
  local current index prev
  current="$(current_branch)"
  index="$(find_index "$current")"

  if [[ "$index" == "-1" ]]; then
    checkout_branch "${branches[0]}"
    return
  fi

  prev=$((index - 1))
  if (( prev < 0 )); then
    echo "Already at the first workshop branch: ${branches[$index]}"
    return
  fi

  checkout_branch "${branches[$prev]}"
}

goto_branch() {
  local input="$1"
  if [[ "$input" == "initial-state" || "$input" == "initial" || "$input" == "1" ]]; then
    checkout_branch "${branches[0]}"
    return
  fi

  if [[ "$input" == "expand" || "$input" == "2" ]]; then
    checkout_branch "${branches[1]}"
    return
  fi

  if [[ "$input" == "migrate" || "$input" == "3" ]]; then
    checkout_branch "${branches[2]}"
    return
  fi

  if [[ "$input" == "contract" || "$input" == "4" ]]; then
    checkout_branch "${branches[3]}"
    return
  fi

  echo "Unknown target: $input" >&2
  echo "Use one of: initial-state, expand, migrate, contract" >&2
  exit 1
}

interactive_menu() {
  while true; do
    echo
    print_branches
    echo
    echo "Choose an action:"
    echo "  1) initial-state"
    echo "  2) expand"
    echo "  3) migrate"
    echo "  4) contract"
    echo "  n) next"
    echo "  p) previous"
    echo "  l) list"
    echo "  q) quit"
    read -r -p "> " choice

    if [[ "$choice" == "1" ]]; then
      goto_branch initial-state
      continue
    fi

    if [[ "$choice" == "2" ]]; then
      goto_branch expand
      continue
    fi

    if [[ "$choice" == "3" ]]; then
      goto_branch migrate
      continue
    fi

    if [[ "$choice" == "4" ]]; then
      goto_branch contract
      continue
    fi

    if [[ "$choice" == "n" || "$choice" == "N" ]]; then
      next_branch
      continue
    fi

    if [[ "$choice" == "p" || "$choice" == "P" ]]; then
      prev_branch
      continue
    fi

    if [[ "$choice" == "l" || "$choice" == "L" ]]; then
      print_branches
      continue
    fi

    if [[ "$choice" == "q" || "$choice" == "Q" ]]; then
      exit 0
    fi

    echo "Unknown option"
  done
}

usage() {
  cat <<USAGE
Usage:
  ./workshop-branch.sh               Open interactive mode
  ./workshop-branch.sh list          Show workshop branches
  ./workshop-branch.sh next          Checkout the next workshop branch
  ./workshop-branch.sh prev          Checkout the previous workshop branch
  ./workshop-branch.sh goto expand   Checkout a specific workshop branch
USAGE
}

if [[ $# -eq 0 ]]; then
  interactive_menu
fi

if [[ "$1" == "list" ]]; then
  print_branches
  exit 0
fi

if [[ "$1" == "next" ]]; then
  next_branch
  exit 0
fi

if [[ "$1" == "prev" ]]; then
  prev_branch
  exit 0
fi

if [[ "$1" == "goto" ]]; then
  [[ $# -eq 2 ]] || { usage; exit 1; }
  goto_branch "$2"
  exit 0
fi

if [[ "$1" == "help" || "$1" == "-h" || "$1" == "--help" ]]; then
  usage
  exit 0
fi

usage
exit 1
