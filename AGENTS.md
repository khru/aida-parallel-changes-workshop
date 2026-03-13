# AGENTS

## Purpose

This file defines how contributors and coding agents work in this parallel change workshop.

## Product context

- The API is public and consumers are not fully controlled.
- Production compatibility cannot break.
- The workshop teaches safe contract and data evolution.

## Technical stack

- .NET 10
- ASP.NET Core controllers
- SQL Server
- Dapper
- FluentMigrator
- NUnit
- NSubstitute
- Shouldly and AwesomeAssertions
- OpenAPI
- JSON:API media type
- Docker and Docker Compose
- JetBrains HTTP Client CLI

## Branch workflow

Workshop branches:

1. `workshop/initial-state`
2. `workshop/expand`
3. `workshop/migrate`
4. `workshop/contract`

Participant flow:

```bash
git checkout workshop/initial-state
git checkout -b team-x-solution
```

## Development rules

- Keep code without comments.
- Use intention-revealing names.
- Keep clear separation of responsibilities.
- Apply SOLID principles.
- Apply dependency inversion and inversion of control.
- Favor encapsulation and immutability when useful.
- Avoid switch statements.
- Avoid magic strings and magic numbers.
- Do not use Entity Framework in this repository.

## TDD rules

- Introduce exactly one failing test at a time.
- A failing test must fail for the right business reason.
- Implement the smallest change to make the test pass.
- Refactor immediately after green.
- Prioritize sociable tests from endpoint downward.
- Use test doubles only to reduce technical coupling.

## Test quality rules

- Tests must verify observable behavior.
- Avoid assertions against internal structure or implementation details.
- Avoid fragile JSON substring assertions.
- Parse JSON payloads and assert semantic values.
- Enforce FIRST across the suite.
- Keep solitary tests only for pure value objects and invariants.
- Fix or remove non-deterministic tests.

## Test time budget

- The fast local suite must complete in 45 seconds or less.
- If the suite exceeds 45 seconds, optimization is mandatory before continuing.

## Quality gates before moving forward

```bash
dotnet restore Aida.ParallelChange.sln
dotnet build Aida.ParallelChange.sln -c Release
dotnet test Aida.ParallelChange.sln -c Release
./scripts/up.sh
./scripts/smoke.sh
./scripts/down.sh
```

## Commit policy

- Keep commits small and intentional.
- Do not mix different workshop phases in one commit.
- Every commit must leave the repository green.
- `workshop/contract` must not keep unused transitional code.
- Avoid merge commits in workshop branches.

## Documentation policy

- Keep `README.md` current with runnable instructions and branch-specific diagrams.
- Keep `docs/INSTRUCTIONS.md`, `docs/DOCUMENTATION.md`, and `docs/FACILITATION.md` current.
- Keep ADRs in `docs/adr` current.
- Create a new ADR when architecture decisions change.
