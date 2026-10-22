---
name: bootstrap
description: Analyzes an empty project (no existing GitHub Copilot setup) and generates a complete ticket-to-verified-feature workflow — .github/copilot-instructions.md plus custom agents and skills (planning, mechanical implementation, mechanical test-writing, verification, and retrospective) — scoped to that project's actual stack, conventions, and tooling. Use this whenever a fresh project needs the feature-orchestrator pipeline bootstrapped from scratch, rather than writing each piece by hand. Do not use this on a project that already has `.github/copilot-instructions.md`, `.github/agents/`, or `.github/skills/` — that needs a merge/sync decision, not a fresh generation.
---

## What this does

Turns the stack-agnostic templates bundled with this skill (`assets/templates/`, described
by `assets/templates/template-manifest.json`) into a working, project-scoped Copilot setup by
actually reading the target project first. No generic boilerplate, no invented conventions —
every `{{template-var}}` in the bundled templates gets filled in from real analysis of the
project it's being generated for, or flagged as an assumption if it genuinely can't be
determined.

## Local workshop source

The source for this workshop is `copilot\test-copilot-marketplace` in the demo repository.
Its templates live under `plugins\workshop-bootstrap\skills\bootstrap\assets\templates`.
Resolve that local marketplace from the caller's supplied source or selected workshop
folder; ask if it is unavailable. Do not substitute an upstream GitHub repository or
assume an installed plugin cache is the editable source.

For generated projects, record a portable marketplace location relative to `project_root`
plus the template subdirectory. In phase 6 this is `..\test-copilot-marketplace`.
Derive the relative location for other targets; ask if no portable location can be
established. The marketplace is part of the demo repository, not a separate Git checkout.

## Before writing anything

1. **Resolve the target roots before discovery.** `project_root` is the absolute folder
   the user is bootstrapping, not necessarily the Git root. Use their selected folder or
   this session's cwd when unambiguous; ask if multiple projects could be intended.
   `harness_root` is `project_root/.github`, constructed with native OS path separators.
   Resolve both once and scope all discovery, commands and generated paths to that project.
   A project such as `<Git root>/copilot/phase-1` owns its own `.github`; never substitute
   the Git root's `.github`. Do not embed machine-specific absolute roots in reusable files.

2. **Refuse to run on a project that already has Copilot setup.** If `.github/copilot-instructions.md`,
   `.github/agents/`, or `.github/skills/` already exist, stop and say so. That needs a
   deliberate merge/sync decision by a human, not a fresh overwrite. Check these paths
   under `project_root`; do not overwrite a parent project's setup either.

3. **Detect the stack.** Look for manifest files (`package.json`, `*.csproj`/`*.sln`,
   `pyproject.toml`/`requirements.txt`, `go.mod`, `Gemfile`, etc.) and existing source
   layout. If nothing conclusive is found, ask rather than guess — a wrong stack guess
   poisons every other variable downstream of it.

4. **Determine the variables** listed in `template-manifest.json`, by actually looking:
   - `project_name`, `project_description` — from the manifest file or repo name, plus a
     one-line read of what the project does.
   - `build_command`, `test_command`, `test_filter_example`, `run_command`, `test_path_glob` —
     from the detected stack's own manifest/scripts, not generic guesses. Leave a command
     empty (not fabricated) if the stack genuinely doesn't have that concept.
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
   - `ticket_tools` — the Copilot tool namespace of that same MCP server (e.g. `'jira/*'`). Only `feature-orchestrator` uses it.
   - `template_repo_location` — the verified local marketplace location relative to the
     target project, plus `plugins\workshop-bootstrap\skills\bootstrap\assets\templates`,
     so `retro-agent` can point proposals at this workshop's source. A local directory
     does not imply a separate Git checkout or permission to modify it.

5. **Show a generation plan** before writing anything: the absolute roots, detected stack, every variable
   value you intend to use (marking proposed defaults and assumptions clearly, separate
   from actually-discovered facts), and the exact list of files you'll create. Wait for
   approval. This mirrors every other "ask before acting" step already built into the
   pipeline this skill generates — bootstrapping the workflow shouldn't hold itself to a
   lower bar than the workflow it produces.

## Generate, after approval

For every file listed in `template-manifest.json`'s `generatedFileMap`, substitute each
`{{variable}}` occurrence in the `.tmpl` source with the value determined above, and write
the result to the corresponding path in the target project (the map's values are relative
to `project_root`, e.g. `.github/agents/feature-planner.agent.md`). Do not resolve map
values against the Git root. Preserve valid test globs such as `**/*.Tests/**`.
Serialize substituted values safely for their context, including YAML frontmatter and
JSON. Fail generation on unknown or unresolved `{{variables}}`; do not leave partial tokens.
The manifest is authoritative: generate only its five workers plus coordinator, not a
duplicate implementation profile. Preserve canonical scalar model IDs and
`include-custom-instructions: true` on repo-aware agents.

Then write `.github/.bootstrap-manifest.json` in the target project with
`templateSet: "workshop-bootstrap"`, `version` from `template-manifest.json`, the variable
values in `variables`, and the generated file list in `generatedFiles`. Also record
`projectPathFromGitRoot` (a repository-relative
path, `.` for a root project, or null outside Git) and `harnessPath: ".github"`. This marker
lets a runtime session identify the actual project folder. Do not persist user-specific
absolute roots; resolve them at run time and pass them in worker inputs. A future update
can then distinguish generated files from later team edits.

## Verify

After generation:

1. Confirm `.github/copilot-instructions.md` and every file in the generated file map now exist.
2. If the project already has a runnable build/test command, run it and report the result
   honestly — a freshly bootstrapped project with no feature implemented yet may have
   nothing to test, and that's fine to report as-is rather than treated as a failure.
3. Report: files created, the variable values used (flagging proposed defaults separately
   from discovered facts), and anything left as an open question for the human — most
   commonly, which issue tracker to connect if none was already configured.
4. Check manifest/file-map agreement, unresolved variables, canonical scalar model IDs,
   worker/skill references, terminal worker tools and the shared root/result contracts.
   These are static checks, not evidence that tools reach a nested worker.
5. Runtime qualification remains separate: start a fresh session at `project_root`, select
   `feature-orchestrator` as the session agent, and prompt `Implement <ticket>`. No per-run
   ownership incantation is needed. Confirm when prompted, authenticate if required, and
   continue the same run at its blocked step. Do not launch another orchestrator.
   The generated coordinator distinguishes definitions, authenticated ticket access without
   ticket content, and effective capabilities. Its bounded artifact-only probes run only
   after confirmation. Report them unverified until actually executed; compare a blocked
   intended nested dispatch with the same canonical worker selected directly, not an alias.
   Skill source must be reported accurately as injected/read/unavailable, not assumed
   preloaded. Runtime nesting/tool/model availability depends on the host and user settings;
   prompts and metadata alone do not enforce deterministic execution.

## Important

Do not guess a variable value you can't actually determine — derive it from real analysis
or flag it as an assumption needing confirmation, the same discipline every other skill in
this pipeline is held to. Do not modify any of the target project's existing source files;
this skill only ever adds `.github/copilot-instructions.md`, `.github/instructions/`,
`.github/agents/`, `.github/skills/`, and `.github/.bootstrap-manifest.json`.
