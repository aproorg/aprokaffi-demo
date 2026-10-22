# Workshop Bootstrap

Generates a project-scoped Copilot harness from the templates in
`skills/bootstrap/assets/templates`. The bootstrap skill is the generator: it analyzes
the actual project and renders the manifest's file map after approval. There is no
separate executable generator.

## Local workshop source

This plugin belongs to the [local workshop marketplace](../../README.md).
Its editable source is `copilot\test-copilot-marketplace` in the demo repository.
Bootstrap and phase-6 retrospectives reference the templates in this local copy, not
the upstream GitHub marketplace or an installed plugin cache. The directory shares the
demo repository's Git history; it is not a separate checkout.

When generating a project, record a portable source location relative to that project's
root, plus `plugins\workshop-bootstrap\skills\bootstrap\assets\templates`.
For phase 6 the marketplace location is `..\test-copilot-marketplace`.
Ask for the local source if it cannot be resolved; do not guess an upstream URL.

## Run in the Copilot App

1. Bootstrap a fresh project using the `bootstrap` skill, review its detected stack,
   project folder, variables and generated-file list, then approve generation. Existing
   harnesses require a deliberate merge, not a fresh bootstrap overwrite.
2. Start a fresh session at the **actual project folder**, even when it is below the
   Git root (for example, `copilot/phase-1`). Select **feature-orchestrator** as the session
   agent and prompt `Implement <ticket>`. No per-run ownership incantation is needed.
3. Review discovery and authentication results, then confirm ticket access and the
   artifact-only capability probes. Authenticate if required and continue the **same run**
   at its blocked step; do not select/spawn another orchestrator.
4. The coordinator qualifies canonical workers before fetching ticket content once,
   then dispatches planning, implementation, testing, verification and retrospective.
   Review the stage evidence and final verdict. A blocked/failed stage is not completion.

Selecting the orchestrator profile already makes that session the execution owner.
Delegating to it from a general session activates the same ownership contract, but adds
a nesting boundary that needs its own qualification. Workers never delegate or act as
alternate coordinators. These are instructions, not a guarantee of deterministic runtime
enforcement.

## Roots, evidence and capability qualification

The actual bootstrapped folder is `project_root`; its `.github` is `harness_root`. The
bootstrap manifest records a portable Git-relative project location, not a user's absolute
path. Runtime dispatches pass absolute roots; plan/file paths remain project-relative.
Valid `applyTo` globs such as `**/*.Tests/**` are unchanged.

Definition discovery, authenticated tracker access and effective worker tools are separate
checks. Tool lists configure/filter tools; they do not prove a nested worker can edit or
execute. After confirmation, the coordinator uses session artifacts for bounded read,
search, create/update/read-back probes, plus a harmless command from the test worker.
No production files are canaries. If the intended nested dispatch fails, compare once with
the **same profile selected directly** in a fresh diagnostic session, using isolated
artifact paths. That session only runs the probe, not the feature workflow. Record direct
results as unverified until observed; do not replace a canonical worker with an alias.
One requalification follows a concrete configuration fix; repeated failure stops the run.

Outside the App, use a unique OS-temp run directory when no session-artifact directory is
supplied. The small run record captures parent/worker identities, stage input/output
references, errors, command/cwd/exit-code results and deviations. It is not committed to
the project. Preserve diagnostic facts while redacting private ticket text and secrets;
never copy raw private logs into a public proposal or PR.

## 0.3.0 contract changes

- One execution owner resumes across confirmation/authentication. The unused
  `feature-implementer` alternate is no longer generated; `implementation-agent` is canonical.
- Shared `passed|blocked|failed` results require outputs, evidence and a blocker field.
  Execution workers report the actual skill source (`injected|read|unavailable`); links
  are locators, not guaranteed preloads.
- The implementer gets a self-contained production plan, not the raw ticket or test plan.
  Testing only passes with the required command succeeding. Production defects return to
  the implementation owner; verification FAIL prevents completion. Recovery is bounded.
- Retro has read/search tools only and returns proposals, never edits/branches/commits.
  Diagnoses distinguish requirement, plan, skill, coordinator, worker and runtime issues,
  citing observed facts separately from hypotheses.
- Model pins use canonical scalar IDs. Repository-aware profiles retain
  `include-custom-instructions: true`. Dispatch restrictions live in the profile body,
  not an assumed App/CLI-enforced `agents:` frontmatter allowlist.

The [custom-agent configuration reference](https://docs.github.com/en/copilot/reference/custom-agents-configuration)
documents scalar `model` values and filtering via `tools`. Host-specific behavior still
requires qualification; metadata cleanup is not evidence of the cause of an earlier failure.

For existing installations, manually review/merge the regenerated contracts and marker.
Remove an old generated `feature-implementer` only after checking it was not customized
or used independently; bootstrap does not delete an existing project's files.
Update the installed plugin through its normal installation mechanism, not by editing
its installed copy. Proposals target this local marketplace's source templates; applying
them still requires a separate human decision.

### Evidence and remaining runtime uncertainty

The motivating App run was already using the orchestrator but recursively spawned it
across confirmation/authentication. Its planner ran on the intended model; planning was
not skipped. Workers tried Git-root instruction paths instead of the nested project folder.
An implementation worker reported read/search-only capability without attempting an edit;
a differently invoked alternate later edited successfully. Even a git-status request
recursed through built-in task agents. Preflight found definitions but did not establish
tracker authentication. The eventual implementation and tests passed, while the retro's
sanitized input omitted those earlier failures.

Those are distinct observed failures. Ancestor tool filtering, nesting and instruction
inclusion differences are hypotheses, not proven runtime restrictions. Changing names is
not a demonstrated fix. The coordinator's edit/execute envelope supports downstream
qualification and artifact bookkeeping while its body prohibits stage takeover; live
qualification must establish whether that envelope changes effective worker tools.
Scalar model metadata is compatibility hardening, not a claim that model resolution failed.

## Regression checks

From the local marketplace root (`copilot\test-copilot-marketplace`), using existing
PowerShell 7 (no packages to install):

```powershell
& .\plugins\workshop-bootstrap\tests\templates.Tests.ps1
```

The script renders all manifest templates for .NET and JavaScript fixtures at both root
and nested project locations, keeps a Git-root instruction decoy intact, checks portable
marker serialization, file/skill inventories, profile metadata shapes, ownership, stage
gates, capability qualification and proposals-only retro. Its frontmatter checks cover
the deliberately simple profile subset, not a general YAML parser. Temporary fixtures are
cleaned up. The script models the documented rendering algorithm; it does not execute an
AI bootstrap or a live App workflow. Static success does not qualify authentication,
tool propagation, model availability, nesting settings or instruction following.
