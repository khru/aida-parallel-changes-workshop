# Documentation

## What this repository teaches

This workshop teaches parallel change in a controller-based ASP.NET Core API backed by SQL Server.

The repository is organised so that the HTTP contract, the domain model, and the database evolve in small safe steps.

## Parallel change

Parallel change breaks an incompatible change into safe intermediate states.

The essential rule is simple:

- never remove something until nothing depends on it
- never force adoption of something that does not yet exist

## Phases used in this repository

### Initial state

The system exposes only the legacy `v1` contract and stores only the legacy shape in SQL Server.

### Expand

The repository adds:

- `v2` endpoints
- new columns for the structured shape
- domain value objects for the new concepts
- dual-write so both shapes stay current

### Migrate

The repository adds:

- batched backfill for old rows
- `v2` writes
- reads that use the new persisted shape

### Contract

The repository removes:

- `v1`
- legacy columns
- legacy queries
- temporary compatibility code

## Why Dapper and FluentMigrator

The workshop uses Dapper because the SQL stays explicit while the schema is in motion.

The workshop uses FluentMigrator because the database evolution is part of the teaching, not a hidden implementation detail.
