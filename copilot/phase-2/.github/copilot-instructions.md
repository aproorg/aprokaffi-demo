# DemoTaskApi — GitHub Copilot instructions

This file provides guidance to GitHub Copilot when working with code in this repository.

## Project root and harness contract

Use the explicit absolute `project_root` supplied by the caller. If none is supplied,
accept the session working directory only when it contains its own
`.github\copilot-instructions.md`. Read that file explicitly. Do not search ancestors or
guess from the Git root. If `.github\.bootstrap-manifest.json` exists, confirm it belongs
to the selected project folder; this manually assembled phase does not require a bootstrap
marker.

`harness_root` is the absolute `project_root\.github` directory. Paths in these instructions
and file lists are project-root-relative. Use native OS path separators when constructing
paths. Run project commands at `project_root` unless a plan supplies another project-relative
working directory. Do not persist machine-specific absolute roots.

## Shared stage-result contract

Return `status: passed|blocked|failed`, `outputs`, `evidence`, and `blocker`. A passed result
requires actual work and evidence. Block on missing instructions, decisions, tools or other
prerequisites; report attempted work that did not meet its gate as failed. Preserve exact
relevant errors and partial results; do not fabricate evidence or hide partial changes.

## Workflow

Phase 2 has project instructions and test guidance plus a configured Jira MCP server. The
active session performs any user-directed work directly. This phase installs no agents or
skills; do not assume either is available. A configured server key is not proof of runtime
tool discovery or authentication.

## Project

.NET 9 minimal API for managing tasks and owners with in-memory services, with xUnit unit and endpoint tests.

## Security context

There is no authentication or persistent storage. The API accepts untrusted HTTP JSON input
and route IDs; endpoint input validation, error handling and API data exposure are the
principal relevant security concerns. This does not establish that repository or runtime
secrets are absent.

## Ticket source

Jira, through the configured `jira` MCP server. Runtime namespace, tool availability and
authentication remain unverified until actually checked.

## Commands

```text
dotnet build api-project\DemoTaskApi.sln
dotnet test api-project\DemoTaskApi.sln
dotnet test api-project\DemoTaskApi.sln --filter FullyQualifiedName~TaskServiceTests
dotnet run --project api-project\DemoTaskApi\DemoTaskApi.csproj
```

## Architecture

Program.cs is the composition root: it registers ITaskService/ITaskOwnerService implementations as singletons, then maps minimal HTTP endpoints directly to those services. Services maintain process-local lists and incrementing IDs; task creation validates an optional owner reference. Request records and storage models live under Models; owner endpoints map storage objects to TaskOwnerDto. Match the existing interface-backed service design, endpoint response conventions, nullable C# style, and tests around service behavior and HTTP responses. There is no database or persistence layer.

## Tests

Tests live in the nested DemoTaskApi.Tests project and use xUnit. Service tests instantiate service classes directly; endpoint tests use Microsoft.AspNetCore.Mvc.Testing's WebApplicationFactory<Program> and HttpClient. Test classes/methods follow descriptive behavior-oriented names. Keep both unit-level service coverage and endpoint-level integration coverage.
