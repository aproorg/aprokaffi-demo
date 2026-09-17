# Anti-Patterns to Avoid

This file contains constraints learned during the workshop. It is referenced by the root guidance, workflow, agents, and retrospective skill.

## Workflow

- Do not write code before the acceptance criteria have a human approval gate.
- Do not hide a product decision inside an agent prompt.
- Do not skip independent QA because a change is small.
- Do not create a pull request without the test command, result, and unresolved decisions.
- Do not propagate a local lesson to the shared template without classifying it first.

## Testing

- Do not treat a green build as proof that the endpoint behaviour is correct.
- Do not write only status-code assertions when the response body carries the behaviour under test.
- Do not let the implementation agent design its own test specification without a separate review context.

## Scope

- Do not add authentication, persistence, or unrelated cleanup to the task-completion slice.
- Do not change repeat-completion semantics without recording the decision and its tests.

## API design

- For a state-changing endpoint, explicitly decide and test the behaviour of a repeated request when idempotency could matter. Do not let the first implementation silently become the contract.
