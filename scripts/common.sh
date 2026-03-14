#!/usr/bin/env bash
set -euo pipefail

AIDA_REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

if [[ -f "$AIDA_REPO_ROOT/.env" ]]; then
  set -a
  source "$AIDA_REPO_ROOT/.env"
  set +a
fi

: "${AIDA_SQL_DATABASE:=AidaParallelChange}"
: "${AIDA_SQL_USER:=sa}"
: "${AIDA_SQL_PASSWORD:=Your_strong_password_123}"
: "${AIDA_SQL_PORT:=14333}"
: "${AIDA_API_PORT:=8080}"
: "${AIDA_SQL_READY_ATTEMPTS:=60}"
: "${AIDA_SQL_READY_SLEEP_SECONDS:=2}"
: "${AIDA_HTTP_ENV_FILE:=http/environments/local.http-client.env.json}"
: "${AIDA_HTTP_ENV:=docker}"
: "${AIDA_COMPOSE_PROJECT_NAME:=aida-parallel-change}"

compose_cmd() {
  docker compose --project-name "$AIDA_COMPOSE_PROJECT_NAME" "$@"
}

ensure_repo_root() {
  cd "$AIDA_REPO_ROOT"
}

ensure_docker_available() {
  set +e
  docker info >/dev/null 2>&1
  local docker_status=$?
  set -e

  if [[ $docker_status -eq 0 ]]; then
    return 0
  fi

  echo "Docker is required and is not available." >&2
  return 1
}

container_exists_by_name() {
  local target_name="$1"

  while IFS= read -r container_name; do
    if [[ "$container_name" == "$target_name" ]]; then
      return 0
    fi
  done < <(docker ps -a --format '{{.Names}}')

  return 1
}

remove_legacy_containers() {
  local legacy_containers=(
    "aida-parallel-change-sqlserver"
    "aida-parallel-change-api"
    "aida-parallel-change-migrator"
    "aida-parallel-change-ijhttp"
  )

  local legacy_container
  for legacy_container in "${legacy_containers[@]}"; do
    if container_exists_by_name "$legacy_container"; then
      docker rm -f "$legacy_container" >/dev/null 2>&1 || true
    fi
  done
}

remove_port_collisions() {
  local compose_name_prefix="${AIDA_COMPOSE_PROJECT_NAME}-"
  declare -A colliding_container_ids=()

  while IFS='|' read -r container_id container_name published_ports; do
    if [[ -z "$container_id" ]]; then
      continue
    fi

    if [[ "$container_name" == "$compose_name_prefix"* ]]; then
      continue
    fi

    if [[ "$published_ports" == *":${AIDA_SQL_PORT}->"* ]] || [[ "$published_ports" == *":${AIDA_API_PORT}->"* ]]; then
      colliding_container_ids["$container_id"]=1
    fi
  done < <(docker ps --format '{{.ID}}|{{.Names}}|{{.Ports}}')

  if (( ${#colliding_container_ids[@]} > 0 )); then
    docker rm -f "${!colliding_container_ids[@]}" >/dev/null
  fi
}

recover_compose_stack_collisions() {
  compose_cmd down --remove-orphans >/dev/null 2>&1 || true
  remove_legacy_containers
  remove_port_collisions
}

wait_for_sqlserver() {
  local attempt=1
  while (( attempt <= AIDA_SQL_READY_ATTEMPTS )); do
    if compose_cmd exec -T sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U "$AIDA_SQL_USER" -P "$AIDA_SQL_PASSWORD" -Q "SELECT 1" -C >/dev/null 2>&1; then
      return 0
    fi
    sleep "$AIDA_SQL_READY_SLEEP_SECONDS"
    attempt=$((attempt + 1))
  done

  echo "SQL Server did not become ready in time" >&2
  return 1
}

recreate_database() {
  compose_cmd exec -T sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -d master -U "$AIDA_SQL_USER" -P "$AIDA_SQL_PASSWORD" -Q "IF DB_ID('$AIDA_SQL_DATABASE') IS NOT NULL BEGIN ALTER DATABASE [$AIDA_SQL_DATABASE] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [$AIDA_SQL_DATABASE]; END" -b -C >/dev/null
  compose_cmd exec -T sqlserver /bin/sh -lc "rm -f /var/opt/mssql/data/$AIDA_SQL_DATABASE.mdf /var/opt/mssql/data/${AIDA_SQL_DATABASE}_log.ldf" >/dev/null 2>&1 || true
  compose_cmd exec -T sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -d master -U "$AIDA_SQL_USER" -P "$AIDA_SQL_PASSWORD" -Q "IF DB_ID('$AIDA_SQL_DATABASE') IS NULL CREATE DATABASE [$AIDA_SQL_DATABASE]" -b -C >/dev/null
  wait_for_database_online
}

wait_for_database_online() {
  local attempt=1
  while (( attempt <= AIDA_SQL_READY_ATTEMPTS )); do
    local result
    result=$(compose_cmd exec -T sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U "$AIDA_SQL_USER" -P "$AIDA_SQL_PASSWORD" -Q "SET NOCOUNT ON; IF EXISTS (SELECT 1 FROM sys.databases WHERE name = '$AIDA_SQL_DATABASE' AND state_desc = 'ONLINE') SELECT 1 ELSE SELECT 0" -h -1 -W -C 2>/dev/null | tr -d '[:space:]')
    if [[ "$result" == "1" ]]; then
      return 0
    fi

    sleep "$AIDA_SQL_READY_SLEEP_SECONDS"
    attempt=$((attempt + 1))
  done

  echo "Database '$AIDA_SQL_DATABASE' did not become online in time" >&2
  return 1
}
