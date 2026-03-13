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

## How participants start

All participants must create their own working branch from `workshop/initial-state` before they begin the hands-on exercise.

The branch name is up to each group.

Example:

```bash
git checkout workshop/initial-state
git checkout -b team-1-solution
```

Do not ask participants to work directly on `workshop/expand`, `workshop/migrate`, or `workshop/contract`.

## Facilitator walkthrough

Use one of these helper scripts from the project root:

- `./workshop-branch.sh`
- `./workshop-branch.ps1`

They let the facilitator move through the workshop branches in order and use the repository as a step-by-step resolution.

## Documentation map

- `INSTRUCTIONS.md`: contexto de negocio y objetivos del ejercicio
- `DOCUMENTATION.md`: teoria y tecnica de parallel change aplicada al repo
- `FACILITATION.md`: guia de facilitacion para la sesion
- `AGENTS.md`: reglas de contribucion y calidad para agentes y colaboradores
- `adr/ADR-00X.md`: decisiones de arquitectura

## Current branch snapshot

This branch represents: the initial live system before parallel change begins

It contains the legacy `v1` contract, the initial SQL Server schema, and the starting point for all participants.

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
