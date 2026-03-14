$ErrorActionPreference = 'Stop'

. "$PSScriptRoot/common.ps1"
Import-AidaEnv

$branch = 'main'

$dryRun = $false
$auto = $false
$delaySeconds = 2

for ($i = 0; $i -lt $args.Count; $i++) {
    switch ($args[$i]) {
        '--dry-run' { $dryRun = $true }
        '--auto' { $auto = $true }
        '--delay' {
            $delaySeconds = [int]$args[$i + 1]
            $i++
        }
        default { throw "Unknown option: $($args[$i])" }
    }
}

Invoke-InAidaRepoRoot {
    $status = git status --porcelain
    if ($status) {
        throw 'Working tree must be clean before replay.'
    }

    $initialRef = git branch --show-current
    if (-not $initialRef) {
        $initialRef = git rev-parse HEAD
    }

    function Restore-Ref {
        git checkout $initialRef *> $null
    }

    try {
        git show-ref --verify --quiet "refs/heads/$branch"
        if ($LASTEXITCODE -ne 0) {
            throw "Branch '$branch' does not exist."
        }

        $range = git rev-list --reverse $branch

        foreach ($commit in $range) {
            if (-not $commit) { continue }
            $message = git show -s --format='%s' $commit
            Write-Output "$branch $commit $message"

            if (-not $dryRun) {
                git checkout --detach $commit *> $null
                if ($auto) {
                    Start-Sleep -Seconds $delaySeconds
                }
                else {
                    Read-Host 'Press enter to continue'
                }
            }
        }
    }
    finally {
        Restore-Ref
    }
}
