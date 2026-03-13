# Facilitation

## Session objective

Guide participants to evolve an API contract without breaking unknown consumers while preserving delivery speed.

## Suggested timeline

- 15 minutes: context and guardrails
- 60 minutes: guided implementation
- 15 minutes: debrief and architecture review

## Recommended team setup

- Teams of 2 or 3 people
- Rotate driver and navigator every 15 minutes
- Navigator enforces test quality and branch discipline

## Teaching sequence

1. Run `workshop/initial-state` and prove baseline behavior.
2. Explain why direct breaking migration is risky.
3. Move to `workshop/expand` and show coexistence.
4. Move to `workshop/migrate` and show controlled data migration.
5. Move to `workshop/contract` and remove transitional debt.

## Facilitation checkpoints

### Checkpoint 1

Participants can explain legacy contract behavior and test coverage.

### Checkpoint 2

Participants can explain why coexistence and dual-write are temporary but necessary.

### Checkpoint 3

Participants can run backfill and explain rollback limitations.

### Checkpoint 4

Participants can justify contract phase deletion decisions.

## Quality guardrails during the session

- One failing test at a time.
- Failure must be for the intended reason.
- No progression with red tests.
- Keep branch history coherent and focused.

## Debrief prompts

- Where is the point of no easy return in your solution?
- What decision gave the highest safety gain?
- Which transitional code was hardest to delete and why?
- Which validation step gave the highest confidence?
