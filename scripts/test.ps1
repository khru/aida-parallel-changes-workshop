$ErrorActionPreference = 'Stop'

. "$PSScriptRoot/common.ps1"
Import-AidaEnv

Invoke-InAidaRepoRoot {
    dotnet test Aida.ParallelChange.sln -c Release --filter "TestCategory!=NarrowIntegration"
}
