$ErrorActionPreference = 'Stop'

. "$PSScriptRoot/common.ps1"
Import-AidaEnv

$dryRun = $false
if ($args.Count -gt 0 -and $args[0] -eq '--dry-run') {
    $dryRun = $true
}

$status = git status --porcelain
if ($status) {
    throw 'Working tree must be clean before running god-mode.'
}

$startRef = git rev-parse HEAD

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

function Invoke-TestRun {
    dotnet test Aida.ParallelChange.sln -c Release *> $null
    return ($LASTEXITCODE -eq 0)
}

if (-not (Invoke-TestRun)) {
    .\scripts\play-alert.ps1
    exit 1
}

$lastGreen = $true
$previousHash = Get-WatchHash

while ($true) {
    $currentHash = Get-WatchHash
    if ($currentHash -ne $previousHash) {
        if (Invoke-TestRun) {
            $lastGreen = $true
        }
        else {
            if ($lastGreen) {
                .\scripts\play-alert.ps1
                if (-not $dryRun) {
                    git reset --hard $startRef *> $null
                    git clean -fd *> $null
                }
                exit 1
            }
            $lastGreen = $false
        }

        $previousHash = $currentHash
    }

    Start-Sleep -Seconds ([int]$env:AIDA_WATCH_INTERVAL_SECONDS)
}
