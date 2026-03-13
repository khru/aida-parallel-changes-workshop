# Instructions

## Contexto de negocio

AIDA mantiene una API HTTP en produccion con consumidores desconocidos.

El contrato actual expone un contacto plano:

- `contactName`
- `phone`
- `email`

El negocio necesita evolucionar a un contrato estructurado para mejorar validacion, reutilizacion y mantenibilidad sin romper compatibilidad.

## Restriccion principal

No se permiten breaking changes para consumidores existentes.

## Flujo del workshop

1. Todo el alumnado parte de `workshop/initial-state`.
2. Cada grupo crea su propia rama de trabajo.
3. La facilitacion avanza por `workshop/expand`, `workshop/migrate` y `workshop/contract`.

## Inicio para participantes

```bash
git checkout workshop/initial-state
git checkout -b team-1-solution
```

No se trabaja directamente sobre `workshop/expand`, `workshop/migrate` o `workshop/contract`.

## Objetivo tecnico

Aplicar parallel change sobre una API .NET con SQL Server para pasar de un contrato legacy a uno estructurado mediante estados intermedios seguros.

## Definicion de hecho por fase

### Initial state

- `GET /api/v1/customer-contacts/{customerId}` operativo
- `PUT /api/v1/customer-contacts/{customerId}` operativo
- esquema inicial en SQL Server
- tests de aceptacion de `v1` en verde

### Expand

- `v2` disponible sin eliminar `v1`
- nuevas columnas de persistencia creadas
- dual write activo
- OpenAPI versionado con `v1` y `v2`

### Migrate

- backfill por lotes ejecutable
- lectura de `v2` desde shape nuevo
- escritura `v2` operativa
- coexistencia de ambos contratos

### Contract

- eliminacion de `v1`
- eliminacion de columnas legacy
- eliminacion de queries legacy
- solo queda un camino funcional

## Comandos de verificacion

```bash
dotnet restore Aida.ParallelChange.sln
dotnet build Aida.ParallelChange.sln -c Release
dotnet test Aida.ParallelChange.sln -c Release
```
