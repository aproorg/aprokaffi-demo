# Phase 1 — Bare API

The starting point for this workshop: `DemoTaskApi`, a plain .NET 9 minimal API, with no
GitHub Copilot setup at all — no `.github/copilot-instructions.md`, no `.vscode/mcp.json`,
no custom agents, skills or instructions.

## Try it yourself

From this bare state, prompting Copilot Chat:

> Please create a `.github/copilot-instructions.md` for this project

and then:

> now please add an atlassian MCP server to `.vscode/mcp.json` so I can call this ticket
> https://programm.jira.com/browse/GENAI-1805 <- replace with your own ticket

produces the starting point for [phase 2](../phase-2/README.md): a documented project with
a live Jira MCP connection, but still no reusable Copilot setup.

You can also try, from this same clean state:

> ok please use the mcp to read the ticket and implement it

to see the ticket implemented directly, by hand, with no skill or agent involved — the
baseline every later phase improves on.
