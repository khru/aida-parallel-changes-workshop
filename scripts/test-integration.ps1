$ErrorActionPreference = 'Stop'

. "$PSScriptRoot/common.ps1"
Import-AidaEnv

Invoke-InAidaRepoRoot {
    Assert-DockerAvailable
    $env:TESTCONTAINERS_RYUK_DISABLED = if ($env:TESTCONTAINERS_RYUK_DISABLED) { $env:TESTCONTAINERS_RYUK_DISABLED } else { 'true' }
    if ($env:ACT -eq 'true' -and -not $env:TESTCONTAINERS_HOST_OVERRIDE) { $env:TESTCONTAINERS_HOST_OVERRIDE = 'host.docker.internal' }
    elseif ($env:GITHUB_ACTIONS -eq 'true' -and $env:TESTCONTAINERS_HOST_OVERRIDE -eq 'host.docker.internal') { Remove-Item Env:TESTCONTAINERS_HOST_OVERRIDE }
    dotnet test Aida.ParallelChange.sln -c Release --filter "TestCategory=NarrowIntegration"
}
