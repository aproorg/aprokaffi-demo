# Phase 6 — The finale

Phase 6 shows a fully implemented bootsrap example derived from simple prompts from the 
previous phases. This is the culmination of the 5 previous phases and can be tested 
by running the `workshop-bootstrap:bootstrap` skill against a completely empty project 
(phase 1) to generate the same workflow you see here, from nothing.
It includes the `feature-orchestrator` pipeline (planning on Opus, mechanical
implementation and test-writing on Haiku, verification, and a self-improving retro step),
plus the [`test-plugin-marketplace`](../test-plugin-marketplace) that turns that pipeline
into a reusable, installable Claude Code plugin (`workshop-bootstrap:bootstrap`) able to
generate the same workflow, scoped to any project, from nothing.

For the fuller version of this idea, see
[bootstrap-demo](https://github.com/aproorg/bootstrap-demo).


## Try it yourself

This is the closing move of the demo: from this folder, `workshop-bootstrap:bootstrap` can
be run against [phase 1](../phase-1/README.md) — a genuinely empty project — to generate
the same workflow you're looking at right now, from nothing, in one pass.
