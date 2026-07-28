# Pagination API Hotfix

Date: 2026-07-27

## Problem

Flutter server-side PlutoGrid pages expect Spring-style pagination metadata such as:

- `totalElements`
- `totalPages`
- `number`
- `size`
- `pageable.pageNumber`
- `pageable.pageSize`
- `pageable.offset`
- `first`
- `last`
- `numberOfElements`
- `empty`

A large number of legacy BLL methods returned `PageReturn<T>` with only `content`, `totalElements`, `size`, and an incorrect or incomplete `number`. As a result, the Flutter footer could read page zero repeatedly or receive `totalPages = 0`, making next/previous navigation appear non-functional.

## Changes

### `AskalePortal.Data/RequestParams/FilterPageParam.cs`

Added the non-generic `IPaginationRequest` contract. `FilterPageParam<T>` implements it without changing the existing lowercase JSON/form properties used by the application.

### `AskalePortal.Data/ResponseParams/PageReturn.cs`

Added the non-generic `IPaginationResult` contract and `NormalizePagination(...)`.

Normalization now consistently fills:

- zero-based current page,
- effective page size,
- total page count,
- page offset,
- first/last flags,
- current element count,
- empty flag,
- pageable and sort metadata.

`GetPage(...)` now counts the full query before `Skip/Take`, performs zero-based paging, and calls the same normalization routine.

### `AskalePortal/Infrastructure/Serialization/PaginationResultFilter.cs`

Added a global action filter. When an action receives an `IPaginationRequest` and returns an `IPaginationResult`, it normalizes the response metadata after the controller action has completed.

This fixes existing legacy pagination endpoints centrally instead of modifying dozens of BLL methods individually.

### `AskalePortal/Program.cs`

Registered `PaginationResultFilter` in dependency injection and added it to MVC controller filters before result serialization.

## Contract

API page numbering remains zero-based:

- first page: `page = 0`
- second page: `page = 1`

For a request with `page = 1`, `size = 10`, and `totalElements = 25`, the normalized response reports:

- `number = 1`
- `totalPages = 3`
- `pageable.pageNumber = 1`
- `pageable.pageSize = 10`
- `pageable.offset = 10`
- `first = false`
- `last = false`

## Validation

The legacy codebase was inspected for `PageReturn<T>` construction. The dominant pattern populated `content`, `totalElements`, `number`, and `size` but omitted the metadata required by the Flutter footer, which is why the global normalization filter is used.

Static syntax/balance checks were run for all modified C# files. The execution environment did not contain the .NET SDK, so `dotnet build` and automated tests could not be executed here. Run the normal solution CI/build before production deployment.
