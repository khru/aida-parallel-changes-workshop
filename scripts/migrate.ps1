. "$PSScriptRoot/common.ps1"
Import-AidaEnv

Invoke-InAidaRepoRoot {
    Assert-DockerAvailable
    Recover-ComposeStackCollisions

    Invoke-Compose up -d sqlserver
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

    Wait-ForSqlServer
    Recreate-Database

    Invoke-Compose build migrator
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

    Invoke-Compose run --rm migrator
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
}
