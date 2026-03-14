# Instructions

## Purpose

This workshop simulates real-world change on a production-like API.

Treat every change as if unknown consumers were already depending on the current system.

The goal is not to produce a fast rewrite.
The goal is to evolve a live system carefully, keep it safe while it changes, and leave the repository in a state another team could trust.

## Product context

- HTTP API on .NET 10.
- The system is already in use.
- Compatibility cannot be broken carelessly.
- Data and contract evolution must be incremental and safe.

## Starting point

- Everyone must start from `main`.
- Each pair or group must create its own working branch from `main` before making changes.
- The branch name is up to the team.
- Everyone must begin from the same baseline.

## Current API scope

The current system exposes these endpoints:

- `GET /api/v1/customer-contacts/{customerId}`
- `POST /api/v1/customer-contacts`
- `PUT /api/v1/customer-contacts/{customerId}`

At the start of the workshop, customer contact data is exposed in a flat shape.

Example response:

```json
{
  "data": {
    "type": "customer-contacts",
    "id": "42",
    "attributes": {
      "contactName": "Maria Garcia",
      "phone": "+34 600123123",
      "email": "maria.garcia@example.com"
    }
  }
}
```

The current persistence model stores contact data in a flat form too.

Logical table shape:

* `customer_id`
* `contact_name`
* `phone`
* `email`

Your first responsibility is to understand the current system before changing it.

## Desired outcome

Product wants customer contact information to be represented in a structured way instead of a flat one.

The desired target shape is:

* `name.firstName`
* `name.lastName`
* `phone.countryCode`
* `phone.localNumber`
* `email`

Example target response:

```json
{
  "data": {
    "type": "customer-contacts",
    "id": "42",
    "attributes": {
      "name": {
        "firstName": "Maria",
        "lastName": "Garcia"
      },
      "phone": {
        "countryCode": "+34",
        "localNumber": "600123123"
      },
      "email": "maria.garcia@example.com"
    }
  }
}
```

This change affects:

* the HTTP contract
* the internal model
* the persistence model

How you reach that target while protecting the current system is part of the exercise.

## Work as in day-to-day engineering

* Do not code for the exercise only.
* Work as if this repository had to be maintained after the workshop.
* Keep production discipline: tests first, small steps, measurable quality gates.
* Refactor production and test code together.
* Keep documentation and executable HTTP docs updated in the same cycle.
* Do not leave the repository in a state that would confuse the next engineer.


## Commit policy (mandatory)

- Commits are required while working, not optional.
- Every commit must be small and focused on one intention.
- Commit messages must include phase and intent, not only what changed.
- Every message must include one phase marker:
  - `[expand]`
  - `[migrate]`
  - `[contract]`

Recommended message format:

`[phase] type: short change summary and rationale`

Example:

`[expand] test: add acceptance for create conflict to preserve legacy compatibility`

## OpenAPI is part of your daily workflow

Do not forget the OpenAPI contract.

* Check the OpenAPI document before and after contract-related changes.
* Use OpenAPI as a source of truth for endpoint behavior.
* Keep OpenAPI, acceptance tests, and `.http` files aligned.
* Treat drift between implementation, OpenAPI, and HTTP examples as a real defect.

## TDD workflow

Use outside-in double-loop TDD.

1. Start with an outer acceptance test.
2. Add inner unit or integration tests only when needed.
3. Implement the minimum code required for green.
4. Refactor production and test code together.
5. Reprioritize `to-do.md`.

Only one failing test at a time.

Do not write large batches of tests first.
Do not postpone refactoring until the end.
Do not commit a broken state.

## Allowed simplifications

This workshop is about disciplined change, not exhaustive modelling of the real world.

To keep the scope bounded, the following simplifications are allowed.

### Name splitting rule

You may split `contactName` using the first space:

* first token becomes `firstName`
* the remaining text becomes `lastName`

Examples:

* `Maria Garcia` becomes `Maria` and `Garcia`
* `Ana Lopez Torres` becomes `Ana` and `Lopez Torres`

### Phone splitting rule

You may split `phone` using the first space:

* first token becomes `countryCode`
* the remaining text becomes `localNumber`

Example:

* `+34 600123123` becomes `+34` and `600123123`

Be explicit and consistent.
Do not expand the problem beyond what the workshop is trying to teach.


## Expected deliverables

By the end of the workshop, your group should produce:

### Code

A working implementation that addresses the problem stated above.

### Tests

A test suite that gives confidence in the behavior you changed.

### Persistence changes

A database evolution path consistent with the requested target state.

### Documentation

Updated documentation and executable HTTP examples that stay aligned with the implementation.

### History of work

Small, intentional commits with meaningful commit messages.

## What success looks like

A successful outcome is not simply that a new shape exists.

A successful outcome is that:

* the problem is clearly understood
* the current system is respected
* the requested target state is addressed
* existing consumers are not carelessly broken
* the code remains readable
* the tests remain trustworthy
* the repository tells a coherent story of change

Another team should be able to read your solution and understand both what changed and why it changed that way.

## Common failure modes to avoid

Avoid these patterns:

* changing the current contract in place without considering existing consumers
* making a large atomic change with no safety net
* mixing transport, domain, and persistence concerns together
* introducing unnecessary complexity
* overusing test doubles
* keeping important concepts as raw strings when they clearly deserve stronger modelling
* writing too many tests at once
* performing large refactors without a reliable test safety net
* leaving the repository in a confusing state

## Working attitude expected during the session

This workshop should be approached with care.

* Be precise.
* Be explicit.
* Be methodical.
* Communicate your reasoning.
* Keep steps small.
* Keep the repository safe.

Do not rush past the problem statement.
Do not assume the obvious direct change is safe.
Do not optimise for speed at the cost of clarity.

## Validation commands

Run these regularly while working:

```bash
dotnet restore Aida.ParallelChange.sln
dotnet build Aida.ParallelChange.sln -c Release
./scripts/test.sh
dotnet test Aida.ParallelChange.sln -c Release --filter "TestCategory=NarrowIntegration"
./scripts/coverage.sh
./scripts/mutation.sh
./scripts/up.sh
./scripts/smoke.sh
./scripts/down.sh
```

## Final reminder

This exercise is about evolving a live system responsibly.

The challenge is not to produce a dramatic before-and-after transformation.
The challenge is to make progress without creating unnecessary risk.

Keep the system safe.
Keep the changes understandable.
Keep the work deliberate.

## WSL
```bash
dotnet restore Aida.ParallelChange.sln
dotnet build Aida.ParallelChange.sln -c Release
./scripts/test.sh
dotnet test Aida.ParallelChange.sln -c Release --filter "TestCategory=NarrowIntegration"
./scripts/coverage.sh
./scripts/mutation.sh
./scripts/up.sh
./scripts/smoke.sh
./scripts/down.sh
```

## Windows

```pwershell
dotnet restore Aida.ParallelChange.sln
dotnet build Aida.ParallelChange.sln -c Release
./scripts/test.ps1
dotnet test Aida.ParallelChange.sln -c Release --filter
./scripts/coverage.ps1
./scripts/mutation.ps1
./scripts/up.ps1
./scripts/smoke.ps1
./scripts/down.ps1
```