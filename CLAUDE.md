# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

.NET 8 solution (`NAS-Catalog-API.sln`). There are no test projects and no linter configured.

```bash
dotnet restore
dotnet build                                   # CI runs: dotnet build --no-restore --configuration Release
dotnet run --project Catalog.Api               # Swagger served via BuildingBlocks UseCustomSwagger
docker build -t nas-catalog-api .              # container listens on port 6004

# EF Core migrations (DbContext lives in Infrastructure, startup project is Api)
dotnet ef migrations add <Name> --project Catalog.Infrastructure --startup-project Catalog.Api
```

Migrations are applied automatically on startup (`app.InitialiseDatabaseAsync()` → `Database.MigrateAsync()`), unless `UseInMemoryDatabase` is true. Note `appsettings.Development.json` points at a remote PostgreSQL instance, so running the API locally (or adding a migration and starting the app) migrates that shared database.

CI: PRs to `main` only build. Pushes to `main` build, push `ghcr.io/<owner>/nas-catalog-api` and deploy over SSH to a GCP host (container `nas-catalog-api` on docker network `nas-net`, port 6004).

## Architecture

Clean Architecture + CQRS with MediatR. Dependency direction: `Catalog.Api` → `Catalog.Application` / `Catalog.Infrastructure` → `Catalog.Domain`, with shared plumbing in `BuildingBlocks/*` (shared across NAS microservices — changes there affect other services' copies).

- **Catalog.Domain** — EF entities (`Entities/`) and enums. No logic layer of its own.
- **Catalog.Application** — `Features/<Area>/{Commands,Queries}/<UseCase>/` each containing the `record` request, its handler (same file), and optional `*Validator.cs` (FluentValidation). DTOs in `Features/<Area>/Models/` declare their AutoMapper mapping in a nested `private class Mapping : Profile`. Handlers depend on `ICatalogDbContext` (`Common/Interfaces`) — not on the concrete DbContext — and return `ApiResponse` / `ApiResponse<T>` from `BuildingBlocks.Core.Response`. Throw `BuildingBlocks.Common.Exceptions` (e.g. `NotFoundException`) for error cases. MediatR pipeline: UnhandledException → Validation → Performance behaviours.
- **Catalog.Infrastructure** — `CatalogDbContext` (PostgreSQL via `AddCustomDbContext`, or InMemory), Fluent entity configs in `Persistence/Configurations/`, migrations, and Refit clients to other microservices (`IOrderClient`, `IIdentityClient`) whose base URLs come from `MicroserviceUri:*` config and which forward the caller's token via `AuthorizationMessageHandler`.
- **Catalog.Api** — thin controllers in `Controllers/V1/` inheriting `ApiControllerBase`, which applies JWT `[Authorize]`, the `PublicApi` rate-limit policy, and route `api/v{version:apiVersion}/[controller]` (from `BuildingBlocks.Common.API.Controllers.ApiController`). Actions just `Mediator.Send(...)`; create/update commands are typically bound `[FromForm]`. `Consumer/` holds MassTransit consumers (AWS SNS/SQS via `AwsBusOptions`; endpoint names come from `AwsBusOptions:Topic`) that dispatch into MediatR commands.

Adding a new entity typically touches: Domain entity → `ICatalogDbContext` + `CatalogDbContext` DbSet → `Persistence/Configurations/<X>Configuration.cs` → migration → Application feature folder → V1 controller.

## Domain notes

- Calendars support recurrence (`RecurrenceType`) with per-date exceptions stored as `CalendarOverride` rows (an override with `IsDeleted = true` cancels a single occurrence); edit/delete commands take a `CalendarScope` (`Single` vs `All`). `CalendarDayOfWeek` holds weekly recurrence days.
- Configuration: `appsettings.{Environment}.json` in `Catalog.Api`. `BuildingBlocks/Common/Configurations/sharedConfiguration*.json` exist but are not loaded — the `AppConfiguration.Configure` hook is commented out in `Program.cs`.
- Some folder/file names contain typos or spaces (`Features/Produts`, `Commands/UpdateCaledar`, `Entities/AppDeepLinks .cs`, `GetTechniciansWithFreeSlotQuery .cs`); namespaces follow those names, so match them rather than "fixing" them incidentally.
