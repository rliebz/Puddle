# Puddle — Repository Technical Map

> Generated: 2026-05-03T17:14:09-07:00  
> Author: Ripley (Engine Dev)  
> Purpose: Foundation for browser-port feasibility assessment  
> Branch: squad/web-port-spike

---

## 1. Entry Points

**`Program.cs`**
- Namespace: `Puddle`
- Guarded by `#if WINDOWS || LINUX` — there is no `#if BROWSER` or cross-platform entrypoint for web targets.
- Static entry: `static void Main()` annotated `[STAThread]`
- Instantiates `Game1` and calls `game.Run()` — standard MonoGame bootstrap via `Microsoft.Xna.Framework.Game.Run()`

**Bootstrap sequence:**
```
Program.Main()
  └─ new Game1()                    // constructor: creates GraphicsDeviceManager, sets Content.RootDirectory
       └─ game.Run()                // MonoGame internal: creates SDL window, enters event loop
            ├─ Game1.Initialize()   // loads initial TMX map, creates Player, Level, Controls, loads audio
            ├─ Game1.LoadContent()  // creates SpriteBatch, calls player1.LoadContent(), items/enemies LoadContent()
            └─ [game loop starts]
```

---

## 2. Game Loop

**Class:** `Game1 : Microsoft.Xna.Framework.Game`  
**File:** `Game1.cs`

MonoGame's default loop is used — `IsFixedTimeStep` is **not explicitly set**, so it defaults to `true` at 60 Hz (MonoGame default: 16.67ms per tick). There is no `TargetElapsedTime` override in the code.

### `Update(GameTime gameTime)` — `Game1.cs:224`
- Checks `player1.newMap != null` → sets `loadingMap = true`
- If `loadingMap`: counts down `newMapTimer` (1.5s), then calls `LoadMap()`
- `pauseControls.Update(level)` — always runs (even while loading/paused)
- Pause toggle: `pauseControls.onPress(Keys.Enter, Buttons.Start)`
- Escape while paused: returns player to level select (`player1.newMap = "Select"`)
- If not paused:
  - `controls.Update(level)` — snapshot keyboard/gamepad state
  - `player1.Update(controls, level, gameTime)` — player physics and input
  - `level.Update()` — updates projectiles, enemies, items, then prunes destroyed objects
  - `base.Update(gameTime)` — MonoGame internals

### `Draw(GameTime gameTime)` — `Game1.cs:275`
- Single `spriteBatch.Begin()` / `spriteBatch.End()` block — **no layered batches**
- If `loadingMap`: black screen + `TextField` with level name
- If running:
  1. Background stretched to `PreferredBackBufferWidth × PreferredBackBufferHeight`
  2. Intro slides (if on Menu level)
  3. `level.Draw(spriteBatch)` — items → projectiles → enemies → player
  4. Pause screen overlay (if paused)
  5. HUD message text (bottom-left)
- `base.Draw(gameTime)` — MonoGame internals

**Timing note:** `elapsed` is computed as `(float)gameTime.ElapsedGameTime.TotalSeconds` in the loading screen countdown. Player physics uses integer counters (`count++` in `Level.Update()`) not delta-time — this is **frame-rate coupled**.

---

## 3. Rendering Pipeline

**SpriteBatch:** Standard XNA/MonoGame `SpriteBatch`  
**Mode:** Single `Begin()`/`End()` per frame, no sort mode or blend mode specified (defaults to `SpriteSortMode.Deferred`, `BlendState.AlphaBlend`)

**Draw order (back to front):**
1. Background image (stretched rectangle, no camera transform)
2. Intro slide image (720×540 hardcoded)
3. `Level.Draw()`:
   - `items` (sorted by `Sprite.depth` at load time via `List.Sort(CompareTo)`)
   - `projectiles`
   - `enemies`
   - `player`
4. Pause overlay image (720×540 hardcoded, centered vertically)
5. HUD text (`TextField`)

**Camera / Viewport:**
- **No camera** — the viewport is the full backbuffer. No scroll, no transform matrix passed to `SpriteBatch.Begin()`.
- Resolution is **dynamic**: read from TMX map at load time:
  ```csharp
  graphics.PreferredBackBufferWidth = map.Width * map.TileWidth;
  graphics.PreferredBackBufferHeight = map.Height * map.TileHeight;
  ```
  All levels are 22×22 tiles at 32×32px = **704×704 px** (based on Level1-1.tmx). Menu/UI maps may differ.
