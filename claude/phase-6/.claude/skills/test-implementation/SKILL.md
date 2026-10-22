---
name: test-implementation
description: Analyzes code coverage and edge cases for a recently implemented feature in DemoTaskApi, then writes and runs the tests needed to fully cover it, following this project's existing service-level and HTTP-endpoint test patterns. Use this once a feature's implementation already exists and needs test coverage built against the real code, not just the plan.
---

## What this does

Takes a test plan plus the actual implemented code (not just the plan the code came from)
and closes any coverage gap between them. Reads the real implementation first, because a
plan written before the code existed can miss something the code actually does.

## Process

1. Read the test plan you were given.
2. Read the actual implementation this test plan is meant to cover — the real files changed
   by `feature-implementation`, not just the plan's description of them. Confirm the test
   plan's cases still match what was actually built; if the implementation diverged from
   the plan in some way, cover what's actually there.
3. Look for edge cases the test plan itself might have missed by reading the real code:
   boundary conditions, null/missing-input handling, not-found paths — the same rigor
   you'd want in a real coverage review.
4. Write tests at both levels used in this repo: a service-level xUnit test and a
   `WebApplicationFactory`-based HTTP endpoint test, matching the structure of the existing
   tests in `DemoTaskApi.Tests`.
5. Run the full suite:
   ```bash
   dotnet test api-project/DemoTaskApi.sln
   ```
   Fix any failure before finishing — don't hand back a red suite.
6. If you find a real behavioral gap between the plan and the implementation that changes
   what should be tested, report it rather than silently testing around it.

## Output

Report the tests added, which cases they cover, and the full suite's pass/fail result.
