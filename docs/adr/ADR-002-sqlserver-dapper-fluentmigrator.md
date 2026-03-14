# ADR-002: SQL Server with Dapper and FluentMigrator

## Status

Accepted

## Context

The workshop requires transparent schema and query evolution to teach dual-write, backfill, and contraction.

## Decision drivers

- SQL evolution must stay explicit.
- Migrations must be executable and reviewable.
- Runtime behavior must remain close to production-like enterprise setups.

## Considered options

### Option A: Entity Framework Core

- Pros: rapid CRUD development
- Cons: hides SQL shape transitions and migration SQL details needed for workshop pedagogy

### Option B: SQL Server + Dapper + FluentMigrator

- Pros: explicit SQL, explicit migrations, clear data-shape transitions
- Cons: more manual mapping and SQL maintenance

### Option C: In-memory persistence only

- Pros: fast local feedback
- Cons: misses core migration learning outcomes and storage compatibility risks

## Decision

Adopt Option B.

## Consequences

- Schema changes remain visible and auditable.
- Backfill behavior can be demonstrated concretely.
- Participants understand data migration trade-offs directly.

## Risks

- SQL scripts can drift from domain changes.
- Manual mapping errors can increase.

## Mitigations

- Keep acceptance and migration coverage in place.
- Keep SQL mapping tests focused on externally visible behavior.

## Rollback strategy

If runtime complexity blocks workshop progress, keep SQL Server and FluentMigrator, but temporarily simplify mapping complexity while preserving migration visibility.
