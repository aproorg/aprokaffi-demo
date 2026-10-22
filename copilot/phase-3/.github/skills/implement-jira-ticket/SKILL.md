---
name: implement-jira-ticket
description: >
  Implements a feature or fix in this project directly from a user-supplied Jira ticket key
  or URL. Explicitly invoke this skill for direct, non-orchestrated work; it reads the
  authorized ticket once, makes the approved code change, adds matching tests and runs the
  required suite. Do not use it as a fallback for a blocked pipeline.
disable-model-invocation: true
---

## What this does

Takes a Jira ticket as the contract for a change, and turns it into working code plus tests
without expanding beyond its approved scope. Existing project changes and explicit human
decisions must be reused rather than duplicated or re-asked.

## Ticket source

Jira, through the configured `jira` MCP server and only the `jira/*` ticket namespace.
Configuration alone does not prove that tools are available or authenticated.

## Process

0. **Resolve roots and authorization.** Use the root contract in
   `<harness_root>/copilot-instructions.md`; read it and
   `<harness_root>/instructions/tests.instructions.md` explicitly. Check authentication
   only with an identity/auth-status operation, never by fetching ticket content. If no
   such operation is available, report authentication as unverified and ask the user to
   resolve it. Require explicit user authorization for ticket access and edits before
   either. Reuse any supplied approval, approved proceed/no-op decision and cached ticket.
   Do not invoke an orchestrator or worker.

1. **Resolve the ticket once.** Reuse a supplied successful ticket fetch. Otherwise extract
   the issue key from the supplied key or URL and, after authorization, fetch summary,
   description and relevant comments exactly once. If ticket tools or instructions are
   missing, block with the actual error; do not fabricate ticket content or refetch merely
   because execution paused.

2. **Extract the actual scope.** Read the description as the acceptance criteria. Note
   explicitly:
   - What must change.
   - What the ticket explicitly excludes — treat exclusions as binding, not suggestions.
   - Anything the ticket leaves unstated that your implementation will have to decide one
     way or another (an edge case, a status/error choice, idempotency, ordering, etc.).

3. **Check existing work and decisions.** Search the project for the requested change
   before editing. Reuse a supplied approved proceed decision; if the change already
   exists and no proceed/no-op decision is supplied, stop for that decision. An approved
   no-op must explain what satisfies the requirements and still follow the approved test
   requirements. Reuse a cached successful ticket fetch across all pauses; never refetch
   because authorization or a product decision is pending.

4. **Resolve load-bearing questions.** If behavior is unstated and would change observable
   results, surface the question and get a decision or explicit permission to choose a
   default before editing. Do not stall on cosmetic ambiguity.

5. **Read the project's conventions.** The common instructions document the actual
   architecture and conventions. Match existing patterns rather than introducing a new
   style.

6. **Implement only what the ticket asks for.** Resist bundling in adjacent cleanup,
   renames, or "while I'm here" improvements — even ones that would objectively improve the
   code. If you notice something worth fixing that's out of scope, mention it at the end
   instead of doing it.

7. **Write tests** matching this project's documented conventions, including the happy
   path and relevant edge cases at service and HTTP endpoint levels.

8. **Run the full suite at `project_root` before reporting done:**
   ```bash
   dotnet test api-project\DemoTaskApi.sln
   ```
   Record the actual command, absolute cwd, exit code and result. Missing tools,
   instructions or commands block; an unresolved attempted command failure is failed.

9. **Report evidence.** Return the common `status`, `outputs`, `evidence` and `blocker`
   fields, ticket key, files changed, tests added, out-of-scope items, actual fetch and
   command evidence, and any approved no-op decision. Never delegate or mutate Git; leave
   changes unstaged for review. This manual path is not evidence that an orchestrated
   pipeline passed.
