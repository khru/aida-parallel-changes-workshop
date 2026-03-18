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

## Local execution plan policy (`plan.md`)

- `plan.md` is the mandatory local execution plan while work is in progress.
- `plan.md` must be complete, rigorous, coherent, cohesive, and detailed enough to execute without ambiguity.
- `plan.md` must be updated continuously as tasks complete, fail, or are blocked.
- If a new approved plan is different from the current one, rewrite `plan.md` completely with the new plan.
- `plan.md` is local-only and must never be versioned.
- Verification scripts must fail if `plan.md` is tracked by git.

## Design and coding rules

- No comments in executable code.
- Names must reveal intention.
- Enforce separation of concerns.
- Enforce SOLID.
- Enforce dependency inversion and inversion of control.
- Avoid use of magic numbers and magic strings
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
- Tests that assert infrastructure primitive state as a primary behavior claim (for example checking `SqlConnection.State` instead of repository-observable outcomes).

Security and configuration test rules:

- Do not hardcode credentials in test fixtures when an environment-based source is available.
- Prefer environment variables for testcontainer secrets and runtime-sensitive values.

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
- `scripts/check-shell-eol.*`

For shell scripts:

- `*.sh` are LF-only.
- CRLF in `*.sh` is a hard failure.

## GitHub Actions local validation rules

- Workflows under `.github/workflows/*.yml` must be executable locally with `https://github.com/nektos/act`.
- Docker access is a prerequisite for local `act` validation.
- Workflow design must support fractional execution to speed up feedback (for example by job selection and/or explicit scope input).
- Local workflow validation commands must be documented when CI workflows are introduced or changed.

## Executable HTTP documentation rules

- Keep `.http` coverage for all endpoint outcomes.
- Include `.http` for health and OpenAPI.
- Keep `.http` aligned with acceptance tests and docs.

## Commit policy

- Commits are mandatory while working.
- Commits must be small and single-intention.
- Commit messages must explain intent clearly.
- Phase markers (`[expand]`, `[migrate]`, `[contract]`) are workshop markers for student exercise commits, not a global requirement for repository maintenance commits.

## Commit identity policy

All commits must use this identity configuration:

- GitHub handle: `evalverde-eng`
- Author: `Emmanuel Valverde Ramos <evalverde@domingoalonsogroup.com>`
- Committer: `Emmanuel Valverde Ramos <evalverde@domingoalonsogroup.com>`

The identity `OpenAI <openai@example.com>` is forbidden for commits in this repository.

## Planning policy

- Do not assume missing user intent when preparing plans; resolve ambiguity explicitly before execution.
- Every generated plan must include these two questions with scores:
  - `How certain is this plan to achieve 100% of what the user wants? (score)`
  - `If someone read this plan for the first time, could they execute 100% of it without mistakes? (score)`
- The maximum score for each question is `1`.
- If either score is lower than `1`, the plan must explain:
  - what is still weak
  - why that weakness remains
  - what must be improved to raise both scores to `1`
- Plans must be complete, rigorous, coherent, cohesive, non-breaking, and explicit enough to execute without interpretation.

## Documentation policy

- Do not modify `docs/INSTRUCTIONS.md`, `docs/DOCUMENTATION.md`, or `docs/FACILITATION.md` unless explicitly requested by the user.
- Keep `to-do.md` aligned with accepted AC and current execution decisions.
- Update `docs/adr/*.md` only when an explicit architecture decision must be recorded.

## DoD (mandatory)

A task is done only when all of the following are satisfied:

- Application compiles in Release.
- Required scripts run successfully.
- Migrations run successfully.
- All required `.http` requests run successfully.
- Acceptance, Integration, and Unit tests are green.
- Fast test suite is deterministic and passes in consecutive runs without retries.
- Coverage is 100% for non-configuration code, or closest possible with explicit rationale.
- Mutation score is 100% for non-equivalent mutants.
- GitHub Actions workflows for build, tests, and mutation run on `pull_request` and `main`, and are validated locally with `act` when Docker access is available.
- CI pipeline can be fractioned for targeted execution (for example by scope input or selected jobs).
- No generated artifacts (`bin`, `obj`, coverage reports, mutation logs/reports) are versioned.
- Workshop and developer experience remain clear and usable.
