# Apró kaffi demo: GitHub Copilot version

The GitHub Copilot version of the demo, phases 1–6. Each `phase-N` is a full snapshot of
the same DemoTaskApi project with a progressively more developed Copilot configuration.

## Progression

- [Phase 1](phase-1/README.md) is the read-only reference snapshot.
- [Phase 2](phase-2/README.md) adds project instructions, test guidance and MCP metadata;
  it has no agents or skills.
- [Phase 3](phase-3/README.md) adds an explicitly invoked direct Jira ticket skill.
- [Phase 4](phase-4/README.md) adds a terminal ticket-taking agent that explicitly reads
  and follows that skill; it implements and tests without delegation.
- [Phase 5](phase-5/README.md) is the manually assembled six-profile pipeline: one
  coordinator and five terminal workers, with an approved-plan-only implementation worker.
- [Phase 6](phase-6/README.md) packages that pipeline in the
  [local marketplace](test-copilot-marketplace/README.md), adds bootstrap, and connects
  project-agnostic retro proposals back to the local templates.

The phase-4 agent takes a ticket and invokes the direct skill through an explicit
instruction-file read. In phases 5 and 6, the implementation worker receives only an
approved production implementation plan; it has no ticket tools or execute permission and
does not write tests. These are different roles.

Retrospectives in phases 5 and 6 are read-only, proposals-only, and never edit workflows or
create branches, commits or pull requests. MCP configuration and model pins describe
settings; they are not evidence of runtime tool discovery, authentication or effective
models. The phase-1 reference is intentionally read-only; use a separate clean target for
any future bootstrap demonstration, not deletion or overwrite of phase 1.

Phase 5's retrospective proposes improvements only to its own workflow. The shared-template
and marketplace tie-in is introduced in phase 6. The local marketplace is a sibling of the
phase folders, matching the layout of the Claude workshop.
