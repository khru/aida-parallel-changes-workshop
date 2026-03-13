$ErrorActionPreference = 'Stop'

function Invoke-Cleanup {
    try {
        .\scripts\down.ps1 *> $null
    }
    catch {
    }
}

try {
    dotnet restore Aida.ParallelChange.sln
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

    dotnet build Aida.ParallelChange.sln -c Release
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

    dotnet test Aida.ParallelChange.sln -c Release
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

    .\scripts\up.ps1
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

    .\scripts\smoke.ps1
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
}
finally {
    Invoke-Cleanup
}
