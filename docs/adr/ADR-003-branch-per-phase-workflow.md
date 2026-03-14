# ADR-003: Branch-per-phase workshop workflow

## Status

Superseded by ADR-004

## Context

The workshop must provide a clear narrative from legacy baseline to contracted final state.

## Decision drivers

- Keep pedagogical flow explicit.
- Make each phase runnable and reviewable.
- Keep facilitator navigation simple in live sessions.

## Considered options

### Option A: Single branch with tags

- Pros: simple branch model
- Cons: less discoverable phase boundaries, weaker day-to-day branch ergonomics

### Option B: Dedicated branch per phase

- Pros: explicit phase snapshots, straightforward facilitator progression, easier branch-level smoke checks
- Cons: requires branch consistency discipline

### Option C: Separate repositories per phase

- Pros: hard isolation
- Cons: high maintenance overhead and duplicated maintenance effort

## Decision

Adopt Option B.

## Consequences

- Facilitators can move through phases predictably.
- Participants can start from a stable initial branch.
- Automation can validate phase behavior branch-by-branch.

## Risks

- Branch drift can appear if transversal fixes are not propagated.
- Historical coherence can degrade after uncoordinated rewrites.

## Mitigations

- Use shared scripts and common branch checks.
- Use replay and history verification scripts.

## Rollback strategy

If branch drift becomes unmanageable, rebuild phase branches from a validated base branch and replay phase commits in order.
