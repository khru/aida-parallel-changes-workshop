$ErrorActionPreference = 'Stop'

$script:AidaRepoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path

function Invoke-InAidaRepoRoot {
    param(
        [Parameter(Mandatory = $true)]
        [scriptblock]$ScriptBlock
    )

    Push-Location $script:AidaRepoRoot
    try {
        & $ScriptBlock
    }
    finally {
        Pop-Location
    }
}

function Import-AidaEnv {
    $envFilePath = Join-Path $script:AidaRepoRoot '.env'

    if (Test-Path $envFilePath) {
        Get-Content $envFilePath | ForEach-Object {
            if ($_ -match '^(?<name>[A-Za-z_][A-Za-z0-9_]*)=(?<value>.*)$') {
                $name = $Matches['name']
                $value = $Matches['value']
                [Environment]::SetEnvironmentVariable($name, $value)
            }
        }
    }

    if (-not $env:AIDA_SQL_DATABASE) { $env:AIDA_SQL_DATABASE = 'AidaParallelChange' }
    if (-not $env:AIDA_SQL_USER) { $env:AIDA_SQL_USER = 'sa' }
    if (-not $env:AIDA_SQL_PASSWORD) { $env:AIDA_SQL_PASSWORD = 'Your_strong_password_123' }
    if (-not $env:AIDA_SQL_PORT) { $env:AIDA_SQL_PORT = '14333' }
    if (-not $env:AIDA_API_PORT) { $env:AIDA_API_PORT = '8080' }
    if (-not $env:AIDA_SQL_READY_ATTEMPTS) { $env:AIDA_SQL_READY_ATTEMPTS = '60' }
    if (-not $env:AIDA_SQL_READY_SLEEP_SECONDS) { $env:AIDA_SQL_READY_SLEEP_SECONDS = '2' }
    if (-not $env:AIDA_HTTP_ENV_FILE) { $env:AIDA_HTTP_ENV_FILE = 'http/environments/local.http-client.env.json' }
    if (-not $env:AIDA_HTTP_ENV) { $env:AIDA_HTTP_ENV = 'docker' }
    if (-not $env:AIDA_COMPOSE_PROJECT_NAME) { $env:AIDA_COMPOSE_PROJECT_NAME = 'aida-parallel-change' }
}

function Invoke-Compose {
    param(
        [Parameter(ValueFromRemainingArguments = $true)]
        [string[]]$Arguments
    )

    & docker compose --project-name $env:AIDA_COMPOSE_PROJECT_NAME @Arguments
}

function Test-ContainerExistsByName {
    param(
        [Parameter(Mandatory = $true)]
        [string]$ContainerName
    )

    $containerNames = docker ps -a --format '{{.Names}}'
    foreach ($name in $containerNames) {
        if ($name -eq $ContainerName) {
            return $true
        }
    }

    return $false
}

function Remove-LegacyContainers {
    $legacyContainerNames = @(
        'aida-parallel-change-sqlserver',
        'aida-parallel-change-api',
        'aida-parallel-change-migrator',
        'aida-parallel-change-ijhttp'
    )

    foreach ($legacyContainerName in $legacyContainerNames) {
        if (Test-ContainerExistsByName -ContainerName $legacyContainerName) {
            docker rm -f $legacyContainerName *> $null
        }
    }
}

function Remove-PortCollisions {
    $projectNamePrefix = "$($env:AIDA_COMPOSE_PROJECT_NAME)-"
    $collidingContainerIds = [System.Collections.Generic.HashSet[string]]::new()
    $containerMetadata = docker ps --format '{{.ID}}|{{.Names}}|{{.Ports}}'

    foreach ($entry in $containerMetadata) {
        if ([string]::IsNullOrWhiteSpace($entry)) {
            continue
        }

        $parts = $entry -split '\|', 3
        if ($parts.Count -ne 3) {
            continue
        }

        $containerId = $parts[0]
        $containerName = $parts[1]
        $publishedPorts = $parts[2]

        if ($containerName.StartsWith($projectNamePrefix, [System.StringComparison]::Ordinal)) {
            continue
        }

        $collidesWithSqlPort = $publishedPorts -like "*:$($env:AIDA_SQL_PORT)->*"
        $collidesWithApiPort = $publishedPorts -like "*:$($env:AIDA_API_PORT)->*"

        if ($collidesWithSqlPort -or $collidesWithApiPort) {
            [void]$collidingContainerIds.Add($containerId)
        }
    }

    if ($collidingContainerIds.Count -gt 0) {
        docker rm -f @($collidingContainerIds) *> $null
    }
}

function Recover-ComposeStackCollisions {
    Invoke-Compose down --remove-orphans *> $null
    Remove-LegacyContainers
    Remove-PortCollisions
}

function Assert-DockerAvailable {
    docker info *> $null
    if ($LASTEXITCODE -ne 0) {
        throw 'Docker is required and is not available.'
    }
}

function Wait-ForSqlServer {
    $attempts = [int]$env:AIDA_SQL_READY_ATTEMPTS
    $sleepSeconds = [int]$env:AIDA_SQL_READY_SLEEP_SECONDS

    for ($attempt = 1; $attempt -le $attempts; $attempt++) {
        Invoke-Compose exec -T sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U $env:AIDA_SQL_USER -P $env:AIDA_SQL_PASSWORD -Q "SELECT 1" -C *> $null
        if ($LASTEXITCODE -eq 0) {
            return
        }

        Start-Sleep -Seconds $sleepSeconds
    }

    throw 'SQL Server did not become ready in time.'
}

function Wait-ForDatabaseOnline {
    $attempts = [int]$env:AIDA_SQL_READY_ATTEMPTS
    $sleepSeconds = [int]$env:AIDA_SQL_READY_SLEEP_SECONDS

    for ($attempt = 1; $attempt -le $attempts; $attempt++) {
        $result = Invoke-Compose exec -T sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U $env:AIDA_SQL_USER -P $env:AIDA_SQL_PASSWORD -Q "SET NOCOUNT ON; IF EXISTS (SELECT 1 FROM sys.databases WHERE name = '$($env:AIDA_SQL_DATABASE)' AND state_desc = 'ONLINE') SELECT 1 ELSE SELECT 0" -h -1 -W -C 2>$null
        if ($LASTEXITCODE -eq 0 -and ($result -replace '\s', '') -eq '1') {
            return
        }

        Start-Sleep -Seconds $sleepSeconds
    }

    throw "Database '$($env:AIDA_SQL_DATABASE)' did not become online in time."
}

function Recreate-Database {
    Invoke-Compose exec -T sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -d master -U $env:AIDA_SQL_USER -P $env:AIDA_SQL_PASSWORD -Q "IF DB_ID('$($env:AIDA_SQL_DATABASE)') IS NOT NULL BEGIN ALTER DATABASE [$($env:AIDA_SQL_DATABASE)] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [$($env:AIDA_SQL_DATABASE)]; END" -b -C *> $null
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
    Invoke-Compose exec -T sqlserver /bin/sh -lc "rm -f /var/opt/mssql/data/$($env:AIDA_SQL_DATABASE).mdf /var/opt/mssql/data/$($env:AIDA_SQL_DATABASE)_log.ldf" *> $null
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
    Invoke-Compose exec -T sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -d master -U $env:AIDA_SQL_USER -P $env:AIDA_SQL_PASSWORD -Q "IF DB_ID('$($env:AIDA_SQL_DATABASE)') IS NULL CREATE DATABASE [$($env:AIDA_SQL_DATABASE)]" -b -C *> $null
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
    Wait-ForDatabaseOnline
}
