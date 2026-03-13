. "$PSScriptRoot/common.ps1"
Import-AidaEnv

docker compose up -d sqlserver
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Wait-ForSqlServer
Ensure-DatabaseExists

docker compose build migrator
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

docker compose run --rm migrator
