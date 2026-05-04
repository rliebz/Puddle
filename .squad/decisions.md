# Squad Decisions

## Active Decisions

### Project Guardrails: Puddle Web Port
**From:** Project  
**Date:** 2026-05-03T17:14:09-07:00  
**Status:** Established

#### Non-negotiables
- Do not do big rewrites until feasibility is documented in docs/web-port-feasibility.md
- Keep planning in repo files (docs/plan.md, docs/context.md)
- Prefer small, reviewable commits and checkpoint often

#### Sub agent discipline
- Use /fleet for sub agents when doing broad work
- Each sub agent writes a short summary to docs/subagent-notes/<name>.md
- Orchestrator reads those summaries before making decisions
- Do not spawn more than needed (sub-agent limit exists)

#### Slop prevention
- If unsure, write a question into docs/context.md instead of guessing
- Reviewer must be a different role/model than the implementer

#### Token cap fail-safe
If any agent hits a rate limit, quota, or token cap:
1) Immediately stop new work
2) Update docs/context.md with: where it stopped, what files changed, the exact next 3 tasks
3) Run ./scripts/pause.sh (or ask Jake to run it)
4) Inform Jake: "Token cap hit. Repo checkpointed. Ready to resume."

---

### Web Port Feasibility Options
**From:** Dallas (Lead / Architect)  
**Date:** 2026-05-03T17:14:09-07:00  
**Status:** Recommendation — pending spike validation  
**Ref:** docs/web-port-feasibility.md

#### Decision
**Recommended path: Option 2 — TypeScript + Phaser 3 partial rewrite.**

#### Rationale
Three options were evaluated for porting Puddle to browser + Vercel:

1. **MonoGame WASM** — Blocked by toolchain debt: MonoGame v3.0 → 3.8+ upgrade required, two removed namespaces (`Storage`, `GamerServices`) in every source file, compiled binary font (`Arial.xnb`), old vendored TiledSharp DLL. Clears none of this before touching the browser. Effort: Large.

2. **TypeScript + Phaser 3** *(recommended)* — All level data (TMX), PNG textures, and WAV audio transfer without conversion. Phaser 3 reads TMX natively. C# entity code is clean and translates mechanically to TypeScript. Vercel deploy is `vite build` → static folder. Effort: Medium (3–6 weeks).

3. **Full rewrite** — Warranted only if game design changes significantly. Not applicable here. Effort: Large+.

#### Required before committing
- Spike: Phaser 3 loads `Level1-1.tmx` object layers correctly
- Spike: Identify TMX `type` attribute format (qualified vs short name)
- Decision: Audio init strategy (click-to-start required for browser autoplay policy)

#### Files
- Full analysis: `docs/web-port-feasibility.md`
- Spike plan: `docs/spike-plan.md` (Spike 2 is the next gate)

---

### Ripley Findings: repo-map
**Date:** 2026-05-03T17:14:09-07:00  
**Source:** repo-map task  
**File produced:** `docs/repo-map.md`

#### Summary
Full technical map of the Puddle repo completed. Key findings for browser port feasibility:

#### 🔴 Hard Blockers
1. **`.NET Framework 4.5` target** — must migrate to `.NET 6+` (SDK-style csproj) before any browser port is possible. No `dotnet` CLI support in current form.
2. **`#if WINDOWS || LINUX` entrypoint guard** — `Program.Main()` will not compile for a WASM/browser target. New entrypoint required.
3. **MonoGame 3.0 with local Windows DLLs** — references `$(MSBuildExtensionsPath)\..\MonoGame\v3.0\Assemblies\WindowsGL\`. No NuGet, no Mac support without .csproj surgery.
4. **`SDL.dll` embedded as Windows native binary** — irreplaceable in browser sandbox. Must swap for WebGL/Canvas renderer.
5. **`TiledSharp` uses synchronous `System.IO` file access** — `new TmxMap("Content/Levels/...")` will fail in browser. Must be replaced with async asset fetch + in-memory XML parse.
6. **OpenAL audio (`SoundEffect`)** — not available in browser. Must port to Web Audio API.

#### 🟡 Medium Complexity
1. **Reflection-based object instantiation** — `Type.GetType(obj.Type)` + `Activator.CreateInstance` for all TMX objects. AOT/IL-linker in Blazor WASM strips unreferenced types by default. Needs explicit `[DynamicDependency]` or registration table.
2. **Frame-rate coupled physics** — uses integer `count++` tick counter, not delta-time. Browser runs at variable `requestAnimationFrame` rate (not guaranteed 60Hz). Behavior will drift or break without delta-time refactor.
3. **`SpriteFont` from XNB** — `Arial.xnb` is MonoGame Content Pipeline binary. Not usable in browser; needs conversion to TTF + new font rendering.
4. **Window/canvas sizing** — backbuffer resized from TMX at level load (`graphics.PreferredBackBufferWidth`). Browser canvas must be resized via DOM API.

#### 🟢 Reusable / Portable Assets
- All game logic (physics, collision, state machines) is pure C# — zero platform deps
- PNG sprite assets are browser-ready
- WAV audio files are browser-compatible (Web Audio API)
- TMX level data is plain XML — parseable anywhere
- `Controls.cs` input abstraction is clean and easily re-implemented for browser keyboard events
- AABB collision in `Sprite.Intersects()` — pure math

#### Open Questions
1. **Tile layer rendering** — `Level.Draw()` only draws game objects (items, projectiles, enemies, player). The background is drawn as a stretched PNG. There is no visible tile-layer rendering loop. Are the TMX tile layers purely decorative/unused, or is there a rendering path not yet found?
2. **Mac build path** — project does not build on Mac in current state. A `.csproj` migration spike is needed before any code can be run and verified on Mac.
3. **`Microsoft.Xna.Framework.Storage` + `GamerServices`** — imported in multiple files but likely unused at runtime. Should be removed before port to avoid stub complexity.

#### Recommended Next Steps
1. **Spike: Migrate `.csproj` to SDK-style targeting `net6.0` with MonoGame 3.8.1 NuGet** — this unlocks Mac builds and is a prerequisite for any further platform work.
2. **Spike: Enumerate all `Type.GetType` call sites** — document every TMX object type string to ensure they can be preserved through AOT compilation.
3. **Spike: Replace `TiledSharp` file I/O with in-memory parse** — validate that TMX XML can be parsed from a `Stream` or `string` without filesystem access.
4. **Spike: Delta-time physics refactor assessment** — estimate scope of decoupling physics from `count++` ticks.

## Governance

- All meaningful changes require team consensus
- Document architectural decisions here
- Keep history focused on work, decisions focused on direction

## Model policy (Puddle)
**By:** Jake  
**What:**  
- Leader/orchestrator uses **Opus 4.7 (1M context)**.  
- Code reviewer, designer, and dev designer use **Opus 4.7 (Extra High)**.  
- Implementers use **Sonnet**.  
**Why:** Max context for orchestration, extra-high reasoning for critique/design, lower-cost implementers for throughput.

## Cap and stop policy (Puddle)
**By:** Jake  
**What:** If any agent hits rate limit, quota, token cap, or "premium requests disabled":  
1) Stop creating new subagents immediately  
2) Update `docs/context.md` with: what was in progress, files touched, next 3 tasks  
3) Instruct Jake to run `./scripts/pause.sh`  
4) Default action is stop and resume later (only continue if Jake explicitly opts in)  
**Why:** Prevent partial work and preserve a clean resume point.
