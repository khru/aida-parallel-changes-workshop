$ErrorActionPreference = 'Stop'

. "$PSScriptRoot/common.ps1"
Import-AidaEnv

function Get-WatchHash {
    $files = git ls-files '*.cs' '*.csproj' '*.json' '*.http' '*.yml' '*.yaml' '*.sh' '*.ps1'
    if (-not $files) {
        return ''
    }

    $content = ($files | ForEach-Object { (Get-FileHash $_ -Algorithm SHA256).Hash }) -join ''
    $bytes = [Text.Encoding]::UTF8.GetBytes($content)
    $sha = [Security.Cryptography.SHA256]::Create()
    try {
        $hash = $sha.ComputeHash($bytes)
        return [BitConverter]::ToString($hash).Replace('-', '').ToLowerInvariant()
    }
    finally {
        $sha.Dispose()
    }
}

$previousHash = ''

while ($true) {
    $currentHash = Get-WatchHash

    if ($currentHash -ne $previousHash) {
        dotnet test Aida.ParallelChange.sln -c Release
        if ($LASTEXITCODE -ne 0) {
            .\scripts\play-alert.ps1
        }

        $previousHash = $currentHash
    }

    Start-Sleep -Seconds ([int]$env:AIDA_WATCH_INTERVAL_SECONDS)
}
