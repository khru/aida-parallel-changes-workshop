# ADR-004: Single branch with phase-labelled commits

## Status

Accepted

## Context

The workshop now runs on a single long-lived branch (`main`) that represents the initial state. We still need explicit teaching of parallel change phases.

## Decision drivers

- Reduce operational noise from branch drift.
- Keep facilitator setup simple.
- Preserve explicit phase intent for educational clarity.

## Considered options

### Option A: Keep branch-per-phase workflow

- Pros: explicit snapshots by branch
- Cons: branch maintenance overhead, drift risk

### Option B: Single branch and phase-labelled commits

- Pros: simpler workflow, less drift, clearer history when commit discipline is enforced
- Cons: requires strict commit-message governance

## Decision

Adopt Option B.

## Consequences

- `main` is the only long-lived branch for workshop execution.
- Participants must label commits by phase intent: `[expand]`, `[migrate]`, or `[contract]`.
- Commits must remain small and single-intention.

## Risks

- Phase boundaries can become less visible without commit discipline.

## Mitigations

- Enforce commit message policy in instructions and facilitation.
- Keep `to-do.md` and acceptance evidence aligned with phase intent.
