# Phase 2 — Instructions and MCP

Phase 2 adds project-specific Copilot instructions, test guidance and an MCP configuration
to the API snapshot. It does not install agents or skills; user-directed work is still done
directly in the active session.

The `jira` server key is configured in `.vscode/mcp.json`. Configuration alone does not
prove that a runtime exposes the tools or that authentication works.

## Progression

- [Phase 1](../phase-1/README.md) is the current read-only reference for this workshop.
- [Phase 3](../phase-3/README.md) adds a reusable, explicitly invoked direct skill.

The phase-1 reference is not to be deleted or overwritten. For a fresh bootstrap
demonstration, use a separate clean target.
