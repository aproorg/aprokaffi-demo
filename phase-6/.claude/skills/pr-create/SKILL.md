---
name: pr-create
description: Create a project pull request from reviewed evidence
---

# `/pr-create`

Read the git-manager agent definition. Create a project PR only when the accepted criteria, test result, and independent QA verdict are available.

The PR description must preserve unresolved decisions as unresolved. If a template improvement is warranted, create or draft it separately against `aproorg/bootstrap-demo`; never fold the template change into the project PR.

