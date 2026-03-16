# To-Do

## Context

- Goal: Keep a single-branch initial state in `main` with GET, POST, and PUT endpoints.
- Method: outside-in double-loop TDD with ZOMBIES and triangulation.
- Rule: one failing test at a time.
- Rule: accepted AC are immutable and cannot be weakened.
- Rule: no accepted AC can regress after refactor.
- Supersession policy: accepted AC can change only with explicit product decision, ADR evidence, and a traceable note in this file.
- Baseline reset completed on `main` from `Aida.ParallelChange.Workshop.clean`; parity drift is limited to runtime compatibility fixes in exception handling pipeline.
- Docker is available in this environment; Docker-dependent shell script validation is executable locally.
- PowerShell script execution is blocked in this environment because `pwsh`/`powershell` is unavailable.
- Mutation and coverage gates run on fast scope and were revalidated together with Docker-dependent shell operational scripts.
- Fast suite determinism gate passes in consecutive runs without retries (59/59 on each run).
- Application-layer solitary unit tests were removed after evidence showed no regression in fast tests, coverage, or mutation score.

## Supersession log

- SS-2026-03-14-001: `OP-REPLAY-001` retired by explicit product decision and documented in `docs/adr/ADR-005-retire-workshop-replay-scripts.md`. AC-14 coverage now validates operational scripts: up, down, migrate, smoke, test, coverage, mutation, verify, and shell-eol checks.

## Acceptance criteria and planned tests

| ID | Criterion | Planned tests | Status |
| --- | --- | --- | --- |
| AC-01 | API boots and responds | AT-HEALTH-200, AT-OPENAPI-200 | done |
| AC-02 | GET existing returns 200 | AT-GET-200-001, AP-GET-001, NI-GET-001 | done |
| AC-03 | GET missing returns 404 | AT-GET-404-001, AP-GET-002, NI-GET-002 | done |
| AC-04 | GET invalid id returns 400 | AT-GET-400-001, AP-GET-003 | done |
| AC-05 | POST valid returns 201 | AT-POST-201-001, AP-POST-001, NI-POST-001 | done |
| AC-06 | POST invalid payload returns 400 | AT-POST-400-001, DM-NAME-001, DM-EMAIL-001, DM-PHONE-001 | done |
| AC-07 | POST conflict returns 409 | AT-POST-409-001, AP-POST-002, NI-POST-002 | done |
| AC-08 | PUT existing returns 204 | AT-PUT-204-001, AP-PUT-001, NI-PUT-001 | done |
| AC-09 | PUT missing returns 404 | AT-PUT-404-001, AP-PUT-002, NI-PUT-002 | done |
| AC-10 | PUT invalid input returns 400 | AT-PUT-400-001, AP-PUT-003 | done |
| AC-11 | JSON:API media type consistency | AT-CONTRACT-001, AT-CONTRACT-002 | done |
| AC-12 | `.http` for all statuses | HTTP-GET-200/400/404, HTTP-POST-201/400/409, HTTP-PUT-204/400/404 | done |
| AC-13 | manual scenario create-get-update-get works | HTTP-SCENARIO-001 | done |
| AC-14 | required scripts run | OP-UP-001, OP-DOWN-001, OP-MIG-001, OP-SMOKE-001, OP-TEST-001, OP-COVERAGE-001, OP-MUTATION-001, OP-VERIFY-001, OP-EOL-001 | pending |
| AC-15 | all `.sh` files are LF-only | OP-EOL-001 | done |
| AC-16 | 100% line + branch coverage (non-config code) | Q-COV-001 | done |
| AC-17 | 100% non-equivalent mutants killed | Q-MUT-001 | done |
| AC-18 | no non-config production line without tests | Q-NO-UNT-001 | done |

## Solution branch acceptance criteria (branch `solution` only)

This section adds migration-focused criteria for the workshop resolution branch.
Baseline accepted AC above remain immutable and must not regress.