- Hardcoded intro/pause dimensions: `720×540` — **does not match the computed backbuffer size**. This is a pre-existing inconsistency.

**`Sprite.Draw()` — `Objects/Sprite.cs:160`**
- Draws source rect `(frameIndexX, frameIndexY, frameWidth, frameHeight)` from sprite sheet
- Supports horizontal flip via `SpriteEffects.FlipHorizontally`
- Optional debug hitbox overlay (navy rectangle)
- Optional text overlay via `TextField`

**Font rendering:** `SpriteFont` loaded from `Content/Arial.xnb` (pre-compiled XNB, MonoGame Content Pipeline format)

---

## 4. Input Handling

**File:** `Controls.cs`  
**Class:** `Controls`

Stores previous and current frame snapshots of both keyboard and gamepad state:
```csharp
KeyboardState kb, kbo;    // current, previous
GamePadState  gp, gpo;    // current, previous — PlayerIndex.One only
```

**`Update(Level level)`** — called each frame, rolls `kb → kbo`, `gp → gpo`, then reads fresh state via:
- `Keyboard.GetState()` — MonoGame abstraction over SDL keyboard events
- `GamePad.GetState(PlayerIndex.One)` — MonoGame abstraction over SDL gamepad/XInput

**Helper methods (each takes a `Keys` + `Buttons` pair):**
| Method | Behavior |
|---|---|
| `isPressed(key, btn)` | held this frame |
| `onPress(key, btn)` | pressed this frame (rising edge) |
| `onRelease(key, btn)` | released this frame (falling edge) |
| `isHeld(key, btn)` | held both this and previous frame |

**Two `Controls` instances in `Game1`:**
- `controls` — gameplay input (used in `player1.Update()`)
- `pauseControls` — always-active pause/escape (runs even when paused or loading)

**Key bindings (from README):**
| Action | Keyboard | Gamepad |
|---|---|---|
| Pause | Enter | Start |
| Level Select | Esc (while paused) | Back (while paused) |
| Move | ←/→ Arrow | D-Pad Left/Right |
| Puddle | ↓ Arrow | D-Pad Down |
| Aim Up | ↑ Arrow | D-Pad Up |
| Jump | S | A |
| Shoot | D | Right Bumper |
| Charge Shot | X | Right Trigger |

**SDL layer:** MonoGame 3.0 uses `Tao.Sdl.dll` as its SDL binding. Input does **not** bypass MonoGame — all input goes through `Microsoft.Xna.Framework.Input` namespace. No raw SDL event polling in game code.

**Mouse:** Not used.

---

## 5. Asset / Content Pipeline

### Directory Structure
```
Content/
├── Arial.xnb                  # Pre-compiled SpriteFont (XNB format, MonoGame Content Pipeline)
├── background.png             # Background tileset
├── blank.png                  # 1×1 white debug/hitbox texture
├── *.png                      # Object/item sprite sheets (32px tiles)
├── PC/                        # Player sprite sheets (stand, walk, jump, puddle)
│   ├── puddle.png
│   ├── stand.png
│   ├── walk.png
│   └── jump.png
├── Enemies/                   # Enemy sprite sheets
│   ├── bird.png, face.png, hand.png, rat.png, spikeball.png
├── Slides/                    # Full-screen images (intro slides, pause screens, credits)
│   ├── Slide1–5.png
│   ├── pause0–3.png
│   └── credits.png
├── Sounds/                    # All audio (WAV format)
│   ├── InGame.wav, Boss.wav, Menu.wav   # Music tracks
│   └── BlockFall, Jump, Death, Shot1–4, Checkpoint, Powerup, ...wav  # SFX
└── Levels/                    # Tiled map files (TMX XML)
    ├── LevelMenu.tmx, LevelSelect.tmx, LevelWin.tmx, LevelBoss.tmx
    ├── Level1-1.tmx … Level1-3.tmx
    ├── Level2-1.tmx … Level2-4.tmx
    └── Level3-1.tmx … Level3-3.tmx  (+ Level3dd, LevelBlah, LevelWhat)
```

### Asset Loading

