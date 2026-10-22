---
name: implement-jira-ticket
description: >
  Implements a feature or fix in this project from a user-authorized Jira ticket. Explicitly
  user-invoked, manual and non-orchestrated only; never use it to recover a blocked pipeline.
disable-model-invocation: true
---

## What this does

Takes an authorized Jira ticket as the contract for a change and implements it with
matching tests, without expanding beyond the approved scope. Existing changes and human
decisions must be reused rather than duplicated or re-asked.

## Ticket source

Jira through the configured `jira` MCP server and only the `jira/*` ticket namespace.
Configuration alone does not prove runtime tool availability or authentication.

## Process

0. **Roots and authorization.** Resolve and read
   `<harness_root>/copilot-instructions.md` and
   `<harness_root>/instructions/tests.instructions.md`. Check authentication only with an
   identity/status operation, never with a ticket fetch. Require user authorization before
   ticket access or edits. Missing instructions/tools block. Do not delegate or invoke the
   orchestrator.
1. **Fetch once.** Reuse supplied cached ticket content. Otherwise, once authorized, fetch
   summary, description and relevant comments once using `jira/*`. Never refetch because
   execution paused. No ticket content may be inferred from a key or URL.
2. **Scope.** Identify requested changes, binding exclusions and behavior the ticket leaves
   unstated. Ask for a decision on any load-bearing ambiguity; don't guess observable
   behavior.
3. **Existing work and decisions.** Search the project before edits. Reuse an approved
   proceed decision. If the change already exists without an approved proceed/no-op
   decision, stop for the human. An approved no-op explains what already satisfies the
   requirements and still follows approved testing requirements.
4. **Conventions and implementation.** Read and follow project conventions. Implement only
   approved ticket scope; do not bundle unrelated cleanup.
5. **Tests and suite.** Write tests at service and HTTP endpoint levels following project
   conventions, then run the full suite at `project_root`:
   `dotnet test api-project\DemoTaskApi.sln`. Record actual command, absolute cwd, exit code
   and result. Missing tools, instructions or commands block; unresolved executed-command
   failure is failed.
6. **Result.** Return common `status`, `outputs`, `evidence` and `blocker`, the ticket key,
   changed files, tests, out-of-scope items, actual fetch/command evidence and any approved
   no-op. Never delegate or mutate Git; leave changes unstaged. This manual skill is not a
   fallback for a blocked pipeline and does not certify an orchestrated result.
