---
applyTo: "**/*.Tests/**"
---

# Test instructions

Paths are relative to the supplied `project_root`, which can be below the Git root. Use
the supplied absolute `harness_root` to locate instructions, not the worker's cwd.

Tests live in the nested DemoTaskApi.Tests project and use xUnit. Service tests instantiate
service classes directly; endpoint tests use Microsoft.AspNetCore.Mvc.Testing's
WebApplicationFactory<Program> and HttpClient. Test classes/methods follow descriptive
behavior-oriented names. Keep both unit-level service coverage and endpoint-level
integration coverage.

Run the full suite at `project_root` with
`dotnet test api-project\DemoTaskApi.sln`, or scope it with
`dotnet test api-project\DemoTaskApi.sln --filter FullyQualifiedName~TaskServiceTests`.
A scoped run does not replace the required full-suite gate. Record the actual command,
cwd, exit code and result. A missing command blocks.