| Asset Type | Load Method | Notes |
|---|---|---|
| PNG textures | `ContentManager.Load<Texture2D>(path)` | Path relative to `Content/`, no extension |
| SpriteFont | `ContentManager.Load<SpriteFont>("Arial")` | Must be XNB (pre-compiled) |
| Audio (music) | `ContentManager.Load<SoundEffect>(path)` | `.wav`, used as looping `SoundEffectInstance` |
| Audio (SFX) | `ContentManager.Load<SoundEffect>(path)` | `.wav`, per-sprite via `soundFiles` list |
| TMX maps | `new TmxMap(path)` | Raw file path, not via ContentManager; loaded synchronously on map transition |

**Key detail:** TMX files are loaded with **raw `System.IO` file access** via TiledSharp — `new TmxMap("Content/Levels/Level{name}.tmx")`. This is **not** routed through MonoGame's ContentManager. This means browser ports cannot use ContentManager's virtual filesystem for maps without modification.

**Content path constant:**
```csharp
const string LEVEL_PATH = "Content/Levels/Level{0}.tmx";
```

**Object instantiation from TMX:** `Game1.LoadMap()` uses reflection to create game objects:
```csharp
Type t = Type.GetType(obj.Type);  // obj.Type = class name string from TMX
object item = Activator.CreateInstance(t, obj);
```
This requires all TMX object type strings to resolve to full `Puddle.ClassName` names at runtime. The TMX's `<objectgroup>` defines the type string.

---

## 6. Level / World Structure

**File:** `Level.cs`  
**Class:** `Level`

### Fields
```csharp
public List<Enemy> enemies;
public List<Sprite> projectiles;
public List<Sprite> items;
public Player player;
public string message;        // HUD message text
public string name;           // Level name slug (e.g. "1-1", "Menu", "Select")
public int enterLives;        // Lives at level entry (for rollback on death)
public Dictionary<string, bool> enterPowerUps;
public ContentManager content;
public double gravity = .35;
public int maxFallSpeed = 10;
public int ground = 900;      // fallback ground Y (likely unused given collision system)
```

### Level Lifecycle
1. `Game1.Initialize()` — loads `LevelMenu.tmx`, creates `Level(player1, "menu", Content)`
2. **Level transition** triggered by `player1.newMap != null` (set by `NextLevel`, `Checkpoint`, or pause escape)
3. `Game1.Update()` detects `newMap != null`, sets `loadingMap = true`, starts `newMapTimer` countdown (1.5s)
4. After timer: `Game1.LoadMap(name)` is called
   - Parses new `.tmx` via `new TmxMap(...)`
   - Resizes backbuffer to map dimensions
   - Creates new `Level` object (or restores `levelSelect` snapshot)
   - Iterates `TmxObjectGroup` → reflective `Activator.CreateInstance` for each object
   - Sorts `level.items` by `Sprite.depth`
   - Calls `LoadContent()` on all new items/enemies
5. `Level.Update()` — per-frame: updates projectiles → enemies → items → prunes destroyed

### Level Select State Preservation
```csharp
if (previousMap.Equals("Select"))
    levelSelect = new Level(level);  // snapshot saved
// ...
if (name.Equals("Select") && levelSelect != null)
    level = levelSelect;             // snapshot restored
```

### Tiled Map Integration
- TMX format version 1.0, orthogonal, 22×22 tiles, 32×32px tile size
- Tilesets reference PNG files via relative path (e.g., `../background.png`)
- `<properties>` on the map define `startX`, `startY` (and `startX0`, `startXBoss`, etc. on Select map)
- `<objectgroup>` defines all game objects with `Type` attribute matching a C# class name
- Tile layers are **not rendered by Level.cs** — the TMX tile layers appear to define static visual tiles, but there is no tile-layer rendering code visible in Level.Draw() or Game1.Draw(). The background PNG is drawn as a stretched image. **Tile-layer rendering is either absent or handled implicitly through the background texture.** This warrants investigation.

---

## 7. Dependencies

### From `Puddle.csproj`

