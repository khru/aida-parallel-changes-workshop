docker compose run --rm ijhttp --env-file http/environments/local.http-client.env.json http/v1/get-customer-contact.http
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
