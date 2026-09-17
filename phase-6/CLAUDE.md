# Claude Code workshop demo

This repository is a teaching-sized subset of the Bootstrap marketplace plugin at revision `44c3472a641776a82e8d8b88f014e9400bf12640`. It is intentionally incomplete. The marketplace templates remain the source of truth.

## Project

`DemoTaskApi` is a .NET 9 minimal API with an in-memory task and owner service.

```text
dotnet build api-project/DemoTaskApi.sln
dotnet test api-project/DemoTaskApi.sln
```

## Workflow routing

| Request | Read | Entry point |
|---|---|---|
| New feature | `.claude/workflows/feature-workflow.md` | `/feature-start` |
| Pull request | `.claude/skills/pr-create/SKILL.md` | `/pr-create` |
| Retrospective | `.claude/skills/retro/SKILL.md` | `/retro` |
| Template sync | `.claude/skills/sync-bootstrap/SKILL.md` | `/sync-bootstrap` |

The workflow owns the order and quality gates. Agents own narrow pieces of work. `.claude/anti-patterns.md` is the single source of truth for constraints.

## Demo boundary

The feature brief adds `PATCH /tasks/{id}/complete`. It must make an existing task complete and return it with `200 OK`, or return `404 Not Found` for a missing task. It must not add authentication, persistence, or unrelated cleanup.

Repeat completion is deliberately undecided in the brief. Record that question during planning or the retrospective rather than inventing API behaviour silently.

## Source provenance

The files under `.claude/` are a small, reviewed teaching subset derived from the Bootstrap marketplace plugin revision above. They omit unrelated stacks, bugfix/refactor flows, merge-conflict handling, and most bootstrap variables.