| Assembly | Version / Source | Notes |
|---|---|---|
| `MonoGame.Framework` | v3.0 | Via `$(MSBuildExtensionsPath)\..\MonoGame\v3.0\Assemblies\WindowsGL\` — **Windows path** |
| `OpenTK` | v3.0 (bundled with MonoGame 3.0) | OpenGL/OpenAL bindings |
| `Lidgren.Network` | v3.0 (bundled with MonoGame 3.0) | Networking (unused in game code) |
| `Tao.Sdl` | v3.0 (bundled with MonoGame 3.0) | SDL1/SDL2 C# bindings |
| `TiledSharp` | Version=1.0.5191.24202 | Local `.\TiledSharp.dll` |
| `System`, `System.Xml` | .NET Framework 4.5 | See `app.config` |

### From `app.config`
```xml
<supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.5"/>
```
Target: **.NET Framework 4.5** — **not .NET Core / .NET 5+**. This is a hard constraint.

### Bundled Native Libraries
- `SDL.dll` — included as `<EmbeddedResource>`, copied to output. Version not embedded in csproj metadata; file exists at repo root.
- `TiledSharp.dll` — local DLL, not NuGet. Version: `1.0.5191.24202` from csproj reference.

### Project Format
- Legacy MSBuild `.csproj` format (ToolsVersion 4.0, not SDK-style)
- `OutputType: WinExe`, `Platform: x86`, output to `bin\WindowsGL\Debug\`
- No MonoGame NuGet packages — references local MonoGame installation via absolute Windows path hinting

---

## 8. Build & Run (Mac)

⚠️ **This project was built for Windows + Visual Studio / MonoGame 3.0 installer.** The `.csproj` references DLL paths via `$(MSBuildExtensionsPath)\..\MonoGame\v3.0\Assemblies\WindowsGL\` which is a Windows-only path. **No verified Mac build path exists in the current project file.**

### What the README says
> Install MonoGame for Xamarin (OSX) — no specific commands given.

### Current State Assessment
The project:
1. Uses `.NET Framework 4.5` (not .NET Core) — requires Mono on Mac
2. Uses a legacy non-SDK `.csproj` — requires MSBuild (Mono's `xbuild` or `msbuild` from Mono)
3. Has Windows-only DLL hint paths — these will fail to resolve on Mac
4. Has `SDL.dll` (Windows DLL) embedded — Mac needs `libSDL2.dylib`

### Theoretical Mac commands (unverified — project likely needs .csproj modification):
```bash
# Requires: Mono (brew install mono), MonoGame 3.0 for Mac installed
# MonoGame 3.0 Mac assemblies typically at: /Library/Frameworks/MonoGame.framework/v3.0/Assemblies/MacOS/

# Restore / build
xbuild Puddle.sln /p:Configuration=Debug

# Or with msbuild from Mono:
msbuild Puddle.sln /p:Configuration=Debug

# Run output (after successful build):
mono bin/WindowsGL/Debug/GameName1.exe
```

### Likely required .csproj changes for Mac:
- Change `$(MSBuildExtensionsPath)\..\MonoGame\v3.0\Assemblies\WindowsGL\` to Mac MonoGame path
- Change `OutputType` from `WinExe` to `Exe`
- Replace `SDL.dll` embedded resource with Mac dylib approach

### .NET SDK note
**Not applicable** — this is .NET Framework 4.5, not .NET SDK. `dotnet` CLI **cannot build this project** without migrating to SDK-style csproj targeting `net45` with Mono/Windows, or migrating to MonoGame 3.8+ NuGet packages targeting `net6.0+`.

---

## 9. Platform-Specific Concerns (Browser Port)

### 🔴 Blockers

| Concern | Detail |
|---|---|
| **`#if WINDOWS \|\| LINUX` entrypoint** | `Program.Main()` does not compile for a browser/WASM target. A new entrypoint is needed. |
| **.NET Framework 4.5** | Cannot target WebAssembly. Must migrate to .NET 6+ (e.g., Blazor WASM) or use Uno Platform / Blazor + MonoGame. |
| **MonoGame 3.0 (ancient)** | No WASM support. MonoGame 3.8.1+ has experimental WebGL via KNI/FNA, but requires SDK-style project migration. |
| **SDL.dll (native binary)** | SDL2 native library cannot run in a browser sandbox. Must be replaced with WebGL/Canvas equivalent. |
| **`Tao.Sdl.dll`** | Tao SDL bindings are a native interop layer — cannot run in WASM. |
| **`System.IO` file access in TiledSharp** | `new TmxMap("Content/Levels/...")` uses `File.Open` — synchronous filesystem I/O. Browsers require async fetch; this must be replaced. |
| **`ContentManager.Load<SoundEffect>()` with `.wav`** | MonoGame 3.0 audio via OpenAL (native). WASM requires Web Audio API. |
| **`Activator.CreateInstance` + `Type.GetType`** | Reflection-based object instantiation from TMX strings. Works in .NET but may require AOT-compatibility fixes for Blazor WASM (IL Linker strips types). |

