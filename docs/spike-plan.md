# Spike Plan (timeboxed prototypes)

## Spike 1: Baseline build and run
Goal: Get the current repo building and running locally, document entry points and asset pipeline.
Pass: Build and run instructions exist in docs/repo-map.md and game launches locally.
Fail: Blocking dependency cannot be installed or run.

## Spike 2: Web feasibility probe
Goal: Identify a plausible browser path and prove one small thing in-browser (render a sprite or load a level).
Pass: Minimal web prototype exists OR clear evidence it is not viable without rewrite.
Fail: No viable path found after timebox.

## Spike 3: Deployment skeleton
Goal: Once we have a web artifact, define the Vercel deploy approach.
Pass: vercel.json or Vercel config doc exists and a static build can deploy.
Fail: Hosting constraints block deployment.
