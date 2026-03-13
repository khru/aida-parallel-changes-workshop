#!/usr/bin/env bash
set -euo pipefail

source "$(dirname "$0")/common.sh"

if [[ -f "$AIDA_ALERT_SOUND" ]]; then
  if command -v aplay >/dev/null 2>&1; then
    aplay "$AIDA_ALERT_SOUND" >/dev/null 2>&1 || true
    exit 0
  fi

  if command -v paplay >/dev/null 2>&1; then
    paplay "$AIDA_ALERT_SOUND" >/dev/null 2>&1 || true
    exit 0
  fi
fi

if command -v powershell.exe >/dev/null 2>&1; then
  powershell.exe -NoProfile -Command "try { [console]::beep(700,700) } catch {}" >/dev/null 2>&1 || true
  exit 0
fi

printf '\a'
