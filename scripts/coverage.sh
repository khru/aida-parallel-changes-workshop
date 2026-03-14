#!/usr/bin/env bash
set -euo pipefail

source "$(dirname "$0")/common.sh"

ensure_repo_root

dotnet test tests/Aida.ParallelChange.Api.Tests/Aida.ParallelChange.Api.Tests.csproj \
  -c Release \
  --filter "TestCategory!=NarrowIntegration" \
  /p:CollectCoverage=true \
  /p:CoverletOutputFormat=json \
  /p:Include="[Aida.ParallelChange.Api]Aida.ParallelChange.Api.Application.*%2c[Aida.ParallelChange.Api]Aida.ParallelChange.Api.Domain.*%2c[Aida.ParallelChange.Api]Aida.ParallelChange.Api.Infrastructure.Http.Controllers.*%2c[Aida.ParallelChange.Api]Aida.ParallelChange.Api.Infrastructure.InMemory.*" \
  /p:Threshold=100 \
  /p:ThresholdType=line \
  /p:ThresholdStat=total

dotnet test tests/Aida.ParallelChange.Api.Tests/Aida.ParallelChange.Api.Tests.csproj \
  -c Release \
  --filter "TestCategory!=NarrowIntegration" \
  /p:CollectCoverage=true \
  /p:CoverletOutputFormat=json \
  /p:Include="[Aida.ParallelChange.Api]Aida.ParallelChange.Api.Application.*%2c[Aida.ParallelChange.Api]Aida.ParallelChange.Api.Domain.*%2c[Aida.ParallelChange.Api]Aida.ParallelChange.Api.Infrastructure.Http.Controllers.*%2c[Aida.ParallelChange.Api]Aida.ParallelChange.Api.Infrastructure.InMemory.*" \
  /p:Threshold=100 \
  /p:ThresholdType=branch \
  /p:ThresholdStat=total
