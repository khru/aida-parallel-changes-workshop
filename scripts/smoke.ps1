. "$PSScriptRoot/common.ps1"
Import-AidaEnv

$requests = @(
    'http/v1/get-customer-contact.http',
    'http/v1/update-customer-contact.http',
    'http/v2/get-customer-contact.http',
    'http/v2/update-customer-contact.http'
)

foreach ($request in $requests) {
    if (Test-Path $request) {
        docker compose run --rm ijhttp --env-file $env:AIDA_HTTP_ENV_FILE --env $env:AIDA_HTTP_ENV $request
        if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
    }
}
