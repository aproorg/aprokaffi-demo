# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

All commands run from `api-project/` (relative to this file).

```bash
dotnet build DemoTaskApi.sln
dotnet test DemoTaskApi.sln
dotnet test DemoTaskApi.sln --filter "FullyQualifiedName~TaskEndpointTests"
dotnet test DemoTaskApi.sln --filter "FullyQualifiedName~TaskEndpointTests.PostTask_WithTitle_ReturnsCreated"
dotnet run --project DemoTaskApi
```

## Architecture

`DemoTaskApi` is a .NET 9 ASP.NET Core minimal API with no persistence layer — all state lives in memory for the lifetime of the process.

- **`Program.cs`** is the composition root: DI registration and every HTTP endpoint are defined here as top-level statements, not in separate controller classes. `public partial class Program { }` at the bottom exists solely so `WebApplicationFactory<Program>` in the test project can bootstrap the app in-process.
- **Services are singletons, not scoped** (`TaskService`, `TaskOwnerService`), each backing its data with a private `List<T>` and an auto-incrementing `_nextId`. This means test runs and app restarts do not share state, but concurrent requests within one run do — there's no locking around the lists.
- **Tasks and owners are related but only loosely validated**: `TaskItem.OwnerId` is a nullable FK-by-convention with no database-level enforcement. `POST /tasks` manually checks `owners.GetById(ownerId)` before accepting an `OwnerId`, since nothing else prevents a dangling reference.
- **Storage model vs. DTO split for owners only**: `TaskOwner` (internal) is mapped to `TaskOwnerDto` (public contract) via the `ToDto` helper in `Program.cs` before being returned from any endpoint. `TaskItem` has no equivalent DTO and is returned directly — keep that asymmetry in mind if extending either resource.

## Tests

`DemoTaskApi.Tests` uses `WebApplicationFactory<Program>` for full in-process HTTP integration tests (see `TaskEndpointTests.cs`, `OwnerEndpointTests.cs`) plus plain xUnit unit tests against the service classes directly (`TaskServiceTests.cs`, `TaskOwnerServiceTests.cs`). Shared fixture data lives under `MockData/`.
