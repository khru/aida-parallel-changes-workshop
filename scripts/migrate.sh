#!/usr/bin/env bash
set -euo pipefail

source "$(dirname "$0")/common.sh"

docker compose up -d sqlserver
wait_for_sqlserver
ensure_database_exists
docker compose build migrator
docker compose run --rm migrator
