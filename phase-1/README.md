# Phase 1 — Bare API

The starting point for this workshop: `DemoTaskApi`, a plain .NET 9 minimal API, with no
Claude Code setup at all — no `CLAUDE.md`, no `.mcp.json`, no skills or agents.

## Try it yourself

From this bare state, prompting:

> Please init a new claude.md in the phase 1 project

and then:

> now please add to the local claude settings in this project a connection to the
> atlassian mcp so I can call this ticket https://programm.jira.com/browse/GENAI-1805 <- replace with your own ticket

produces the starting point for [phase 2](../phase-2/README.md): a documented project with
a live Jira MCP connection, but still no reusable Claude setup.

You can also try, from this same clean state:

> ok please use the mcp to read the ticket and implement it

to see the ticket implemented directly, by hand, with no skill or agent involved — the
baseline every later phase improves on.
