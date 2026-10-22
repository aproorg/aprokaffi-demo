# DemoTaskApi — GitHub Copilot instructions

This file provides guidance to GitHub Copilot when working with code in this repository.

## Project root and harness contract

Use the explicit absolute `project_root` supplied by the caller. If none is supplied,
accept the selected session working directory only when it contains its own
`.github\copilot-instructions.md`. Read that file explicitly; this manually assembled
pipeline does not require a bootstrap marker. If `.github\.bootstrap-manifest.json` exists,
confirm that it belongs to the selected project folder. Do not search ancestors or guess
from the Git root.

`harness_root` is the absolute `project_root\.github` directory. All local paths in these
instructions, plans and file lists are project-root-relative. Use native OS path separators
when constructing paths. Run project commands at `project_root` unless an approved plan
supplies another project-relative working directory. Do not persist machine-specific roots.
The coordinator passes absolute `project_root`, `harness_root` and `artifact_root` to every
worker; workers use those inputs and never re-derive roots from cwd or the Git root.
Explicitly read `<harness_root>\copilot-instructions.md`; also read
`<harness_root>\instructions\tests.instructions.md` for planning/testing.

## Shared stage-result contract

Every stage returns `status: passed|blocked|failed`, `outputs`, `evidence` and `blocker`.
Execution workers also report `skill_source: injected|read|unavailable` and the skill path.
Passed requires actual work and evidence. Blocked means a missing decision, plan detail,
authentication, capability or other prerequisite; failed means attempted work did not
meet its gate. Preserve exact relevant errors and partial results; never fabricate evidence
or hide partial changes.

## Qualification and workflow boundary

Qualification is artifact-only and does not start feature work. Follow the coordinator's
bounded canary protocol; do not touch production or test files during qualification.
Normal stage calls use their supplied stage inputs, not deleted canaries. A skill supplies
same-context instructions; it never authorizes delegation.

## Project

.NET 9 minimal API for managing tasks and owners with in-memory services, with xUnit unit and endpoint tests.

## Security context

There is no authentication or persistent storage. The API accepts untrusted HTTP JSON input
and route IDs; endpoint input validation, error handling and API data exposure are the
principal relevant security concerns. This does not establish that repository or runtime
secrets are absent.

## Ticket source

Jira, through the configured `jira` MCP server. Runtime namespace, tool availability and
authentication remain unverified until actually checked. Use only the `jira/*` ticket
namespace.

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
