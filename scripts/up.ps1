. "$PSScriptRoot/common.ps1"
Import-AidaEnv

Invoke-InAidaRepoRoot {
    Assert-DockerAvailable
    Recover-ComposeStackCollisions

    Invoke-Compose build ijhttp
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

    Invoke-Compose build migrator
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

    Invoke-Compose up -d --build sqlserver
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

    Wait-ForSqlServer
    Recreate-Database

    Invoke-Compose run --rm migrator
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

    Invoke-Compose up -d --build api
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
}
