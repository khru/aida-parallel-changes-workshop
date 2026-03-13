#!/usr/bin/env bash
set -euo pipefail

if [[ -f .env ]]; then
  set -a
  source .env
  set +a
fi

: "${AIDA_SQL_DATABASE:=AidaParallelChange}"
: "${AIDA_SQL_USER:=sa}"
: "${AIDA_SQL_PASSWORD:=Your_strong_password_123}"
: "${AIDA_SQL_READY_ATTEMPTS:=60}"
: "${AIDA_SQL_READY_SLEEP_SECONDS:=2}"
: "${AIDA_HTTP_ENV_FILE:=http/environments/local.http-client.env.json}"
: "${AIDA_HTTP_ENV:=docker}"
: "${AIDA_WATCH_INTERVAL_SECONDS:=2}"
: "${AIDA_ALERT_SOUND:=/mnt/c/Windows/Media/Windows Critical Stop.wav}"

wait_for_sqlserver() {
  local attempt=1
  while (( attempt <= AIDA_SQL_READY_ATTEMPTS )); do
    if docker compose exec -T sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U "$AIDA_SQL_USER" -P "$AIDA_SQL_PASSWORD" -Q "SELECT 1" -C >/dev/null 2>&1; then
      return 0
    fi
    sleep "$AIDA_SQL_READY_SLEEP_SECONDS"
    attempt=$((attempt + 1))
  done

  echo "SQL Server did not become ready in time" >&2
  return 1
}

ensure_database_exists() {
  docker compose exec -T sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U "$AIDA_SQL_USER" -P "$AIDA_SQL_PASSWORD" -Q "IF DB_ID('$AIDA_SQL_DATABASE') IS NULL CREATE DATABASE [$AIDA_SQL_DATABASE]" -C >/dev/null
}
