$ErrorActionPreference = 'Stop'

. "$PSScriptRoot/common.ps1"
Import-AidaEnv

Invoke-InAidaRepoRoot {
    $projectFiles = @(git ls-files '*.csproj')

    if ($projectFiles.Count -eq 0) {
        throw 'No project files were found to restore.'
    }

    foreach ($projectFile in $projectFiles) {
        dotnet restore $projectFile
        if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
    }
}
