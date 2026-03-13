#!/usr/bin/env bash
set -euo pipefail

docker compose down --remove-orphans --volumes
docker network prune -f
docker volume prune -f
