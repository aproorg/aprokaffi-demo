# Phase 6 — Reusable bootstrap and local marketplace

Phase 6 presents the same canonical six-profile pipeline as phase 5 — one coordinator and
five terminal workers — packaged as a reusable bootstrap plugin. Its implementation
worker is limited to an approved production implementation plan; it receives no ticket
content, has no ticket tools or execute permission, does not write tests and does not run
commands.

## Local marketplace

The [local Copilot marketplace](../test-copilot-marketplace/README.md) is the counterpart
of Claude's `test-plugin-marketplace` folder. It contains both plugins from the Copilot
marketplace snapshot, including `workshop-bootstrap`, its templates and regression checks.

Install `workshop-bootstrap` from that local marketplace, then invoke
`/workshop-bootstrap:bootstrap` against a clean target. The skill reads the target,
presents its detected stack, variables and file map for approval, and generates the same
workflow with project-specific values. It refuses to overwrite an existing setup.
For the workshop, phase 1 can serve as that target after its temporary reference setup
has been removed by the presenter; do not delete it automatically.

## Retrospective tie-in

Phase 5 reviews only its project's workflow. Phase 6 adds the distinction between
project-specific and project-agnostic improvements. The latter produce proposed diffs
against `..\test-copilot-marketplace\plugins\workshop-bootstrap\skills\bootstrap\assets\templates`,
not a remote repository or installed cache.

The retro remains read-only: no edits, branches, commits, pushes or pull requests.
The local marketplace is part of this demo repository, not a separate Git checkout.
Manifest metadata, model pins and MCP configuration are not proof of runtime model,
tool or authentication behavior.

## Progression

- [Phase 5](../phase-5/README.md) is the manually assembled equivalent without a bootstrap
  marker.
- Phase 4's ticket-taking terminal agent explicitly loads its skill and implements/tests;
  that role is intentionally different from the ticket-free plan-only pipeline implementer.

The bootstrap skill and template source used in this phase are both in the
[local marketplace](../test-copilot-marketplace/plugins/workshop-bootstrap/README.md).
