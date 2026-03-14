. "$PSScriptRoot/common.ps1"
Import-AidaEnv
 
Invoke-InAidaRepoRoot {
    Assert-DockerAvailable
    Remove-LegacyContainers
    Remove-PortCollisions

    $requests = @(
        'http/v1/customer-contacts/scenario-create-get-update-get.http',
        'http/system/health-200.http',
        'http/system/openapi-v1-200.http'
    )

    foreach ($request in $requests) {
        if (Test-Path $request) {
            Invoke-Compose run --rm ijhttp --env-file $env:AIDA_HTTP_ENV_FILE --env $env:AIDA_HTTP_ENV $request
            if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
        }
    }
}
