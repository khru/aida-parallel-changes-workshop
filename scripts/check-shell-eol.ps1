$ErrorActionPreference = 'Stop'


. "$PSScriptRoot/common.ps1"
Import-AidaEnv

Invoke-InAidaRepoRoot {
    $shellFiles = git ls-files '*.sh'
    $filesWithCrlf = @()

    foreach ($shellFile in $shellFiles) {
        if (-not (Test-Path $shellFile)) {
            continue
        }

        $content = [System.IO.File]::ReadAllText($shellFile)
        if ($content.Contains("`r")) {
            $filesWithCrlf += $shellFile
        }
    }

    if ($filesWithCrlf.Count -gt 0) {
        Write-Error "CRLF detected in shell scripts:`n$($filesWithCrlf -join "`n")"
        exit 1
    }
}
