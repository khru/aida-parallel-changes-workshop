# Instructions

## Business context

AIDA owns a live HTTP API with unknown consumers.

The current contract is flat:

- `contactName`
- `phone`
- `email`

The business now needs the contact to become structured because validation and future reuse are getting harder.

The desired shape introduces two concepts that are currently hidden inside strings:

- a personal name
- a phone number split into country code and local number

## Constraint

You cannot break existing consumers.

## Initial state

The system starts with:

- `GET /api/v1/customer-contacts/{customerId}`
- `PUT /api/v1/customer-contacts/{customerId}`
- one SQL Server table with legacy columns only

## Goal

Use parallel change to move from the legacy shape to the new structured shape through safe intermediate states.

## Definition of done for the initial state branch

- legacy `v1` contract works
- acceptance tests pass
- repository persists data in SQL Server
- migrations can create the initial table