### 🟡 Medium Complexity

| Concern | Detail |
|---|---|
| **`GraphicsDeviceManager` / `SpriteBatch`** | If migrating to MonoGame for WASM (via KNI), SpriteBatch API is preserved. If porting to a JS renderer, all `Sprite.Draw()` calls must be rewritten. |
| **`SoundEffect` / `SoundEffectInstance`** | Used for both music and SFX. Music is looped `SoundEffectInstance`. Need Web Audio API equivalent. |
| **`SpriteFont` / XNB font** | `Arial.xnb` is MonoGame Content Pipeline format. Not usable in browser without Content Pipeline tooling or conversion. |
| **Frame-rate coupling** | Physics/game logic uses integer `count++` ticks, not delta-time. At browser's `requestAnimationFrame` (variable rate), behavior will drift. Needs delta-time refactor. |
| **Window resize / backbuffer** | `graphics.PreferredBackBufferWidth/Height` set at runtime from TMX. Browser canvas resize requires different mechanism. |
| **`Microsoft.Xna.Framework.Storage` + `GamerServices`** | Imported but likely unused at runtime. Must be removed or stubbed for non-Windows targets. |

### 🟢 Portable / Low Risk

| Concern | Detail |
|---|---|
| **Game logic** | All physics, collision, state machines are pure C# integers/doubles — fully portable. |
| **Level data** | TMX files are plain XML — parseable in any environment once I/O is resolved. |
| **Sprite sheets** | Standard PNG files — directly usable in browser Canvas/WebGL. |
| **Input abstraction** | `Controls.cs` cleanly wraps input behind `isPressed/onPress/onRelease/isHeld`. Easy to re-implement with browser keyboard events. |
| **Collision system** | AABB in `Sprite.Intersects()` — pure math, zero platform dependency. |
| **Audio assets** | WAV files are universally supported in browsers via Web Audio API. |

---

## Appendix: File Index

| File | Role |
|---|---|
| `Program.cs` | Entry point (`#if WINDOWS\|\|LINUX`) |
| `Game1.cs` | Game loop, initialization, map loading, draw orchestration |
| `Level.cs` | Level state container, per-frame update, draw dispatch |
| `Controls.cs` | Input snapshot wrapper (keyboard + gamepad) |
| `TextField.cs` | SpriteFont text rendering helper |
| `Objects/Sprite.cs` | Abstract base: position, collision, draw, sound |
| `Objects/Player.cs` | Player state, physics, input consumption |
| `Objects/Block.cs` | Static solid tile |
| `Objects/Button.cs` | Activatable switch |
| `Objects/Cannon.cs` | Projectile emitter |
| `Objects/Checkpoint.cs` | Respawn point |
| `Objects/Egg.cs` | Collectible |
| `Objects/Fireball.cs` | Hazard projectile |
| `Objects/Geyser.cs` | Vertical hazard |
| `Objects/NextLevel.cs` | Level transition trigger |
| `Objects/Pipe.cs` | Teleport/transit object |
| `Objects/PowerShot.cs` | Charged projectile |
| `Objects/PowerUp.cs` | Ability unlock item |
| `Objects/Roller.cs` | Moving platform/hazard |
| `Objects/Shot.cs` | Basic projectile |
| `Enemies/Enemy.cs` | Enemy base class |
| `Enemies/Bird.cs` | Flying enemy |
| `Enemies/Dying.cs` | Death animation entity |
| `Enemies/Face.cs` | Boss-type enemy |
| `Enemies/Hand.cs` | Boss appendage |
| `Enemies/Rat.cs` | Ground enemy |
| `Enemies/SpikeBall.cs` | Rolling hazard enemy |
| `Puddle.csproj` | Legacy MSBuild project, .NET Framework 4.5, x86, WinExe |
| `app.config` | Runtime: .NET Framework v4.5 |
| `TiledSharp.dll` | Local DLL, v1.0.5191.24202 |
| `SDL.dll` | Native SDL2 Windows binary (EmbeddedResource) |
