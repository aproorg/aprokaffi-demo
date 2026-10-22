# Local Copilot marketplace

This directory is the Copilot counterpart of
[`claude/test-plugin-marketplace`](../../claude/test-plugin-marketplace).
It is a self-contained marketplace source inside the demo repository, not a nested Git
repository or an installed plugin cache.

The initial snapshot comes from
[`josuaApro/test-copilot-marketplace`](https://github.com/josuaApro/test-copilot-marketplace/tree/eb20c28bce68cd68535d2e71ccb1a17ca332feac).
Workshop references and proposals use this local copy; they do not require changes to
that upstream repository.

## Contents

- [Marketplace manifest](.github/plugin/marketplace.json), with local plugin sources.
- [Workshop Bootstrap](plugins/workshop-bootstrap/README.md): the bootstrap skill,
  eleven workflow templates, template manifest and regression checks.
- [Developer Toolkit](plugins/dev-toolkit/plugin.json): the code-auditor agent and
  PR-description skill.

The marketplace name is `aprokaffi-copilot-workshop`, distinct from the remote
`my-custom-marketplace`, so the local workshop source does not replace that registration.

## Workshop use

Register this directory as a local marketplace in Copilot, then install
`workshop-bootstrap` from **this** marketplace. Source edits do not automatically update
an already installed plugin; update or reinstall the local plugin before demonstrating
`/workshop-bootstrap:bootstrap`.

[Phase 5](../phase-5/README.md) reviews its own project workflow.
[Phase 6](../phase-6/README.md) introduces this marketplace, bootstrapping and
project-agnostic proposals against the local templates at
`plugins\workshop-bootstrap\skills\bootstrap\assets\templates`.
Retrospectives propose changes; they never edit, commit or open pull requests.

## Template checks

From this directory, using PowerShell 7:

```powershell
& .\plugins\workshop-bootstrap\tests\templates.Tests.ps1
```
