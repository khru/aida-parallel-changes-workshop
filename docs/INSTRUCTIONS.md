# Instructions

## Product context

AIDA exposes a .NET HTTP API used by internal and external consumers.

The current production contract is flat and legacy. The business wants a structured shape that supports better validation and clearer semantics, but compatibility with unknown consumers must remain intact throughout the transition.

## Current system status

The starting system serves and updates customer contact data through a legacy contract.

Legacy fields:

- `contactName`
- `phone`
- `email`

Legacy endpoints:

- `GET /api/v1/customer-contacts/{customerId}`
- `PUT /api/v1/customer-contacts/{customerId}`

Persistence starts from a legacy table shape and evolves through additive migrations.

## Workshop objective

Apply parallel change safely from legacy to structured contracts and data models through reversible intermediate states.

## Non-negotiable constraints

- Do not break compatibility for existing consumers.
- Keep every branch runnable.
- Keep every branch buildable and testable.
- Keep migration and smoke scripts executable.

## Execution flow

1. Start from `workshop/initial-state`.
2. Implement expansion in `workshop/expand`.
3. Move and validate data in `workshop/migrate`.
4. Remove legacy paths in `workshop/contract`.

## Branch goals

### workshop/initial-state

- Legacy `v1` contract is the only public contract.
- SQL schema and repository support `v1`.
- Acceptance tests validate observable v1 behavior.

### workshop/expand

- Add `v2` read contract without removing `v1`.
- Add new persistence columns for the target shape.
- Keep dual-write behavior while coexistence is required.

### workshop/migrate

- Backfill existing rows in batches.
- Add `v2` write contract.
- Keep compatibility while traffic and data move.

### workshop/contract

- Remove `v1` surface and legacy storage paths.
- Keep only target `v2` contract and target schema.

## Definition of done for any phase

- Build succeeds.
- Tests are green.
- Migrations run successfully.
- HTTP smoke requests pass.
- Branch README includes phase-specific architecture and sequence diagrams.

## Standard validation commands

```bash
dotnet restore Aida.ParallelChange.sln
dotnet build Aida.ParallelChange.sln -c Release
dotnet test Aida.ParallelChange.sln -c Release
./scripts/up.sh
./scripts/smoke.sh
./scripts/down.sh
```
