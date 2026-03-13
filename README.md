# AIDA Parallel Change Workshop

This repository contains a hands-on workshop for teaching parallel change in a .NET 10 HTTP API.

## Scenario

The starting API exposes a legacy customer contact resource.

`GET /api/v1/customer-contacts/{customerId}` returns a flat contract with `contactName`, `phone`, and `email`.

The target contract is structured. The system must evolve without breaking unknown consumers.

## Branches

- `main`
- `workshop/initial-state`
- `workshop/expand`
- `workshop/migrate`
- `workshop/contract`
- `workshop/starter`
- `workshop/facilitator-notes`

## Tooling

- .NET 10
- ASP.NET Core controllers
- OpenAPI
- JSON:API media type
- SQL Server
- Dapper
- FluentMigrator
- NUnit
- NSubstitute
- Shouldly
- AwesomeAssertions
- JetBrains HTTP Client CLI

## Quick start

```bash
./scripts/up.sh
./scripts/migrate.sh
./scripts/test.sh
./scripts/smoke.sh
```

## Verification

```bash
./scripts/verify.sh
```

Use `scripts/verify.ps1` on Windows PowerShell.
