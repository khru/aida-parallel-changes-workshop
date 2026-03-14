# Facilitation

## Session objective

Help participants evolve API and data contracts with production-grade discipline.

## Facilitator framing

State this explicitly at the start:

- This is a workshop, but participants must work as in real day-to-day delivery.
- OpenAPI is part of the workflow, not optional documentation.
- Every cycle must include tests, refactor, and documentation alignment.

## Suggested timeline

- 15 min: context and rules.
- 75 min: iterative implementation.
- 20 min: debrief and architecture review.

## Recommended team setup

- Pair or mob in groups of 2-3.
- Rotate driver every 15 minutes.
- Navigator enforces TDD and no-regression discipline.

## Facilitation sequence

1. Run baseline (`up`, `smoke`, `test`).
2. Explain current contract and OpenAPI document.
3. Execute one full double-loop TDD cycle live.
4. Show refactor of production and tests in the same cycle.
5. Show commit message with phase marker (`expand`, `migrate`, `contract`).

## Coaching checkpoints

- Are participants preserving compatibility?
- Are they checking OpenAPI after contract changes?
- Are tests behavior-focused (not structure-focused)?
- Are commits small and phase-explicit?
- Is `to-do.md` updated without mutating accepted AC?

## Debrief prompts

- Which change improved safety the most?
- Where did design pressure appear first?
- Which mutant was hardest to kill and why?
- Which documentation update prevented confusion?
