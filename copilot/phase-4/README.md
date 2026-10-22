# Phase 4 — Ticket-taking implementation agent

Phase 4 adds a terminal `implementation-agent` that takes a user-directed Jira ticket and
explicitly reads and follows the reusable `implement-jira-ticket` skill. It owns the
ticket workflow and performs implementation plus the required testing; it does not
delegate.

The skill retains `disable-model-invocation: true`. The agent's explicit instruction-file
read is an ordinary user-directed load, not automatic skill invocation or model selection.
Configured MCP metadata is not runtime qualification.

## Progression

- [Phase 3](../phase-3/README.md) provides the direct skill without a wrapper agent.
- [Phase 5](../phase-5/README.md) introduces the manually assembled full pipeline. Its
  approved-plan-only implementation worker differs from this phase's ticket-taking
  implement-and-test agent.

Phase 1 remains a read-only reference; use a separate clean target for fresh bootstrap work.
