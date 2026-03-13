set -e
dotnet restore Aida.ParallelChange.sln
dotnet build Aida.ParallelChange.sln -c Release
dotnet test Aida.ParallelChange.sln -c Release
docker compose up -d sqlserver
dotnet run --project src/Aida.ParallelChange.Migrator/Aida.ParallelChange.Migrator.csproj
docker compose up -d api
docker compose run --rm ijhttp --env-file http/environments/local.http-client.env.json http/v1/get-customer-contact.http
