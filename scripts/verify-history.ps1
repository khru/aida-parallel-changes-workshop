$ErrorActionPreference = 'Stop'

$branches = @(
    'main',
    'workshop/initial-state',
    'workshop/expand',
    'workshop/migrate',
    'workshop/contract'
)

$full = $false
if ($args.Count -gt 0 -and $args[0] -eq '--full') {
    $full = $true
}

$status = git status --porcelain
if ($status) {
    throw 'Working tree must be clean before verify-history.'
}

$initialRef = git branch --show-current
if (-not $initialRef) {
    $initialRef = git rev-parse HEAD
}

function Restore-Ref {
    git checkout $initialRef *> $null
}

$previousBranch = $null

try {
    foreach ($branch in $branches) {
        git show-ref --verify --quiet "refs/heads/$branch"
        if ($LASTEXITCODE -ne 0) {
            continue
        }

        if (-not $previousBranch) {
            $range = git rev-list --reverse $branch
        }
        else {
            $base = git merge-base $previousBranch $branch
            $range = git rev-list --reverse "$base..$branch"
        }

        foreach ($commit in $range) {
            if (-not $commit) { continue }

            git checkout --detach $commit *> $null
            Write-Output "Checking $branch $commit"

            dotnet restore Aida.ParallelChange.sln *> $null
            if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

            dotnet build Aida.ParallelChange.sln -c Release *> $null
            if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

            dotnet test Aida.ParallelChange.sln -c Release *> $null
            if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

            if ($full -and (Test-Path 'scripts/up.ps1') -and (Test-Path 'scripts/smoke.ps1') -and (Test-Path 'scripts/down.ps1')) {
                .\scripts\down.ps1 *> $null
                .\scripts\up.ps1 *> $null
                if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
                .\scripts\smoke.ps1 *> $null
                if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
                .\scripts\down.ps1 *> $null
            }
        }

        $previousBranch = $branch
    }
}
finally {
    Restore-Ref
}
