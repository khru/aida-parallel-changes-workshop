# Documentation

## Objetivo

Este repositorio enseña parallel change en una API .NET con SQL Server, Dapper y FluentMigrator usando un ejercicio acotado para 90 minutos.

## Parallel change

Parallel change divide un cambio incompatible en estados intermedios seguros.

Reglas base:

- no eliminar una interfaz mientras exista un consumidor
- no forzar adopcion de una interfaz que todavia no existe

## Fases

### Expand

Se añade capacidad nueva sin retirar la capacidad anterior.

En este workshop:

- aparece `v2`
- se agregan columnas nuevas
- el sistema mantiene ambos caminos en paralelo

### Migrate

Se mueve el trafico y los datos del shape antiguo al shape nuevo de forma incremental.

En este workshop:

- backfill por lotes para filas historicas
- coexistencia de `v1` y `v2`
- lectura y escritura adaptadas al objetivo final

### Contract

Se elimina lo temporal cuando ya no hay dependencias.

En este workshop:

- se elimina `v1`
- se elimina persistencia legacy
- se simplifica el repositorio a un solo modelo

## Tecnicas usadas

- versionado de endpoint
- coexistencia de contratos
- migraciones aditivas
- backfill por lotes
- contraccion explicita

## Point of no easy return

El punto de no retorno facil aparece cuando el shape antiguo deja de recibir datos actuales. Antes de ese punto el rollback es simple. Despues, volver al shape antiguo puede devolver datos obsoletos.

## Implementacion .NET

- ASP.NET Core con controllers
- puertos y adaptadores para lectura y escritura
- value objects para nombre y telefono
- Dapper para SQL explicito
- FluentMigrator para evolucion de esquema
- OpenAPI para exponer contratos
- JSON:API como media type de intercambio

## Estrategia de ramas

La secuencia didactica se representa en ramas:

1. `workshop/initial-state`
2. `workshop/expand`
3. `workshop/migrate`
4. `workshop/contract`

Cada estado debe compilar, pasar tests y ser desplegable.
