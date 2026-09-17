---
name: sync-bootstrap
description: Compare local teaching configuration with the marketplace subset
---

# `/sync-bootstrap`

Treat the Bootstrap marketplace plugin as the source of truth. Read `.claude/.bootstrap-manifest.json` to distinguish generated files, changed generated files, preserved user-owned files, and skipped files.

Show a file-by-file diff before importing any template change. Preserve local teaching choices and do not import unrelated stacks or workflows into this demo.

