# dallas history

## Project Context
Project: Puddle
Language/Stack: C# MonoGame (SDL2, TiledSharp, XNA pattern)
Goal: Port to browser, deploy to Vercel
Branch: squad/web-port-spike
Owner: Jake
Approach: Docs-first. No big rewrites until feasibility is documented in docs/web-port-feasibility.md.
Key files: Game1.cs (game loop), Program.cs (entry point), Level.cs, Controls.cs, Content/ (assets), SDL.dll, TiledSharp.dll

## Learnings

### 2026-05-03 — Web Port Feasibility Analysis

- **MonoGame version**: This project uses MonoGame **v3.0** (old-style MSBuild .csproj, non-NuGet, WindowsGL backend). It is NOT using MonoGame 3.8+. Upgrading to WASM requires a full toolchain migration first.
- **Deprecated namespaces**: `Microsoft.Xna.Framework.Storage` and `Microsoft.Xna.Framework.GamerServices` are referenced in every source file but both were removed from modern MonoGame. Any WASM path must strip these.
- **Arial.xnb**: The only compiled XNA Content Pipeline artifact. It is a binary `.xnb` font file — not a raw `.ttf`. Must be regenerated via MonoGame Content Pipeline (Option 1) or replaced with a web font (Option 2).
- **TiledSharp**: A local `.dll` (v1.0.5191, pre-NuGet era) is vendored in the repo. Object types from TMX are passed directly to `Activator.CreateInstance(Type.GetType(obj.Type), obj)` — this is the entity spawning mechanism.
- **Level format**: All 17 levels are Tiled TMX XML v1.0 (22×22 tiles at 32px = 704×704px maps). Object layers use `<object type="...">` for entity placement. Fully portable.
- **Assets**: All textures are raw `.png` (no XNB except the font), all audio is `.wav`. Both are browser-compatible without conversion.
- **Recommended path**: Option 2 — TypeScript + Phaser 3. Game logic is clean and mechanical (~15 entity classes), all assets transfer, Phaser 3 reads TMX natively. Vercel deploy is `vite build` → static folder.
- **Key blocker before spike**: Confirm Phaser 3 parses these specific TMX v1.0 object layers correctly, and identify exact format of `type` attribute in object definitions.

### 2026-05-03 — Ripley's Repo-Map Key Findings

**Technical Map Complete. Six Hard Blockers Identified.**

Ripley completed the repo-map and identified six hard blockers for any browser port: .NET Framework 4.5 target (needs .NET 6+ migration), platform-guarded entrypoint, MonoGame 3.0 with local Windows DLLs, SDL.dll Windows binary, TiledSharp sync I/O (requires async fetch replacement), OpenAL audio (requires Web Audio API). Medium-complexity issues include reflection-based entity instantiation (AOT/IL-linker incompatible), frame-rate-coupled physics, XNB font, and canvas sizing. All game logic is pure C# and portable; all assets (PNG, WAV, TMX) are browser-compatible. Recommended next spike: migrate `.csproj` to SDK-style .NET 6+ with MonoGame 3.8.1 NuGet to enable Mac builds.

