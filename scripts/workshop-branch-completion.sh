#!/usr/bin/env bash

_workshop_branch_completion() {
  local current previous
  current="${COMP_WORDS[COMP_CWORD]}"
  previous="${COMP_WORDS[COMP_CWORD-1]}"

  local commands="list branches next prev goto help"
  local phases="initial-state initial expand migrate contract 1 2 3 4"

  if [[ "$previous" == "goto" ]]; then
    COMPREPLY=( $(compgen -W "$phases" -- "$current") )
    return
  fi

  COMPREPLY=( $(compgen -W "$commands" -- "$current") )
}

complete -F _workshop_branch_completion ./workshop-branch.sh
