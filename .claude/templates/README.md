# Local bootstrap templates

This folder is the complete template input for the workshop's minimal local `/bootstrap` command.

## Starting state

Before `/bootstrap`, `.claude/` should contain only:

```text
.claude/
  commands/
    bootstrap.md
  templates/
    README.md
    template-manifest.json
    dotnet-api/
    universal/
```

The command reads these files, shows a generation plan, and waits for approval before creating the project guidance and generated workflow files.

## Generated state

After approval, the command creates:

```text
CLAUDE.md
.claude/
  .bootstrap-manifest.json
  anti-patterns.md
  agents/
  skills/
  workflows/
```

The templates are intentionally small. They contain only the pieces needed to demonstrate a skill, narrow agents, an MCP handoff, a workflow, independent QA, pull-request preparation, and a retrospective.
