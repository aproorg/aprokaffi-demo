---
name: implement-jira-ticket
description: >
  Implements a feature or fix in the DemoTaskApi project directly from a Jira ticket key
  or URL (e.g. "GENAI-1805" or a browse link). Reads the ticket's summary and description
  through the connected Jira MCP server, then makes the requested code change in
  api-project/ with matching service-level and HTTP endpoint tests. Use this any time the
  user references a Jira ticket and wants it implemented, built, or picked up — including
  phrasing like "implement GENAI-1805", "can you pick up this ticket", "build what's
  described in <ticket link>", or "start on the ticket I just linked". Do not use this for
  work that has no ticket reference, or for changes unrelated to the DemoTaskApi project.
---

## What this does

Takes a Jira ticket as the single source of truth for a change, and turns it into working
code plus tests — without expanding past what the ticket actually asks for. The point of
routing through a ticket instead of a freeform description is that the ticket is the
contract: if it doesn't say something, that's a signal to ask, not to guess.

## Process

1. **Resolve the ticket.** Extract the issue key from whatever was given (a bare key like
   `GENAI-1805`, or a browse URL — the key is the segment right after `/browse/`). Use the
   Jira MCP tools available in this session (a server named something like `atlassian` or
   `jira`, configured in this project's `.mcp.json`) to fetch the issue's summary,
   description, and any comments. If no Jira MCP tool is available, stop and tell the user
   — don't fabricate ticket content from the key alone.

2. **Extract the actual scope.** Read the description as the acceptance criteria. Note
   explicitly:
   - What must change (endpoints, service methods, models).
   - What the ticket explicitly excludes (tickets in this project often say things like
     "do not add authentication, persistence, or unrelated cleanup" — treat exclusions as
     binding, not as suggestions).
   - Anything the ticket leaves unstated that your implementation will have to decide one
     way or another (an edge case, a status code choice, idempotency behavior, etc.).

3. **If something load-bearing is unstated, ask before implementing.** This project treats
   silently inventing behavior for an undecided case as a bug in the process, even if the
   choice seems obviously reasonable. Surface the open question to the user in one or two
   sentences and get a decision (or explicit permission to pick a default) before writing
   code. Don't stall on truly cosmetic ambiguity — only pause for choices that would change
   observable behavior.

4. **Read the project's own conventions before writing anything.** This project's
   `CLAUDE.md` documents the architecture (minimal-API composition root in `Program.cs`,
   singleton in-memory services behind an interface, the storage-model/DTO split). Match
   those patterns exactly rather than introducing a new style — e.g. a new resource gets an
   `IXService`/`XService` pair and endpoints added as top-level statements in `Program.cs`,
   not a controller class.

5. **Implement only what the ticket asks for.** Resist bundling in adjacent cleanup,
   renames, or "while I'm here" improvements — even ones that would objectively improve the
   code. If you notice something worth fixing that's out of scope, mention it at the end
   instead of doing it.

6. **Write tests at both levels already used in this repo:**
   - A service-level xUnit test (alongside `TaskServiceTests.cs` / `TaskOwnerServiceTests.cs`)
     exercising the new/changed service method directly, including the not-found or
     invalid-input case.
   - A `WebApplicationFactory`-based HTTP endpoint test (alongside `TaskEndpointTests.cs` /
     `OwnerEndpointTests.cs`) exercising the new/changed route, including its error status
     code.

7. **Run the full suite before reporting done:**
   ```bash
   dotnet test api-project/DemoTaskApi.sln
   ```
   If anything fails, fix it before finishing — don't hand back a red test suite.

8. **Report back concisely**, referencing the ticket key, listing the files touched, and
   flagging anything you deliberately left out as out-of-scope.
