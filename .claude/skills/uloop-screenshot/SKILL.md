---
name: uloop-screenshot
toolName: screenshot
description: "Capture Unity Editor windows or Game View rendering as PNG. Use for visual checks, debugging, documentation, or annotated UI element coordinates."
---

# uloop screenshot

Take a screenshot of any Unity EditorWindow by name and save as PNG.

## Usage

```bash
uloop screenshot [--window-name <name>] [--resolution-scale <scale>] [--match-mode <mode>] [--capture-mode <mode>] [--annotate-elements] [--annotate-raycast-grid] [--raycast-layer-mask <layers>] [--elements-only] [--output-directory <path>]
```

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `--window-name` | string | `Game` | Window name to capture (for example `Game`, `Scene`, `Console`, `Inspector`). Ignored when the resolved capture mode is rendering (auto during Play Mode resolves to rendering). When the Game tab is Device Simulator and the title is Simulator, default Game falls back to Simulator. |
| `--resolution-scale` | number | `1.0` | Resolution scale (0.1 to 1.0) |
| `--match-mode` | enum | `exact` | Window name matching mode: `exact`, `prefix`, or `contains`. Ignored when the resolved capture mode is rendering (auto during Play Mode resolves to rendering). |
| `--capture-mode` | enum | `auto` | `auto` - rendering in PlayMode and window otherwise, `window` - capture EditorWindow including toolbar, `rendering` - capture game rendering only (PlayMode required), `GameView` - alias for `rendering`. Rendering screenshots return `ScreenshotToInputFormula` for converting raw image pixels before calling simulate-mouse-input (including `--dry-run`) or simulate-mouse-ui. |
| `--output-directory` | string | `""` | Output directory path for saving screenshots. When empty, uses default path (.uloop/outputs/Screenshots/). Accepts absolute paths. |
| `--annotate-elements` | flag | - | Annotate interactive UI elements with index labels and interaction hints (A / CLICK, B / DRAG, ...). The response includes an `AnnotatedElements` array with element metadata sorted by `SortingOrder` then `SiblingIndex` (an approximation of z-order, not the guaranteed draw order). Only works when the resolved capture mode is rendering (auto during Play Mode resolves to rendering) in PlayMode. |
| `--annotate-raycast-grid` | flag | - | Annotate clustered 3D physics collider candidates as `PhysicsCollider` entries in `AnnotatedElements`. Uses `Camera.main` visibility and the same top-left Game View coordinates as `simulate-mouse-input`. Only works when the resolved capture mode is rendering (auto during Play Mode resolves to rendering) in PlayMode. |
| `--raycast-layer-mask` | string | `""` | Comma-separated physics layer names to narrow which layers `--annotate-raycast-grid` clusters. Hits are limited to layers also visible to `Camera.main.cullingMask`. When omitted, clusters against `Physics.DefaultRaycastLayers`. |
| `--elements-only` | flag | - | Return only annotated element JSON without capturing a screenshot image. Requires `--annotate-elements` or `--annotate-raycast-grid`, and the resolved capture mode to be rendering (auto during Play Mode resolves to rendering) in PlayMode. |

## Match Modes

| Mode | Description | Example |
|------|-------------|---------|
| `exact` | Window name must match exactly (case-insensitive) | "Project" matches "Project" only |
| `prefix` | Window name must start with the input | "Project" matches "Project" and "Project Settings" |
| `contains` | Window name must contain the input anywhere | "set" matches "Project Settings" |

## Window Name

The window name is the text displayed in the window's title bar (tab). Common names: Game, Simulator, Scene, Console, Inspector, Project, Hierarchy, Animation, Animator, Profiler. Custom EditorWindow titles are also supported.

## Device Simulator

- Prefer `--capture-mode rendering` for the annotate → click flow. It works with both the normal Game view and Device Simulator, and coordinates match `simulate-mouse-ui` / `simulate-mouse-input` (including `--dry-run`).
- `--capture-mode window` captures Editor chrome (toolbar/borders). With Device Simulator as the play view, default `--window-name Game` falls back to `Simulator` when no Game tab exists; you can also pass `--window-name Simulator`.
- Simulator Fit to Screen / Scale / Safe Area overlays are chrome-only and do not change rendering-mode input coordinates. After device rotation, re-run annotate before clicking: rotation changes the Game View size, so every earlier `AnnotatedElements` entry (bounds, `SimX`/`SimY`, probe and occlusion results) is stale.

## Output

Returns JSON with:

- `ScreenshotCount`: Number of windows captured
- `ResolvedCaptureMode`: `"window"` or `"rendering"` — the mode actually used after resolving `auto`
- `Warning`: Appears for window captures taken while Play Mode is running with at least one image, and for any image captured while Play Mode is paused (UGUI/canvas content may show the frame rendered before the pause; run `uloop control-play-mode --action Step` once and capture again to refresh it).
- `Screenshots`: Array of screenshot info, each containing:
  - `ImagePath`: Absolute path to the saved PNG file. Empty (`""`) when `--elements-only` is used because no image file is written; `FileSizeBytes`, `Width`, and `Height` are then `0`, while `GameViewWidth`/`GameViewHeight` and `AnnotatedElements` still carry real values. When it is non-empty, always open the file named here — the output directory accumulates every past capture, so guessing the newest file with directory listing (`ls -t` or similar) can silently pick a stale screenshot from an earlier run.
  - `FileSizeBytes`: Size of the saved file in bytes
  - `Width`: Captured image width in pixels
  - `Height`: Captured image height in pixels
  - `ImageCoordinateSystem`: `"top-left-game-view"` or `"top-left-window"`
  - `ResolutionScale`: Resolution scale used for capture
  - `ImageToInputOffsetY`: Y offset used for top-left-game-view coordinate conversion
  - `GameViewWidth`, `GameViewHeight`: Game View size in input coordinates at capture time — the basis for `SimX`/`SimY` and for the Y flip inside `simulate-mouse-input`. Populated for rendering captures, including `--elements-only`
  - `ScreenshotToInputFormula`: Formula converting raw image pixels to simulate-mouse input coordinates
  - `AnnotatedElements`: Array of annotated UI element metadata. Empty unless `--annotate-elements` or `--annotate-raycast-grid` is used.
  - `RaycastLayerSummaries` / `RaycastLayerNamesChecked`: Physics-layer diagnostics populated when `--annotate-raycast-grid` is used.

For `AnnotatedElements` / `RaycastLayerSummaries` fields and gameView coordinate conversion, read [references/annotated-elements.md](references/annotated-elements.md) before using screenshot coordinates with mouse simulation tools.

When multiple windows match (e.g., multiple Inspector windows or when using `contains` mode), all matching windows are captured with numbered filenames (e.g., `Inspector_1_*.png`, `Inspector_2_*.png`).

## Notes

- Use `uloop focus-window` first if needed
- Target window must be open in Unity Editor
- Window name matching is always case-insensitive
