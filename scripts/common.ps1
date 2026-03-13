$ErrorActionPreference = 'Stop'

function Import-AidaEnv {
    if (Test-Path '.env') {
        Get-Content '.env' | ForEach-Object {
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
    if (-not $env:AIDA_SQL_READY_ATTEMPTS) { $env:AIDA_SQL_READY_ATTEMPTS = '60' }
    if (-not $env:AIDA_SQL_READY_SLEEP_SECONDS) { $env:AIDA_SQL_READY_SLEEP_SECONDS = '2' }
    if (-not $env:AIDA_HTTP_ENV_FILE) { $env:AIDA_HTTP_ENV_FILE = 'http/environments/local.http-client.env.json' }
    if (-not $env:AIDA_HTTP_ENV) { $env:AIDA_HTTP_ENV = 'docker' }
    if (-not $env:AIDA_WATCH_INTERVAL_SECONDS) { $env:AIDA_WATCH_INTERVAL_SECONDS = '2' }
    if (-not $env:AIDA_ALERT_SOUND) { $env:AIDA_ALERT_SOUND = 'C:\Windows\Media\Windows Critical Stop.wav' }
}

function Wait-ForSqlServer {
    $attempts = [int]$env:AIDA_SQL_READY_ATTEMPTS
    $sleepSeconds = [int]$env:AIDA_SQL_READY_SLEEP_SECONDS

    for ($attempt = 1; $attempt -le $attempts; $attempt++) {
        docker compose exec -T sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U $env:AIDA_SQL_USER -P $env:AIDA_SQL_PASSWORD -Q "SELECT 1" -C *> $null
        if ($LASTEXITCODE -eq 0) {
            return
        }

        Start-Sleep -Seconds $sleepSeconds
    }

    throw 'SQL Server did not become ready in time.'
}

function Ensure-DatabaseExists {
    docker compose exec -T sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U $env:AIDA_SQL_USER -P $env:AIDA_SQL_PASSWORD -Q "IF DB_ID('$($env:AIDA_SQL_DATABASE)') IS NULL CREATE DATABASE [$($env:AIDA_SQL_DATABASE)]" -C *> $null
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
}