| ID | Criterion | Planned tests | Status |
| --- | --- | --- | --- |
| S-AC-01 | additive schema expansion preserves current runtime behavior | SI-MIG-EXP-001, AT-V1-NONREG-001 | done |
| S-AC-02 | deterministic split/compose policy is explicit and testable | SU-SPLIT-NAME-001, SU-SPLIT-PHONE-001, SU-COMPOSE-001 | done |
| S-AC-03 | repository create path performs dual-write for legacy and structured persistence | SI-DUAL-CREATE-001 | done |
| S-AC-04 | repository update path performs dual-write for legacy and structured persistence | SI-DUAL-UPDATE-001 | done |
| S-AC-05 | repository read path supports transitional mixed rows | SI-READ-TRANSITION-001 | done |
| S-AC-06 | deterministic backfill is idempotent and safe | SI-BACKFILL-001 | done |
| S-AC-07 | gradual completion process is idempotent for remaining rows | SI-COMPLETION-001 | done |
| S-AC-08 | `GET /api/v2/customer-contacts/{customerId}` returns structured contract and expected errors | AT-V2-GET-200-001, AT-V2-GET-400-001, AT-V2-GET-404-001 | pending |
| S-AC-09 | `POST /api/v2/customer-contacts` creates structured contact and preserves compatibility behavior | AT-V2-POST-201-001, AT-V2-POST-400-001, AT-V2-POST-409-001 | pending |
| S-AC-10 | `PUT /api/v2/customer-contacts/{customerId}` updates structured contact and preserves compatibility behavior | AT-V2-PUT-204-001, AT-V2-PUT-400-001, AT-V2-PUT-404-001 | pending |
| S-AC-11 | internal persistence contract removes legacy flat storage while preserving `v1` observable behavior | SI-CONTRACT-DB-001, AT-V1-NONREG-002 | pending |
| S-AC-12 | solution documentation explains full strategy, ordering, and evidence with executable diagrams | DOC-SOLUTION-001 | pending |

## Solution branch next red test (only one)

- [ ] AT-V2-GET-200-001

## ZOMBIES decomposition

### Zero

- TD-Z-001: GET existing customer returns expected document.
- TD-Z-002: POST valid customer creates resource.
- TD-Z-003: PUT valid customer updates resource.

### One

- TD-O-001: GET one missing customer returns 404.
- TD-O-002: POST one invalid payload returns 400.
- TD-O-003: PUT one missing customer returns 404.

### Many

- TD-M-001: multiple create requests do not corrupt retrieval.
- TD-M-002: multiple update requests keep last write.
- TD-M-003: repeated invalid IDs consistently return 400.

### Boundaries

- TD-B-001: customer id <= 0 is invalid.
- TD-B-002: blank contact name is invalid.
- TD-B-003: blank or too-long phone is invalid.
- TD-B-004: email without '@' is invalid.

### Interfaces

- TD-I-001: application handlers map port outcomes to API outcomes.
- TD-I-002: repository port contracts match SQL adapter behavior.

### Exceptions

- TD-E-001: create conflict maps to 409.
- TD-E-002: malformed route id maps to 400.

### Simplicity

- TD-S-001: remove accidental duplication in controller mapping.
- TD-S-002: prefer setup and parameterization before helper methods.

## Refactor-discovered tests

| ID | Trigger | Test to add | Status |
| --- | --- | --- | --- |
| RF-001 | Controller mapping duplication | AT-CONTRACT-001 | done |
| RF-002 | Status-specific error documents | AT-CONTRACT-002 | done |

## Mutation findings and kill tests

| Mutant ID | Location | Kill test | Equivalent | Status |
| --- | --- | --- | --- | --- |
| MUT-PENDING-001 | `scripts/mutation.sh` reaches 100% score with 54/54 tested mutants killed | Q-MUT-001 | no | done |

## Reprioritization gate

Run this section after every refactor and before next cycle:

1. Reorder pending work: non-equivalent mutants first.
2. Select one `Next Red Test`.
3. Record why this is the highest-value next step.
4. Commit before starting the next cycle.

## Next Red Test (only one)

- [ ] OP-UP-001
