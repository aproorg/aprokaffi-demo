---
name: feature-implementation
description: Executes a precise implementation plan for this project's production code — no tests. Use this when you have been handed a concrete implementation plan from feature-planner and need to turn it into working code, following this project's existing conventions exactly.
---

## What this does

Turns an already-decided implementation plan into code. This skill is deliberately narrow:
production code only, whatever layers the plan touches. It does not write, run, or think
about tests — that's `test-implementation`'s job, and it runs afterward against the real
code this skill produces.

## Process

1. Read the implementation plan you were given in full before touching anything.
2. Use the supplied absolute `project_root` and `harness_root`; all plan paths are
   project-root-relative, not Git-root-relative. Read `<harness_root>/copilot-instructions.md`
   and the existing code it describes, so the new code matches the same shape and
   conventions — don't introduce a pattern the codebase doesn't already use.
3. Implement exactly what the plan says — including its edge-case decisions — without
   re-deciding anything the plan already settled. This skill is meant to be followed
   precisely, not second-guessed.
4. If the plan is missing something you need to proceed (a detail it should have specified
   but didn't), stop and report the gap rather than guessing past it. A well-formed plan
   shouldn't have gaps; if you hit one, that's worth surfacing, not papering over it.
5. Do not write tests or make changes the plan does not ask for — no unrelated cleanup,
   no "while I'm here" improvements, no capability the plan didn't call for.

## Output

Return the common `status`, `outputs`, `evidence`, and `blocker` fields plus accurate
`skill_source: injected|read|unavailable`. List project-relative files changed with
one-line summaries and the plan steps they satisfy, with actual edit/read-back evidence.
Required changes must exist before `status: passed`; otherwise block unless an explicit
approved no-op decision is supplied. Never delegate, fetch tickets, write tests or
substitute another skill. Keep partial changes visible on failure.
