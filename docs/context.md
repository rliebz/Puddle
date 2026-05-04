# Puddle Web Port Context (single source of truth)

## Current state
- Branch: squad/web-port-spike
- Last checkpoint commit: cfab8c6 (cast team — Alien crew)
- What runs today: docs/repo-map.md ✅, docs/web-port-feasibility.md ✅
- What does not run today: no browser prototype yet, no build verification on Mac yet

## Decisions made
- Decision 1: **TypeScript + Phaser 3** is the recommended port path (Dallas). Assets transfer without conversion; Phaser 3 reads TMX natively; C# logic maps mechanically to TS. See docs/web-port-feasibility.md.
- Decision 2: MonoGame WASM (Option 1) is blocked by .NET 4.5 / MonoGame 3.0 toolchain debt — two removed namespaces, compiled binary font, vendored TiledSharp. See docs/repo-map.md.

## Next 3 tasks (keep small)
1. **Ash reviews docs/web-port-feasibility.md** — critique-only gate before committing to the TS+Phaser path
2. **Spike 2: TypeScript + Phaser 3 scaffold** — minimal project that loads one TMX level and renders one sprite from Content/; pass = something visible in browser
3. **Parker: Vercel deploy skeleton** — static build config, vercel.json; can run in parallel with Spike 2

## Active spike(s)
- Spike name: Spike 2 — Web feasibility probe (TypeScript + Phaser 3)
  - Goal: Load one TMX level and render one sprite in-browser from the existing Content/ assets
  - Pass criteria: Visible game element in browser window served by a local dev server
  - Fail criteria: Asset format incompatibility, TMX parser error, or blocker that requires rewrite of content pipeline
  - Notes: Phaser 3 includes built-in TMX/Tiled support. Existing .tmx files should load directly.

## If token capped or stopping
- Run: ./scripts/pause.sh
- Then resume with: ./scripts/resume.sh
