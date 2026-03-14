#!/usr/bin/env bash
set -euo pipefail

source "$(dirname "$0")/common.sh"

ensure_repo_root
ensure_docker_available
recover_compose_stack_collisions

compose_cmd build ijhttp
compose_cmd build migrator
compose_cmd up -d --build sqlserver
wait_for_sqlserver
recreate_database
compose_cmd run --rm migrator
compose_cmd up -d --build api
