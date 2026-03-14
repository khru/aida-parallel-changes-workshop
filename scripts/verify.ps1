$ErrorActionPreference = 'Stop'

. "$PSScriptRoot/common.ps1"
Import-AidaEnv

function Invoke-Cleanup {
    try {
        & (Join-Path $PSScriptRoot 'down.ps1') *> $null
    }
    catch {
    }
}

try {
    Invoke-InAidaRepoRoot {
        Assert-DockerAvailable

        dotnet restore Aida.ParallelChange.sln
        if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

        dotnet build Aida.ParallelChange.sln -c Release
        if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

        & (Join-Path $PSScriptRoot 'check-shell-eol.ps1')
        if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

        & (Join-Path $PSScriptRoot 'test.ps1')
        if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

        dotnet test Aida.ParallelChange.sln -c Release --filter "TestCategory=NarrowIntegration"
        if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

        & (Join-Path $PSScriptRoot 'coverage.ps1')
        if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

        & (Join-Path $PSScriptRoot 'mutation.ps1')
        if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

        & (Join-Path $PSScriptRoot 'up.ps1')
        if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

        & (Join-Path $PSScriptRoot 'smoke.ps1')
        if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
    }
}
finally {
    Invoke-Cleanup
}
