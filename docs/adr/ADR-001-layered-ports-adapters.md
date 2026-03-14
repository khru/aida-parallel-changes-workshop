# ADR-001: Layered architecture with ports and adapters

## Status

Accepted

## Context

The workshop must demonstrate safe contract and schema evolution without coupling HTTP handlers directly to SQL details.

The audience needs to see where behavioral policy lives and where infrastructure variability is isolated.

## Decision drivers

- Keep migration logic understandable during refactors.
- Support reversible intermediate states.
- Keep tests focused on observable behavior.

## Considered options

### Option A: Controller and repository direct coupling

- Pros: low upfront code volume
- Cons: migration logic leaks into transport layer, weak test boundaries, high refactor risk

### Option B: Layered architecture with ports and adapters

- Pros: explicit use cases, stable seams for migration, improved testability
- Cons: more types and dependency wiring

### Option C: Full vertical slices with duplicated infrastructure access

- Pros: local autonomy per endpoint
- Cons: duplication grows quickly in workshop phases, lower didactic clarity

## Decision

Adopt Option B.

## Consequences

- Application handlers own business behavior.
- Ports define contracts between application and infrastructure.
- Adapters can evolve independently from transport and domain decisions.

## Risks

- Participants may initially perceive added complexity.
- Over-abstraction could hide practical migration concerns.

## Mitigations

- Keep naming intention-revealing.
- Keep endpoint-to-handler path explicit in diagrams and tests.

## Rollback strategy

If educational complexity becomes too high, reduce abstraction depth while keeping at least one explicit port boundary.
