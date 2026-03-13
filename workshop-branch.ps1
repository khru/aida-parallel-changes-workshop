$ErrorActionPreference = 'Stop'

$Branches = @(
    'workshop/initial-state',
    'workshop/expand',
    'workshop/migrate',
    'workshop/contract'
)

Set-Location $PSScriptRoot

git rev-parse --is-inside-work-tree *> $null
if ($LASTEXITCODE -ne 0) {
    throw 'This script must run inside the workshop repository.'
}

function Get-CurrentBranch {
    git branch --show-current
}

function Ensure-Clean {
    $status = git -c core.filemode=false status --porcelain
    if ($status) {
        throw 'Working tree is not clean. Commit or stash your changes before switching branches.'
    }
}

function Show-Branches {
    $current = Get-CurrentBranch
    Write-Host 'Workshop branches:'

    for ($i = 0; $i -lt $Branches.Count; $i++) {
        $marker = ' '
        if ($Branches[$i] -eq $current) {
            $marker = '*'
        }
        Write-Host ('  {0} {1}. {2}' -f $marker, ($i + 1), $Branches[$i])
    }
}

function Get-BranchIndex([string] $branchName) {
    return [Array]::IndexOf($Branches, $branchName)
}

function Checkout-WorkshopBranch([string] $branchName) {
    Ensure-Clean
    git checkout $branchName
    Write-Host "Now on $branchName"
}

function Go-Next {
    $current = Get-CurrentBranch
    $index = Get-BranchIndex $current

    if ($index -lt 0) {
        Checkout-WorkshopBranch $Branches[0]
        return
    }

    if ($index -ge ($Branches.Count - 1)) {
        Write-Host "Already at the last workshop branch: $($Branches[$index])"
        return
    }

    Checkout-WorkshopBranch $Branches[$index + 1]
}

function Go-Previous {
    $current = Get-CurrentBranch
    $index = Get-BranchIndex $current

    if ($index -lt 0) {
        Checkout-WorkshopBranch $Branches[0]
        return
    }

    if ($index -le 0) {
        Write-Host "Already at the first workshop branch: $($Branches[$index])"
        return
    }

    Checkout-WorkshopBranch $Branches[$index - 1]
}

function Go-ToPhase([string] $phase) {
    if ($phase -eq 'initial-state' -or $phase -eq 'initial' -or $phase -eq '1') {
        Checkout-WorkshopBranch $Branches[0]
        return
    }

    if ($phase -eq 'expand' -or $phase -eq '2') {
        Checkout-WorkshopBranch $Branches[1]
        return
    }

    if ($phase -eq 'migrate' -or $phase -eq '3') {
        Checkout-WorkshopBranch $Branches[2]
        return
    }

    if ($phase -eq 'contract' -or $phase -eq '4') {
        Checkout-WorkshopBranch $Branches[3]
        return
    }

    throw 'Use one of: initial-state, expand, migrate, contract'
}

function Show-Usage {
    Write-Host 'Usage:'
    Write-Host '  .\workshop-branch.ps1                 Open interactive mode'
    Write-Host '  .\workshop-branch.ps1 list            Show workshop branches'
    Write-Host '  .\workshop-branch.ps1 branches        Show workshop branches'
    Write-Host '  .\workshop-branch.ps1 next            Checkout the next workshop branch'
    Write-Host '  .\workshop-branch.ps1 prev            Checkout the previous workshop branch'
    Write-Host '  .\workshop-branch.ps1 goto migrate    Checkout a specific workshop branch'
}

function Open-InteractiveMenu {
    while ($true) {
        Write-Host ''
        Show-Branches
        Write-Host ''
        Write-Host 'Choose an action:'
        Write-Host '  1) initial-state'
        Write-Host '  2) expand'
        Write-Host '  3) migrate'
        Write-Host '  4) contract'
        Write-Host '  n) next'
        Write-Host '  p) previous'
        Write-Host '  l) list'
        Write-Host '  q) quit'
        $choice = Read-Host '>'

        if ($choice -eq '1') {
            Go-ToPhase 'initial-state'
            continue
        }

        if ($choice -eq '2') {
            Go-ToPhase 'expand'
            continue
        }

        if ($choice -eq '3') {
            Go-ToPhase 'migrate'
            continue
        }

        if ($choice -eq '4') {
            Go-ToPhase 'contract'
            continue
        }

        if ($choice -eq 'n' -or $choice -eq 'N') {
            Go-Next
            continue
        }

        if ($choice -eq 'p' -or $choice -eq 'P') {
            Go-Previous
            continue
        }

        if ($choice -eq 'l' -or $choice -eq 'L') {
            Show-Branches
            continue
        }

        if ($choice -eq 'q' -or $choice -eq 'Q') {
            return
        }

        Write-Host 'Unknown option'
    }
}

param(
    [Parameter(Position = 0)]
    [string] $Command,
    [Parameter(Position = 1)]
    [string] $Target
)

if (-not $PSBoundParameters.ContainsKey('Command')) {
    Open-InteractiveMenu
    return
}

if ($Command -eq 'list') {
    Show-Branches
    return
}

if ($Command -eq 'branches') {
    Show-Branches
    return
}

if ($Command -eq 'next') {
    Go-Next
    return
}

if ($Command -eq 'prev') {
    Go-Previous
    return
}

if ($Command -eq 'goto') {
    if (-not $Target) {
        Show-Usage
        exit 1
    }

    Go-ToPhase $Target
    return
}

if ($Command -eq 'help') {
    Show-Usage
    return
}

Show-Usage
exit 1
