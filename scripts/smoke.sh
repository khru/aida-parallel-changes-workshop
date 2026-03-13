#!/usr/bin/env bash
set -euo pipefail

source "$(dirname "$0")/common.sh"

requests=(
  "http/v1/get-customer-contact.http"
  "http/v1/update-customer-contact.http"
  "http/v2/get-customer-contact.http"
  "http/v2/update-customer-contact.http"
)

for request in "${requests[@]}"; do
  if [[ -f "$request" ]]; then
    docker compose run --rm ijhttp --env-file "$AIDA_HTTP_ENV_FILE" --env "$AIDA_HTTP_ENV" "$request"
  fi
done
