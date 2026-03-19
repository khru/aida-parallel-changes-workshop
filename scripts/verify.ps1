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

function Assert-PlanIsLocalOnly {
    git ls-files --error-unmatch plan.md *> $null
    if ($LASTEXITCODE -eq 0) {
        throw 'plan.md is local-only and must not be tracked by git.'
    }
}

function Assert-GeneratedArtifactsAreNotTracked {
    $trackedFiles = git ls-files
    $trackedGenerated = @()

    foreach ($trackedFile in $trackedFiles) {
        if ($trackedFile -like 'artifacts/*' -or
            $trackedFile -like 'TestResults/*' -or
            $trackedFile -like 'StrykerOutput/*' -or
            $trackedFile -like '.stryker-tmp/*' -or
            $trackedFile -like '*/bin/*' -or
            $trackedFile -like '*/obj/*' -or
            $trackedFile -like 'coverage.json' -or
            $trackedFile -like 'coverage*.json' -or
            $trackedFile -like '*/coverage.json' -or
            $trackedFile -like '*/coverage*.json') {
            $trackedGenerated += $trackedFile
        }
    }

    if ($trackedGenerated.Count -gt 0) {
        throw "Generated artifacts must not be tracked by git:`n$($trackedGenerated -join "`n")"
    }
}

try {
    Invoke-InAidaRepoRoot {
        Assert-PlanIsLocalOnly
        Assert-GeneratedArtifactsAreNotTracked
        Assert-DockerAvailable

        dotnet restore Aida.ParallelChange.sln
        if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

        dotnet build Aida.ParallelChange.sln -c Release -warnaserror
        if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

        & (Join-Path $PSScriptRoot 'check-shell-eol.ps1')
        if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

        & (Join-Path $PSScriptRoot 'test.ps1')
        if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

        & (Join-Path $PSScriptRoot 'test.ps1')
        if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

        & (Join-Path $PSScriptRoot 'test-integration.ps1')
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
