---
name: bootstrap
description: Generate the minimal workshop configuration from local templates
---

# `/bootstrap`

This is the minimal bootstrapper for this workshop. It is intentionally local: use the templates in `.claude/templates/` and do not install or call an external bootstrap plugin.

## Before writing files

1. Inspect the repository and confirm that it contains the existing .NET solution under `api-project/`.
2. Read `.claude/templates/template-manifest.json`.
3. Show a short generation plan listing the stack, files to create, and files that will remain untouched.
4. Wait for the presenter to approve the plan before writing anything.

If the solution is missing or the stack is not the expected minimal .NET API, stop and explain what is different.

## Generate after approval

Use only these local templates:

- `.claude/templates/dotnet-api/CLAUDE.md.tmpl` → `CLAUDE.md`
- `.claude/templates/dotnet-api/anti-patterns.md.tmpl` → `.claude/anti-patterns.md`
- `.claude/templates/dotnet-api/agents/*.md.tmpl` → `.claude/agents/*.md`
- `.claude/templates/dotnet-api/skills/feature-start.md.tmpl` → `.claude/skills/feature-start/SKILL.md`
- `.claude/templates/dotnet-api/skills/retro.md.tmpl` → `.claude/skills/retro/SKILL.md`
- `.claude/templates/dotnet-api/skills/pr-create.md.tmpl` → `.claude/skills/pr-create/SKILL.md`
- `.claude/templates/universal/skills/sync-bootstrap.md` → `.claude/skills/sync-bootstrap/SKILL.md`
- `.claude/templates/dotnet-api/workflows/feature-workflow.md.tmpl` → `.claude/workflows/feature-workflow.md`

Create `.claude/.bootstrap-manifest.json` that records the template paths, generated files, preserved user files, and any skipped files. Set `teachingTemplateSnapshot` to `.claude/templates/template-manifest.json`.

Preserve `.claude/commands/` and `.claude/templates/`. Do not delete or overwrite user-owned project files. Do not create unrelated stacks, workflows, or configuration.

## Verify

After generation:

1. Confirm that `CLAUDE.md` and the generated `.claude/` files exist.
2. Run `dotnet test api-project/DemoTaskApi.sln`.
3. Report the files created, the test command and result, and any unresolved assumptions.

The generated workflow is the orchestration layer for the rest of the demo. There is no separate orchestration agent.
