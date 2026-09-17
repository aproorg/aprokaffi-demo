# Feature workflow

This workflow makes the handoffs and gates visible for the `PATCH /tasks/{id}/complete` story.

## 1. Source the request

Read the prepared GitHub issue through the GitHub MCP. Return the requested behaviour, explicit out-of-scope items, missing decisions, and likely files to inspect. Treat the result as source material, not as a truth oracle.

## 2. Human acceptance gate

Run `/feature-start`. Inspect the existing API. Propose acceptance criteria that include:

- an existing task becomes complete and returns `200 OK`;
- a missing task returns `404 Not Found`;
- service and endpoint behaviour both receive tests;
- repeat completion is explicitly decided or recorded as an unresolved question.

Stop until the presenter approves the criteria.

## 3. Test design

Delegate to `testing-manager`. Accept a short behavioural test specification with no production code.

## 4. Implementation

Pass the approved criteria and test specification to `feature-implementation`. Keep the working tree unstaged and inspect the diff.

## 5. Verification and independent QA

Run `dotnet test api-project/DemoTaskApi.sln`. Then delegate to `qa-review` with a clean context. Fix only small findings live. Otherwise record the recovery point.

## 6. Project pull request

Run `/pr-create` or the git-manager agent. Include the criteria, test result, QA verdict, and any unresolved repeat-completion decision. Keep the project PR separate from template maintenance.

## 7. Retrospective

Run `/retro`. Classify lessons before changing files. Update local rules for local lessons. Prepare a separate reviewed marketplace PR for a generalizable lesson.

