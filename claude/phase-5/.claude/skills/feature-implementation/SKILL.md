---
name: feature-implementation
description: Executes a precise implementation plan for the DemoTaskApi project's service and API layers only — no tests. Use this when you have been handed a concrete implementation plan (from feature-planner or equivalent) and need to turn it into working code, following this project's existing minimal-API and in-memory-service conventions exactly.
---

## What this does

Turns an already-decided implementation plan into code. This skill is deliberately narrow:
service layer and `Program.cs` API layer only. It does not write, run, or think about
tests — that's `test-implementation`'s job, and it runs afterward against the real code
this skill produces.

## Process

1. Read the implementation plan you were given in full before touching anything.
2. Read this project's `CLAUDE.md` and the existing service/endpoint code it's built from,
   so the new code matches the same shape: an `IXService`/`XService` pair for anything new,
   endpoints added as top-level statements in `Program.cs`, the same null-handling and
   status-code conventions already in use.
3. Implement exactly what the plan says — including its edge-case decisions — without
   re-deciding anything the plan already settled. This skill is meant to be followed
   precisely, not second-guessed.
4. If the plan is missing something you need to proceed (a detail it should have specified
   but didn't), stop and report the gap rather than guessing past it. A well-formed plan
   shouldn't have gaps; if you hit one, that's worth surfacing, not papering over.
5. Do not add tests, and do not add anything the plan didn't ask for — no auth, no
   persistence, no unrelated cleanup, no "while I'm here" improvements.

## Output

Report the files changed and a one-line summary of what changed in each, so the calling
agent can hand this forward to the test-writing step.
