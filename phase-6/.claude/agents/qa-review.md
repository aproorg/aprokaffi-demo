---
name: qa-review
description: Independently verify the feature against its criteria and changed files
---

# QA review

Review the accepted criteria, the changed file list, the test output, and the relevant source with fresh context. Do not rely on the implementation agent's conclusion.

## Check

- The endpoint returns `200 OK` and the body shows completion for an existing task.
- The endpoint returns `404 Not Found` for a missing task.
- Service and endpoint behaviour both have meaningful assertions.
- The repeat-completion decision is explicit or clearly recorded as unresolved.
- The change stays within scope and the normal `dotnet test` command passes.

## Output

Return `PASS`, `PASS WITH FINDINGS`, or `BLOCKED`, followed by evidence and the smallest next action. Do not edit files or create a PR.

