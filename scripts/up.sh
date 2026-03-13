#!/usr/bin/env bash
set -euo pipefail

source "$(dirname "$0")/common.sh"

docker compose build ijhttp migrator
docker compose up -d --build sqlserver
wait_for_sqlserver
ensure_database_exists
docker compose run --rm migrator
docker compose up -d --build api
