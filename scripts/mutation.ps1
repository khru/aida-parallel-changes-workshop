$ErrorActionPreference = 'Stop'

. "$PSScriptRoot/common.ps1"
Import-AidaEnv

$toolPath = '.tools/dotnet-stryker'

Invoke-InAidaRepoRoot {
    if (-not (Test-Path $toolPath)) {
        dotnet tool install dotnet-stryker --tool-path ./.tools --version 4.13.0
        if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
    }

    $env:VSTEST_CONNECTION_TIMEOUT = '300'

    & $toolPath `
      --config-file stryker-config.json `
      --output artifacts/stryker `
      --log-to-file
}
