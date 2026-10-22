# Phase 5 — Manually assembled pipeline

Phase 5 contains the full six-profile workflow: one `feature-orchestrator` coordinator and
five terminal workers for planning, approved-plan-only implementation, testing, verification
and retrospective analysis. The implementation worker receives only the approved
implementation plan; it does not receive ticket content, use ticket tools, write tests or
run commands.

The retrospective is read-only and produces evidence-based proposals only. It does not
edit workflow files, create branches, commit, push or open pull requests. Configured model
pins and MCP metadata describe settings, not runtime model/tool/authentication evidence.

This is a manually assembled phase: root selection works from its selected working
directory containing its own `.github/copilot-instructions.md`. Phase 4's ticket-taking
agent explicitly loads a skill and implements/tests; that is intentionally different
from this pipeline's ticket-free, plan-only implementer.

## Retrospective

As in the [Claude phase-5 example](../../claude/phase-5/.claude/agents/retro-agent.md),
the retro reviews the plans, implementation, tests and verification findings. It explains
what worked, traces problems to their causes, and proposes concrete edits to this
project's agents and skills -- including the retro itself. It does not introduce shared
templates or send improvements elsewhere.

## Progression

- [Phase 4](../phase-4/README.md) is the terminal ticket-taking agent-plus-skill flow.
- [Phase 6](../phase-6/README.md) is the next step: making the workflow reusable across
  projects.
