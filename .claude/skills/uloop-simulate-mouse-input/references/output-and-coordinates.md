# simulate-mouse-input Output Fields and Coordinates

## Output

Returns JSON with:

- `Success`: Whether the operation succeeded
- `Message`: Status message
- `Action`: Echoes which action was executed (`Click`, `LongPress`, `MoveDelta`, `SmoothDelta`, or `Scroll`); unused for `--dry-run`
- `Button`: Which button was used (nullable string; populated for `Click` / `LongPress`, null otherwise)
- `PositionX` / `PositionY`: Target top-left Game View coordinates (nullable float; populated for `Click` / `LongPress`)
- `CameraName` / `CameraPath`: Camera that `Camera.main` resolved to for `--dry-run` (reported on both hit and no-hit). When a `No physics hit` result looks wrong, check these first — another camera carrying the `MainCamera` tag can silently win `Camera.main` resolution
- `Hit`: Whether physics hit anything (`--dry-run` only)
- `HitGameObjectName` / `HitGameObjectPath`: Hit object identity when `Hit` is true
- `HitLayer` / `HitLayerName`: Hit object layer when `Hit` is true
- `Distance`, `HitPointX/Y/Z`, `HitNormalX/Y/Z`: Hit details when `Hit` is true
- `InputCoordinateSystem`: `"top-left-game-view"` for click/long-press/dry-run coordinates
- `UnityCoordinateSystem`: `"bottom-left-game-view"` for the injected `Mouse.current.position` (and dry-run conversion)
- `GameViewWidth` / `GameViewHeight`: Game View size used for conversion
- `InputPositionX` / `InputPositionY`: Coordinates received from the caller
- `InjectedUnityPositionX` / `InjectedUnityPositionY`: Coordinates injected into `Mouse.current.position` (or used for dry-run ScreenPointToRay)
- `CoordinateConversionFormula`: Conversion formula used by the tool
- `InterruptedByPausePoint` / `PausePointId` / `PausePointHitCount` / `PausePointHits`: Pause-point interruption info (all nullable except the boolean). `PausePointHits` lists every marker hit during this input in hit order; `PausePointId` only names the latest one. See the Pause Point Inspection section in SKILL.md
- `PressDeliveredToGame` (boolean, nullable): Set only when a `Click` / `LongPress` was interrupted by a pause point. `true`: the Input System processed the press edge in a gameplay update before the pause, so game code polling that frame observed it and the world state may already have changed (for example a block was mined). Do not retry; re-check the affected state and `pause-point-status`. `false`: the queued edge was discarded before any gameplay update, the game never observed a press, and a retry after resume is safe. `null` for other actions and uninterrupted responses

Verify visual outcome with a follow-up screenshot.

## Coordinate System

- `--x` / `--y` use **top-left Game View coordinates**.
- Raw image pixels from `uloop screenshot --capture-mode rendering` must be converted with `ScreenshotToInputFormula`.
- `AnnotatedElements[].SimX/SimY` can be passed directly to this tool.
- Do not flip Y in the caller. The tool converts internally for Unity Input System:

```text
unity_x = input_x
unity_y = gameViewHeight - input_y
```

- `Mouse.current.position` uses bottom-left Unity coordinates, so the value read inside Unity may show the converted Y.
- Device Simulator play view is supported. Prefer rendering-mode screenshots for coordinates; they match the simulated device resolution, not Simulator chrome scale.
