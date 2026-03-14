#!/usr/bin/env bash
set -euo pipefail

source "$(dirname "$0")/common.sh"

ensure_repo_root

tool_path=".tools/dotnet-stryker"

if [[ ! -x "$tool_path" ]]; then
  dotnet tool install dotnet-stryker --tool-path ./.tools --version 4.13.0
fi

export VSTEST_CONNECTION_TIMEOUT="300"

"$tool_path" \
  --config-file stryker-config.json \
  --output artifacts/stryker \
  --log-to-file
