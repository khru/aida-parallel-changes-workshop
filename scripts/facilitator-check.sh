#!/usr/bin/env bash
set -euo pipefail

git -c core.filemode=false status --short --branch
git show-ref --heads --abbrev | grep -E 'refs/heads/(main|workshop/initial-state|workshop/expand|workshop/migrate|workshop/contract)$'
git merge-base --is-ancestor main workshop/initial-state
git merge-base --is-ancestor workshop/initial-state workshop/expand
git merge-base --is-ancestor workshop/expand workshop/migrate
git merge-base --is-ancestor workshop/migrate workshop/contract
echo ancestry-ok
git rev-list --merges --count main..workshop/initial-state
git rev-list --merges --count workshop/initial-state..workshop/expand
git rev-list --merges --count workshop/expand..workshop/migrate
git rev-list --merges --count workshop/migrate..workshop/contract
