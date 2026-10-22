# Phase 3 — Direct ticket skill

Phase 3 adds a reusable `implement-jira-ticket` skill for explicitly user-invoked,
non-orchestrated Jira ticket work. The skill reads the ticket once after authorization,
implements only the approved scope, adds matching tests and runs the required suite.
Automatic/model invocation is disabled; the skill is not a fallback for a blocked pipeline.

The `jira` server key is configured in `.vscode/mcp.json`, but configuration alone does not
prove runtime tool discovery or authentication.

## Progression

- [Phase 2](../phase-2/README.md) adds instructions and MCP without agents or skills.
- [Phase 4](../phase-4/README.md) adds a terminal ticket-taking agent that explicitly reads
  and follows this skill.

The phase-1 snapshot remains a read-only reference. Use a separate clean target for any
fresh bootstrap demonstration.
