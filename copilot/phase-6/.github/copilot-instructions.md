# DemoTaskApi — GitHub Copilot instructions

This file provides guidance to GitHub Copilot when working with code in this repository.

## Project root and harness contract

`project_root` is the actual bootstrapped project folder. Resolve it once at run start:
use the explicit absolute project folder supplied by the caller, or the session working
directory if it contains `.github/.bootstrap-manifest.json`. If neither identifies it,
ask for the project folder; do not guess from the Git root or search for an unrelated
ancestor's `.github`. Confirm the marker and this instructions file belong to that folder.

`harness_root` is the absolute `project_root/.github` directory. All local paths in these
instructions, plans and file lists are project-root-relative. Use native OS path
separators when constructing paths. Run project commands at `project_root` unless an
approved plan supplies a project-relative working directory. Do not persist machine-specific
absolute roots in generated templates or committed plans.

The coordinator passes absolute `project_root`, `harness_root`, and `artifact_root` to every
worker. Workers use those inputs, never independently re-derive roots from their cwd or the
Git root. Read `<harness_root>/copilot-instructions.md` explicitly before stage work; also
read `<harness_root>/instructions/tests.instructions.md` for planning/testing. Markdown
links and `#file` references can aid discovery but do not guarantee content injection. If
instructions are unreadable or roots disagree, return `status: blocked`, not a guess.

## Shared stage-result contract

Each worker is terminal: do its assigned stage and return to the same coordinator. Never
invoke another agent, an orchestrator, a built-in task agent, or an alias. Only the
coordinator fetches tickets. Stage payloads, not ambient ticket access, determine what each
worker receives.

Return `status: passed|blocked|failed`, `outputs`, `evidence`, and `blocker` for both normal
work and qualification. Passed requires actual stage output and evidence. Blocked means a
missing decision, plan detail, authentication, capability or other prerequisite; failed
means attempted work did not meet its gate. Preserve exact relevant errors; never fabricate
execution evidence or hide partial changes. Report missing result fields as blocked, not
implicit success.

Execution workers also report `skill_source: injected|read|unavailable` and the skill's
name/path, accurately describing how they obtained its full content. If unavailable, block.
An injected skill is usable; an explicit read is the reliable fallback. No mandatory claim
that it was preloaded.

For `mode: qualification`, do only the coordinator's artifact-only capability probe and
return its evidence; do not start feature work. Read-only workers read the supplied canary
and common instructions and demonstrate search with an actual tool call scoped to
`harness_root`; record path/query/result and errors. They never edit or execute. Normal
stage calls read their instructions and supplied stage inputs, not deleted canaries. This
proves only observed operations at that dispatch depth, not all future permissions.

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

## Template provenance

The shared source is the local workshop marketplace at `..\test-copilot-marketplace`,
relative to `project_root`. Its templates live under
`plugins\workshop-bootstrap\skills\bootstrap\assets\templates`.
Use that source for template proposals, not a remote marketplace or installed plugin cache.
The marketplace is a directory in this demo repository, not a separate Git checkout.
Proposals do not authorize edits, branches, commits or pull requests.

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
