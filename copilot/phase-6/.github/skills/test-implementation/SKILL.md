---
name: test-implementation
description: Analyzes code coverage and edge cases for a recently implemented feature, then writes and runs the tests needed to fully cover it, following this project's existing test conventions. Use this once a feature's implementation already exists and needs test coverage built against the real code, not just the plan.
---

## What this does

Takes a test plan plus the actual implemented code (not just the plan the code came from)
and closes any coverage gap between them. Reads the real implementation first, because a
plan written before the code existed can miss something the code actually does.

## This project's test conventions

Tests live in the nested DemoTaskApi.Tests project and use xUnit. Service tests instantiate
service classes directly; endpoint tests use Microsoft.AspNetCore.Mvc.Testing's
WebApplicationFactory<Program> and HttpClient. Test classes/methods follow descriptive
behavior-oriented names. Keep both unit-level service coverage and endpoint-level
integration coverage.

## Process

1. Read the test plan you were given. Use the supplied absolute `project_root` and
   `harness_root`, not a guessed Git root. Read
   `<harness_root>/copilot-instructions.md` and
   `<harness_root>/instructions/tests.instructions.md`; use project-root-relative paths.
2. Read the actual implementation this test plan is meant to cover — the real files changed
   by `feature-implementation`, not just the plan's description of them. Confirm the test
   plan's cases still match what was actually built. If behavior diverges, return a blocker
   to the coordinator instead of silently making the tests accept that divergence.
3. Look for edge cases the test plan itself might have missed by reading the real code:
   boundary conditions, null/missing-input handling, not-found paths — the same rigor you'd
   want in a real coverage review.
4. Write tests matching this project's conventions.
5. Run the required full suite at `project_root` (or the approved plan's project-relative
   cwd):
   ```bash
   dotnet test api-project\DemoTaskApi.sln
   ```
   Use the approved plan's exact command if the bootstrap command needs a documented update.
   If no usable command/tool exists, return `status: blocked`. Record command, absolute cwd,
   exit code and actual result. You may correct your own tests once within the approved
   plan, but never edit production code. An unresolved red suite is `status: failed`, not
   success.
6. If you find a real behavioral gap between the plan and the implementation that changes
   what should be tested, report it rather than silently testing around it. Production
   defects go through the coordinator to `implementation-agent`, plan gaps to
   `feature-planner`. Never delegate or fetch tickets yourself.

## Output

Return the common `status`, `outputs`, `evidence`, and `blocker` fields plus accurate
`skill_source: injected|read|unavailable`. List tests and cases covered (or why existing
tests suffice) and the required command/cwd/exit-code/result evidence. `status: passed`
requires exit code 0 and required coverage; a skill acknowledgement alone never suffices.
