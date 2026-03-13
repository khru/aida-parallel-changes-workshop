# AIDA Parallel Change Workshop

This repository is a hands-on workshop to practice safe contract and data evolution in a .NET 10 HTTP API with unknown consumers.

The workshop follows a strict parallel change journey:

1. `workshop/initial-state`
2. `workshop/expand`
3. `workshop/migrate`
4. `workshop/contract`

This README belongs to `main`.

## What participants build

The initial contract is a legacy flat JSON:API payload for customer contact data:

- `contactName`
- `phone`
- `email`

The final target is a structured contract introduced safely in later branches.

## Workshop branch setup

```bash
git checkout workshop/initial-state
git checkout -b team-x-solution
```

`main` keeps the shared baseline tooling and documentation used by every workshop branch.

## Runtime commands

Linux or macOS:

```bash
./scripts/up.sh
./scripts/smoke.sh
./scripts/down.sh
```

Windows PowerShell:

```powershell
pwsh ./scripts/up.ps1
pwsh ./scripts/smoke.ps1
pwsh ./scripts/down.ps1
```

## Quality commands

```bash
./scripts/test.sh
./scripts/verify.sh
```

## Branch navigation

```bash
./workshop-branch.sh list
./workshop-branch.sh goto expand
./workshop-branch.sh next
```

```powershell
./workshop-branch.ps1 list
./workshop-branch.ps1 goto migrate
./workshop-branch.ps1 next
```

## Additional automation scripts

- `scripts/test-watch.sh` and `scripts/test-watch.ps1`
- `scripts/god-mode.sh` and `scripts/god-mode.ps1`
- `scripts/workshop-replay.sh` and `scripts/workshop-replay.ps1`
- `scripts/verify-history.sh` and `scripts/verify-history.ps1`
- `scripts/clean-docker.sh` and `scripts/clean-docker.ps1`

## HTTP requests used as executable docs

- `http/v1/get-customer-contact.http`
- `http/v1/update-customer-contact.http`
- `http/environments/local.http-client.env.json`

## Documentation map

- `docs/INSTRUCTIONS.md`
- `docs/DOCUMENTATION.md`
- `docs/FACILITATION.md`
- `docs/adr/ADR-001.md`
- `docs/adr/ADR-002.md`
- `docs/adr/ADR-003.md`
- `AGENTS.md`

## Architecture Overview

```mermaid
flowchart LR
  Consumer[External Consumer] --> Api[ASP.NET Core API v1]
  Api --> App[Application Handlers]
  App --> PortRead[CustomerContactReader]
  App --> PortWrite[CustomerContactWriter]
  PortRead --> Repo[SQL Server Repository]
  PortWrite --> Repo
  Repo --> Sql[(SQL Server)]
  Migrator[FluentMigrator Runner] --> Sql
```

## Use Cases

```mermaid
flowchart TD
  Facilitator --> Demo[Demonstrate safe evolution path]
  Participant --> GetV1[Read customer contact via v1]
  Participant --> PutV1[Update customer contact via v1]
  Participant --> RunQuality[Run tests and smoke checks]
  RunQuality --> Confidence[Preserve compatibility confidence]
```

## C4 Level 1

```mermaid
flowchart LR
  User[API Consumer] --> System[AIDA Parallel Change Workshop System]
  Facilitator[Workshop Facilitator] --> System
```

## C4 Level 2

```mermaid
flowchart LR
  User[API Consumer] --> ApiContainer[API Container]
  Facilitator[Facilitator] --> Scripts[Automation Scripts]
  Scripts --> ApiContainer
  ApiContainer --> SqlContainer[SQL Server Container]
  MigratorContainer[Migrator Container] --> SqlContainer
```

## C4 Level 3

```mermaid
flowchart TB
  Controller[V1 Controller] --> GetHandler[GetCustomerContactHandler]
  Controller --> UpdateHandler[UpdateCustomerContactHandler]
  GetHandler --> ReaderPort[CustomerContactReader]
  UpdateHandler --> WriterPort[CustomerContactWriter]
  ReaderPort --> SqlRepo[SqlServerCustomerContactRepository]
  WriterPort --> SqlRepo
  SqlRepo --> Sql[(SQL Server)]
```

## C4 Level 4

```mermaid
flowchart LR
  GetEndpoint[GET /api/v1/customer-contacts/{id}] --> GetQuery[GetCustomerContactQuery]
  PutEndpoint[PUT /api/v1/customer-contacts/{id}] --> UpdateCommand[UpdateCustomerContactCommand]
  GetQuery --> Domain[CustomerContact Domain Model]
  UpdateCommand --> Domain
  Domain --> SqlMapper[SQL Mapper]
  SqlMapper --> Sql[(customer_contacts)]
```

## Endpoint Sequences

### GET v1

```mermaid
sequenceDiagram
  participant C as Client
  participant A as API v1 Controller
  participant H as Get Handler
  participant R as SQL Repository
  participant D as SQL Server
  C->>A: GET /api/v1/customer-contacts/42
  A->>H: Handle query
  H->>R: Read customer contact
  R->>D: SELECT legacy columns
  D-->>R: Row
  R-->>H: Domain contact
  H-->>A: Domain contact
  A-->>C: 200 JSON:API v1 document
```

### PUT v1

```mermaid
sequenceDiagram
  participant C as Client
  participant A as API v1 Controller
  participant H as Update Handler
  participant R as SQL Repository
  participant D as SQL Server
  C->>A: PUT /api/v1/customer-contacts/42
  A->>H: Handle command
  H->>R: Upsert customer contact
  R->>D: UPDATE legacy columns
  D-->>R: Ack
  R-->>H: Done
  H-->>A: Done
  A-->>C: 204 No Content
```

## Validation baseline

```bash
dotnet restore Aida.ParallelChange.sln
dotnet build Aida.ParallelChange.sln -c Release
dotnet test Aida.ParallelChange.sln -c Release
./scripts/up.sh
./scripts/smoke.sh
./scripts/down.sh
```
