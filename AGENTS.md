# AGENTS

## Purpose

Este archivo define como contribuir en este workshop de parallel change para AIDA.

## Product context

- API HTTP en .NET con consumidores no controlados
- no se permite romper compatibilidad en produccion
- el ejercicio enseña evolucion segura de contratos y datos

## Technical stack

- .NET 10
- ASP.NET Core controllers
- SQL Server
- Dapper
- FluentMigrator
- NUnit
- NSubstitute
- Shouldly y AwesomeAssertions
- OpenAPI
- JSON:API media type
- Docker y docker compose
- JetBrains HTTP Client CLI

## Branch workflow

Ramas del workshop:

1. `workshop/initial-state`
2. `workshop/expand`
3. `workshop/migrate`
4. `workshop/contract`

Flujo para alumnado:

```bash
git checkout workshop/initial-state
git checkout -b team-x-solution
```

## Development rules

- codigo sin comentarios
- nombres que revelan intencion
- separacion de responsabilidades
- SOLID
- inversion de dependencias e inversion de control
- encapsulacion e inmutabilidad cuando aplica
- evitar switch statements
- evitar magic strings y magic numbers
- evitar Entity Framework en este repositorio

## Testing rules

- TDD con ciclos pequenos
- un test rojo por vez
- minima implementacion para verde
- refactor inmediato
- prioridad a tests sociables desde endpoint hacia abajo
- usar dobles de test solo cuando reduzcan acoplamiento tecnico

## TDD guided by zombies and triangulation

Secuencia recomendada:

1. fake it hasta obtener primer verde
2. obvio cuando el comportamiento es directo
3. triangulacion al aparecer un segundo ejemplo que fuerce generalizacion

Cada nuevo caso se introduce cuando aporta una razon nueva para fallar.

## Quality gates before commit

```bash
dotnet restore Aida.ParallelChange.sln
dotnet build Aida.ParallelChange.sln -c Release
dotnet test Aida.ParallelChange.sln -c Release
```

Para smoke local con contenedores:

```bash
./scripts/up.sh
./scripts/migrate.sh
./scripts/smoke.sh
```

## Commit policy

- commits pequenos y con intencion clara
- no mezclar cambios de fases distintas
- cada commit debe dejar el repo en verde
- la rama `contract` nunca conserva codigo transicional sin uso

## Documentation policy

Mantener actualizados:

- `README.md`
- `INSTRUCTIONS.md`
- `DOCUMENTATION.md`
- `FACILITATION.md`
- `adr/ADR-00X.md`

Cuando cambie una decision de arquitectura, crear un ADR nuevo en lugar de sobrescribir el anterior.
