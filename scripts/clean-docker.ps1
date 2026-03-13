$ErrorActionPreference = 'Stop'

docker compose down --remove-orphans --volumes
docker network prune -f
docker volume prune -f
