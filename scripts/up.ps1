. "$PSScriptRoot/common.ps1"
Import-AidaEnv

docker compose build ijhttp migrator
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

docker compose up -d --build sqlserver
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Wait-ForSqlServer
Ensure-DatabaseExists

docker compose run --rm migrator
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

docker compose up -d --build api
