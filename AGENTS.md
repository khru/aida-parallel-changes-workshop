# AGENTS

## Global quality mandate

Every change must be coherent, cohesive, rigorous, and complete.

- Coherent: code, tests, scripts, HTTP docs, and written docs describe the same behavior.
- Cohesive: each commit has one clear intention.
- Rigorous: decisions are validated with executable evidence.
- Complete: no task is done with code only.

## Repository scope

- Single long-lived branch: `main`.
- API surface in this state:
  - `GET /api/v1/customer-contacts/{customerId}`
  - `POST /api/v1/customer-contacts`
  - `PUT /api/v1/customer-contacts/{customerId}`
- System endpoints are mandatory:
  - `GET /health`
  - `GET /openapi/v1.json`

## Non-regression contract (`to-do.md`)

- `to-do.md` is the only tracker and stays editable.
- Accepted AC are immutable: do not add, remove, weaken, or rewrite AC.
- New tests can be added at any time.
- Nothing already covered by accepted AC can break.

## Design and coding rules

- No comments in executable code.
- Names must reveal intention.
- Enforce separation of concerns.
- Enforce SOLID.
- Enforce dependency inversion and inversion of control.
- Respect the Law of Demeter.
- Prefer object calisthenics where applicable.
- Preserve encapsulation and immutability where applicable.
- Use DTO at boundaries and Value Objects for domain invariants.
- Apply patterns only when needed and according to their original intent.

## Command/query rules

- Commands do not return booleans.
- Command handlers do not return success/failure values.
- Query handlers return data.
- Command failures are represented explicitly (for example, typed errors), then mapped at the boundary.

## Double-loop TDD operating model

Use outside-in double-loop TDD for every change.

1. Update test plan in `to-do.md`.
2. Introduce one failing outer test.
3. Introduce inner tests only as needed.
4. Make green with minimum implementation.
5. Refactor production and tests.
6. Reprioritize `to-do.md`.
7. Commit before next cycle.

Only one failing test at a time.

## ZOMBIES and triangulation

- Keep ZOMBIES decomposition in `to-do.md`.
- Use triangulation and rule of three.
- Do not abstract after one example.

## Test architecture and style rules

- Test folders must be:
  - `Acceptance`
  - `Integration`
  - `Unit`
- Tests must verify observable behavior, not internal structure.
- Tests must have explicit Arrange/Act/Assert.
- Tests must not contain `if`, `switch`, or loops.
- Prefer parameterized tests before helper methods.
- Helper methods are allowed only when strictly necessary for readability.
- Do not change architecture or design only to make sociable tests easier.
- Use solitary tests or doubles only when they reduce technical coupling.

Anti-pattern examples that are not allowed:

- Tests that only verify DTO default values.
- Tests that assert framework/runtime internals instead of system behavior.

## Script rules

Required scripts must work in both shell and PowerShell variants:

- `scripts/up.*`
- `scripts/down.*`
- `scripts/migrate.*`
- `scripts/smoke.*`
- `scripts/test.*`
- `scripts/coverage.*`
- `scripts/mutation.*`
- `scripts/verify.*`
- `scripts/workshop-replay.*`
- `scripts/check-shell-eol.*`

For shell scripts:

- `*.sh` are LF-only.
- CRLF in `*.sh` is a hard failure.

## Executable HTTP documentation rules

- Keep `.http` coverage for all endpoint outcomes.
- Include `.http` for health and OpenAPI.
- Keep `.http` aligned with acceptance tests and docs.

## Commit policy

- Commits are mandatory while working.
- Commits must be small and single-intention.
- Every commit message must include phase marker and intent:
  - `[expand]`
  - `[migrate]`
  - `[contract]`

## Commit identity policy

All commits must use this identity configuration:

- GitHub handle: `evalverde-eng`
- Author: `Emmanuel Valverde Ramos <evalverde@domingoalonsogroup.com>`
- Committer: `Emmanuel Valverde Ramos <evalverde@domingoalonsogroup.com>`

The identity `OpenAI <openai@example.com>` is forbidden for commits in this repository.

## Documentation policy

Always keep these files updated when behavior changes:

- `README.md`
- `docs/INSTRUCTIONS.md`
- `docs/DOCUMENTATION.md`
- `docs/FACILITATION.md`
- `docs/adr/*.md`
- `to-do.md`

`docs/INSTRUCTIONS.md` is where the workshop instructions will be.

## DoD (mandatory)

A task is done only when all of the following are satisfied:

- Application compiles in Release.
- Required scripts run successfully.
- Migrations run successfully.
- All required `.http` requests run successfully.
- Acceptance, Integration, and Unit tests are green.
- Coverage is 100% for non-configuration code, or closest possible with explicit rationale.
- Mutation score is 100% for non-equivalent mutants.
- No generated artifacts (`bin`, `obj`, coverage reports, mutation logs/reports) are versioned.
- Documentation is complete, correct, and aligned with code.
- Workshop and developer experience remain clear and usable.
