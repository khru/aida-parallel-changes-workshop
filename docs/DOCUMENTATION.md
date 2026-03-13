# Documentation

## Purpose

This document explains the technical approach used in the workshop and why each phase exists.

## What parallel change means here

Parallel change turns one breaking migration into several safe increments.

Core rule set:

- Add before remove.
- Keep old and new paths side by side while consumers migrate.
- Remove transitional code only after dependency shutdown is confirmed.

## Techniques covered in this workshop

### 1. Endpoint versioning

Expose `v2` while keeping `v1` alive during migration.

### 2. Additive schema migrations

Introduce new columns first without removing legacy columns.

### 3. Dual-write

Keep old and new shapes in sync while coexistence is required.

### 4. Backfill in batches

Migrate historical rows incrementally to avoid large risky data moves.

### 5. Contract-first validation with executable HTTP files

Use `.http` requests as runnable endpoint documentation and smoke checks.

### 6. Explicit contraction

After migration is complete, remove old endpoints, old columns, and old queries.

## Additional migration approaches and where they fit

### Strangler pattern

Useful when old and new APIs must coexist under separate gateways.

### Tolerant reader

Useful when clients must accept additional non-breaking fields during rollout.

### Tolerant writer

Useful when writes must remain valid across mixed contract generations.

### Event-driven projection backfill

Useful when storage models are rebuilt asynchronously from event streams.

## Why SQL Server + Dapper + FluentMigrator

- SQL Server keeps production behavior close to enterprise reality.
- Dapper keeps SQL explicit and reviewable.
- FluentMigrator keeps schema evolution versioned and reproducible.

## Phase behavior

### Initial state

- Only `v1` is public.
- Legacy schema is authoritative.

### Expand

- `v1` remains available.
- `v2` read path appears.
- New schema columns are introduced.

### Migrate

- Historical data is backfilled.
- `v2` write path appears.
- Compatibility is still preserved.

### Contract

- Legacy API and storage paths are removed.
- Final state keeps only target shape and target contract.

## Point of no easy return

The point of no easy return happens when legacy read or write paths are no longer trustworthy as rollback targets because data freshness diverges.

The workshop keeps this explicit so teams can decide rollback strategy before crossing that point.

## Validation strategy

- Build and tests for each change.
- Migration execution checks.
- Smoke checks through `.http` requests.
- Branch replay and commit-by-commit verification scripts.
