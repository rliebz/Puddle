# Web Port Feasibility: Puddle → Browser → Vercel

**Author:** Dallas (Lead / Architect)
**Date:** 2026-05-03T17:14:09-07:00
**Status:** Draft — awaiting spike validation

---

## Summary

Puddle is a C# MonoGame v3.0 (WindowsGL) game with ~15 entity types, Tiled TMX levels, PNG/WAV assets, and a local TiledSharp DLL. The game logic is self-contained and moderately simple. Three paths exist: (1) upgrade to MonoGame 3.8+ WASM runtime — technically possible but blocked by multiple deprecated APIs and old toolchain debt; (2) port game logic to TypeScript with Phaser 3 for rendering — the recommended path, reusing all level data and art assets with the lowest total risk; (3) full web-native rewrite — only warranted if the game design changes significantly. **Recommendation: pursue Option 2 (TypeScript + Phaser 3).** The C# code is clean enough to serve as a direct blueprint, all assets transfer without conversion, and Vercel static deploy is trivial.

---

## Option 1: Minimal-Change Browser Path — MonoGame WASM

### What this means

Upgrade the project from MonoGame v3.0 (old-style MSBuild, WindowsGL) to MonoGame 3.8.1+ (SDK-style .csproj, NuGet) and target the `MonoGame.Framework.WasmRuntime` package, which compiles .NET to WebAssembly via Blazor WebAssembly. An alternative is [KNI](https://github.com/kni-engine/kni) (a MonoGame fork with a WebGL/WASM backend), which has a more active browser story.

### What changes

| Item | Change required |
|---|---|
| `Puddle.csproj` | Full rewrite from old MSBuild → SDK-style; add `MonoGame.Framework.WasmRuntime` or KNI target |
| MonoGame version | 3.0 → 3.8.1+ (breaking changes in content pipeline and APIs) |
| `Microsoft.Xna.Framework.Storage` | Removed in modern MonoGame — all `using` references and any storage calls must be deleted |
| `Microsoft.Xna.Framework.GamerServices` | Removed in modern MonoGame — same treatment |
| `Arial.xnb` | XNA compiled binary font — cannot be loaded by modern MonoGame without recompiling through the MonoGame Content Pipeline; replace with a SpriteFont `.spritefont` descriptor and recompile, or use BitmapFont |
| `TiledSharp.dll` | Local DLL, version 1.0.5191 (very old, pre-NuGet era) — must verify NuGet TiledSharp ≥ 0.14 compatibility or switch to `TiledLib` / manual TMX parsing |
| `SDL.dll` | Embedded resource for WindowsGL — not applicable to WASM; remove |
| `Lidgren.Network` | Reference present in .csproj but not used in code — remove |
| `Program.cs` | `#if WINDOWS || LINUX` guard — add `#elif WASM` entry point |
| `OpenTK` reference | WindowsGL-specific; not needed under WASM backend |
| `SoundEffect` / `SoundEffectInstance` | Supported in MonoGame WASM but WAV decoding in browser may require MP3/OGG; test audio pipeline |
| `GamePad.GetState()` | Gamepad API in WASM works via browser Gamepad API — should work but needs testing |

### What can be reused

- All game logic: `Level.cs`, `Controls.cs`, `Game1.cs` game loop, all `Objects/` and `Enemies/` classes
- All PNG textures (direct — no XNB conversion needed; MonoGame 3.8 can load PNG directly)
- All WAV audio files (may need transcoding to OGG for browser compatibility)
- All TMX level files (XML format — fully portable)
- Sprite AABB collision system
- Entity composition pattern (TmxObject → `Activator.CreateInstance` → entity list)

### Risks

| Risk | Severity | Notes |
|---|---|---|
| MonoGame 3.0 → 3.8 API breakage | High | `Storage`, `GamerServices` removal requires code surgery in every file that imports them |
| `Arial.xnb` binary font | Medium | Must rebuild content pipeline; no raw `.ttf` in repo |
| TiledSharp API compatibility | Medium | The old local DLL API may not match current NuGet package |
| WASM binary size | Medium | Full .NET WASM runtime is 5–15 MB compressed; cold load latency on Vercel CDN may be noticeable |
| WebAssembly threading | Low | MonoGame WASM is single-threaded; game loop runs on main thread — this game is simple enough that this is not a problem |
| Audio autoplay policy | Medium | Browsers block audio until user interaction; need to defer `menuInstance.Play()` in `Initialize()` |
| Vercel build | Low | WASM output is static files (HTML + JS + WASM) — deploy as static site, no server runtime needed |

### Pass criteria for spike

- Project builds against MonoGame 3.8.1 SDK-style .csproj (no WindowsGL references)
- Game renders a single sprite in a browser tab via WASM
- TMX level loads and at least one tile draws

### Rough effort estimate

**Large** — 2–4 weeks minimum for a working browser build; blocked by toolchain upgrade before any browser work begins.

---

## Option 2: Partial Rewrite — TypeScript + Phaser 3 *(Recommended)*

### What gets rewritten vs reused

**Rewritten in TypeScript:**
- `Game1.cs` → Phaser `Scene` (game loop, map transitions, music management)
- `Controls.cs` → Phaser `KeyboardPlugin` + `GamepadPlugin` (direct replacement for `Keyboard.GetState()` / `GamePad.GetState()`)
- `Level.cs` → Phaser `Scene` + object layer management
- All `Objects/` and `Enemies/` classes → TypeScript classes mirroring the existing C# structure (1:1 translation is feasible — the logic is clean)
- `TextField.cs` → Phaser `Text` objects
- `Sprite.cs` base class → Phaser `GameObjects.Sprite` subclass or composition wrapper
- `Arial.xnb` font → web font or Phaser BitmapFont; remove XNB entirely

**Reused without change:**
- All `.tmx` level files — Phaser 3 has a built-in [Tiled map loader](https://newdocs.phaser.io/docs/3.60.0/Phaser.Tilemaps.TilemapLayer) supporting TMX and JSON. The TMX object layer (used by `Activator.CreateInstance` in `Game1.LoadMap`) maps directly to Phaser object layer factories.
- All PNG texture assets
- All WAV audio files (Web Audio API / Phaser Sound; WAVs are browser-supported)
- Level design and game rules

### Suggested target stack

- **Runtime:** TypeScript compiled to ES modules (Vite or Webpack)
- **Renderer:** Phaser 3 (WebGL primary, Canvas fallback) — replaces MonoGame `SpriteBatch` + OpenTK
- **Level format:** Phaser 3 Tiled plugin reads `.tmx` directly
- **Audio:** Phaser `SoundManager` → Web Audio API backend
- **Input:** Phaser `KeyboardPlugin` + `GamepadPlugin`
- **Deploy:** `vite build` → `dist/` static folder → Vercel static site (zero config)

### Why Phaser 3 over PixiJS

Phaser 3 has native Tiled TMX support, built-in physics, input, audio, and scene management. PixiJS is a lower-level renderer — it would require assembling all those pieces separately. Given the game already uses Tiled and has a scene/level structure, Phaser 3 is a significantly better fit.

### Risks

| Risk | Severity | Notes |
|---|---|---|
| C# → TypeScript translation effort | Medium | ~15 entity classes × ~100 lines each; logic is straightforward but requires careful translation |
| Phaser physics vs. custom AABB | Low | The existing collision system in `Sprite.Intersects()` is custom AABB; can port it directly or use Phaser Arcade Physics (simpler) |
| `Activator.CreateInstance` dynamic dispatch | Low | Replace with a factory map: `{ "Puddle.Block": (obj) => new Block(obj), ... }` |
| Gamepad support | Low | Phaser gamepad API mirrors the XNA `GamePad` API closely |
| Audio autoplay policy | Medium | Same browser constraint as Option 1; defer play until user interaction |
| TMX object type names | Low | TMX files reference `obj.Type` as the fully-qualified C# class name (e.g., `Puddle.Block`); factory map must handle these names |

### Pass criteria for spike

- Phaser 3 project loads `Level1-1.tmx`, renders tile layer and background
- Player sprite appears at start position from TMX `startX`/`startY` properties
- Left/right keyboard input moves player

### Rough effort estimate

**Medium** — 3–6 weeks for a feature-complete browser port. The C# code is clean and well-structured; TypeScript translation is mechanical, not creative.

---

## Option 3: Full Rewrite — Web-Native Stack

### What this means

Discard the C# codebase entirely. Rebuild Puddle from scratch in a web-native stack (TypeScript, canvas/WebGL, a game framework of choice). The game design, level layouts, and art assets are preserved; nothing else is.

### What can be salvaged

- All PNG art assets
- All WAV audio assets
- All `.tmx` level files (level geometry and object placement)
- Game design document / rules (implicit in existing code)
- README controls reference

### Risks

| Risk | Severity | Notes |
|---|---|---|
| Complete logic rewrite | High | All entity behavior, physics, and AI must be reimplemented from scratch |
| Behavioral regressions | High | Without the C# code as a reference, subtle game-feel details (jump arc, shot timing, hydration costs) will be guessed, not translated |
| Timeline | High | 2–4× longer than Option 2 |

### When this would be the right call

Only if the game design is changing significantly (new mechanics, different feel, new art direction) and the existing C# code would be more hindrance than help. For a faithful browser port, this option is wasteful.

### Rough effort estimate

**Large+** — 6–12 weeks. Not recommended for this use case.

---

## Recommendation

**Pursue Option 2: TypeScript + Phaser 3.**

The C# codebase is clean, modestly sized (~15 entity types, well-separated concerns), and the core logic translates mechanically to TypeScript. All level data (TMX), textures (PNG), and audio (WAV) transfer without conversion. Phaser 3's native Tiled support directly replaces TiledSharp. The Vercel deploy is a trivial `vite build` → static folder — no server runtime required.

Option 1 (MonoGame WASM) is technically possible but front-loaded with toolchain debt: the project uses MonoGame v3.0 with non-NuGet references, two removed namespaces (`Storage`, `GamerServices`), a compiled binary font (`Arial.xnb`), and an old local TiledSharp DLL. Clearing all of that before touching the browser target would consume the same effort as the TypeScript port, without the browser-native ergonomics. It also carries ongoing maintenance risk: MonoGame WASM support lags desktop and has limited community tooling.

Option 3 is not warranted here — the existing C# code is a precise blueprint, not a liability.

---

## Blockers and Open Questions

These must be resolved via spike before committing to a path:

1. **TMX object `Type` field format** — Spike required: Do TMX files store object types as fully-qualified C# class names (e.g., `Puddle.Block`) or short names (e.g., `Block`)? Affects factory map design in Option 2. *(Check a `.tmx` file's `<object type="...">` attributes.)*

2. **Phaser 3 TMX object layer loading** — Spike required: Confirm Phaser 3's Tiled loader correctly parses object layers with custom `type` and `properties` from these specific TMX files (v1.0 format). Phaser's Tiled loader targets JSON by default; TMX XML support should be verified.

3. **Audio autoplay gate** — All options: `Initialize()` calls `menuInstance.Play()` immediately. Browsers block audio until the first user gesture. A click-to-start screen or deferred audio init is required in any browser path.

4. **Arial.xnb** — The only compiled XNA content pipeline artifact in the repo. For Option 2: replace with a Phaser BitmapFont or `Phaser.GameObjects.Text` using a web-safe font. For Option 1: must regenerate via MonoGame Content Pipeline.

5. **TiledSharp `obj.Type` → `Activator.CreateInstance(t, obj)`** — The dynamic type dispatch in `Game1.LoadMap()` uses reflection. In TypeScript this becomes a factory registry. Verify all entity constructors accept a `TmxObjectGroup.TmxObject`-equivalent parameter before porting.

6. **Vercel build command** — Trivial for Option 2 (`vite build`, output `dist/`). For Option 1 (WASM), the MonoGame WASM build produces a Blazor host — confirm the static output from `dotnet publish` can be served from Vercel without a server runtime. Expected answer: yes, Blazor WASM publishes as static files.

7. **Gamepad API in browser** — The Gamepad Web API requires HTTPS and a user gesture before `navigator.getGamepads()` returns a device. Phaser handles this, but test with an actual gamepad during the Phaser spike.
