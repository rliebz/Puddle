# ripley history

## Project Context
Project: Puddle
Language/Stack: C# MonoGame (SDL2, TiledSharp, XNA pattern)
Goal: Port to browser, deploy to Vercel
Branch: squad/web-port-spike
Owner: Jake
Approach: Docs-first. No big rewrites until feasibility is documented in docs/web-port-feasibility.md.
Key files: Game1.cs (game loop), Program.cs (entry point), Level.cs, Controls.cs, Content/ (assets), SDL.dll, TiledSharp.dll

## Learnings

### 2026-05-03 — repo-map task

- **Entry point is guarded `#if WINDOWS || LINUX`** — no browser/WASM entrypoint exists. Any browser port needs a new entrypoint.
- **Target is .NET Framework 4.5**, not .NET Core/5+. Cannot use `dotnet` CLI or Blazor WASM without a full project migration to SDK-style csproj + .NET 6+.
- **MonoGame version is 3.0** (DLL hints point to `MonoGame\v3.0\Assemblies\WindowsGL\`). No NuGet packages. References are local Windows paths — project does not build out-of-box on Mac.
- **TiledSharp uses raw `System.IO` file access** (`new TmxMap("Content/Levels/...")`) — cannot run in browser without async fetch replacement.
- **Level objects are instantiated via reflection** (`Type.GetType(obj.Type)` + `Activator.CreateInstance`) from TMX object type strings — AOT/IL-linker incompatible without explicit type preservation.
- **No camera system** — viewport equals full backbuffer, sized dynamically from TMX map dimensions at each level load (22×22 tiles × 32px = 704×704 on gameplay levels).
- **Physics is frame-rate coupled** — `Level.count++` tick counter, not delta-time. Must refactor for variable-rate browser `requestAnimationFrame`.
- **Single SpriteBatch per frame**, no sort mode or layers, draw order hardcoded: items → projectiles → enemies → player.
- **Audio via `SoundEffect`/`SoundEffectInstance`** (OpenAL under the hood). Music is looped instances. WAV assets are browser-compatible, but API is not.
- **`Controls.cs` is cleanly abstracted** — clean separation of input polling from game logic, easy to re-implement for browser keyboard events.
- **All game logic (physics, collision, state) is pure C#** — portable with no platform dependencies.
- **TMX tile layer rendering is absent** — Level.Draw() only draws game objects; background is a stretched PNG. No tile-layer rendering code found. This needs further investigation.

### 2026-05-03 — Dallas's Feasibility Recommendation

**Recommendation: TypeScript + Phaser 3 (Option 2)**

Dallas evaluated three options against the hard blockers and medium-complexity issues identified in the repo-map. MonoGame WASM is blocked by toolchain debt (v3.0 upgrade, deprecated namespaces, binary font, vendored TiledSharp). TypeScript + Phaser 3 is recommended because all assets (TMX, PNG, WAV) transfer without conversion, Phaser 3 reads TMX natively, C# entity logic is clean and translates mechanically to TypeScript, and Vercel deploy is a standard `vite build`. Before spiking: confirm Phaser 3 parses these specific TMX v1.0 object layers and identify exact `type` attribute format.

