---
name: bootstrap
description: Analyzes an empty project (no existing Claude Code setup) and generates a complete ticket-to-verified-feature workflow — CLAUDE.md plus a full set of skills and agents (planning, mechanical implementation, mechanical test-writing, verification, and retrospective) — scoped to that project's actual stack, conventions, and tooling. Use this whenever a fresh project needs the feature-orchestrator pipeline bootstrapped from scratch, rather than writing each piece by hand. Do not use this on a project that already has a `.claude/` setup — that needs a merge/sync decision, not a fresh generation.
---

## What this does

Turns the stack-agnostic templates bundled with this skill (`assets/templates/`, described
by `assets/templates/template-manifest.json`) into a working, project-scoped Claude Code
setup by actually reading the target project first. No generic boilerplate, no invented
conventions — every `{{template-var}}` in the bundled templates gets filled in from real
analysis of the project it's being generated for, or flagged as an assumption if it
genuinely can't be determined.

## Before writing anything

1. **Refuse to run on a project that already has `.claude/` or a root `CLAUDE.md`.** That
   needs a deliberate merge/sync decision by a human, not a fresh overwrite. Stop and say
   so.

2. **Detect the stack.** Look for manifest files (`package.json`, `*.csproj`/`*.sln`,
   `pyproject.toml`/`requirements.txt`, `go.mod`, `Gemfile`, etc.) and existing source
   layout. If nothing conclusive is found, ask rather than guess — a wrong stack guess
   poisons every other variable downstream of it.

3. **Determine the variables** listed in `template-manifest.json`, by actually looking:
   - `project_name`, `project_description` — from the manifest file or repo name, plus a
     one-line read of what the project does.
   - `build_command`, `test_command`, `test_filter_example`, `run_command` — from the
     detected stack's own manifest/scripts, not generic guesses. Leave a command empty
     (not fabricated) if the stack genuinely doesn't have that concept.
   - `architecture_notes` — read the actual existing source code (if any exists beyond
     scaffolding) and write real prose about its composition root/entry point, data flow,
     and conventions a contributor would need to match. If the project is truly empty
     (no source yet), say so plainly rather than inventing an architecture that doesn't
     exist yet.
   - `test_conventions` — read existing tests if any exist. If none exist yet, propose the
     most idiomatic default test setup for the detected stack, and label it explicitly as
     a **proposed default**, not a discovered fact.
   - `security_context_description` — note briefly whether the project has auth,
     persistence, or handles untrusted external input, from what's actually there.
   - `ticket_source_description` — check this session's connected MCP servers for anything
     issue-tracker shaped (Jira/Atlassian, GitHub, Linear, Asana, etc.). If none is
     connected, ask the user which tracker to wire up rather than inventing one.
   - `template_repo_location` — where this template set itself lives, so the generated
     `retro-agent` can correctly reference it later.

4. **Show a generation plan** before writing anything: the detected stack, every variable
   value you intend to use (marking proposed defaults and assumptions clearly, separate
   from actually-discovered facts), and the exact list of files you'll create. Wait for
   approval. This mirrors every other "ask before acting" step already built into the
   pipeline this skill generates — bootstrapping the workflow shouldn't hold itself to a
   lower bar than the workflow it produces.

## Generate, after approval

For every file listed in `template-manifest.json`'s `generatedFileMap`, substitute each
`{{variable}}` occurrence in the `.tmpl` source with the value determined above, and write
the result to the corresponding path in the target project (the map's values are relative
to the project root, e.g. `.claude/agents/feature-planner.md`).

Then write `.claude/.bootstrap-manifest.json` in the target project recording: which
template set and version this came from, the variable values used, and the generated file
list — so a future update can tell generated files apart from anything the project team
edits by hand afterward.

## Verify

After generation:

1. Confirm `CLAUDE.md` and every file in the generated file map now exist.
2. If the project already has a runnable build/test command, run it and report the result
   honestly — a freshly bootstrapped project with no feature implemented yet may have
   nothing to test, and that's fine to report as-is rather than treated as a failure.
3. Report: files created, the variable values used (flagging proposed defaults separately
   from discovered facts), and anything left as an open question for the human — most
   commonly, which issue tracker to connect if none was already configured.

## Important

Do not guess a variable value you can't actually determine — derive it from real analysis
or flag it as an assumption needing confirmation, the same discipline every other skill in
this pipeline is held to. Do not modify any of the target project's existing source files;
this skill only ever adds `CLAUDE.md` and `.claude/`.
