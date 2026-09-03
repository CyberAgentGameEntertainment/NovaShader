---
name: uloop-simulate-mouse-input
toolName: simulate-mouse-input
description: "Simulate Mouse.current input in PlayMode through Unity Input System. Use for gameplay mouse clicks, long-press (LongPress), movement delta (MoveDelta/SmoothDelta), or scroll. Use --dry-run to check what a Game View coordinate hits in 3D physics before clicking (works in EditMode; no Input System required). Use simulate-mouse-ui for UI. Requires the Input System package and Active Input Handling set to 'Input System Package (New)' or 'Both' (except --dry-run)."
---

# Task

Simulate mouse input via Input System in Unity PlayMode, or dry-run a Game View coordinate against 3D physics without injecting input.

## Workflow

1. When checking what a screenshot coordinate would hit in 3D physics before clicking, run `uloop simulate-mouse-input --dry-run --x <x> --y <y>` first (EditMode is fine; no Input System required)
2. Ensure Unity is in PlayMode (use `uloop control-play-mode --action Play` if not) before injecting real mouse input
3. For Click/LongPress: determine the target Game View input position from annotated `SimX`/`SimY`, raycast-grid `InputX`/`InputY`, or raw image pixels converted with `ScreenshotToInputFormula`
4. Execute the needed `uloop simulate-mouse-input` commands
5. Inspect the result with the lightest useful evidence: runtime state, logs, or a screenshot
6. When this input verifies a state transition, use Pause Point inspection from the section below as the standard frame proof
7. Report what happened and which evidence was used

Two rules while verifying:

- Do not touch the physical mouse or keyboard, and keep the OS pointer off the Unity window — real device input mixes into the same `Mouse.current` state this tool injects.
- Read the starting values you will assert against immediately before firing the input; a value measured earlier in the session may have changed.

## Tool Reference

```bash
uloop simulate-mouse-input --action <action> [options]
uloop simulate-mouse-input --dry-run --x <x> --y <y> [--layer-mask <mask>] [--max-distance <distance>]
```

### Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `--action` | enum | `Click` | `Click` - inject button press+release, `LongPress` - inject button hold for `--duration` seconds, `MoveDelta` - inject mouse delta (one-shot), `SmoothDelta` - inject mouse delta smoothly over `--duration` seconds, `Scroll` - inject scroll wheel |
| `--x` | number | `0` | Target X position in Game View pixels (origin: top-left). Used by Click, LongPress, and `--dry-run`. Use `AnnotatedElements[].SimX`, or raw image pixels converted with `ScreenshotToInputFormula`. |
| `--y` | number | `0` | Target Y position in Game View pixels (origin: top-left). Used by Click, LongPress, and `--dry-run`. Use `AnnotatedElements[].SimY`, or raw image pixels converted with `ScreenshotToInputFormula`. |
| `--button` | enum | `Left` | Mouse button: `Left`, `Right`, `Middle`. Used by Click and LongPress. |
| `--duration` | number | `0` | Hold duration for LongPress, or interpolation duration for SmoothDelta (seconds, max 30). For Click, 0 = one-shot tap. |
| `--delta-x` | number | `0` | Delta X in pixels for MoveDelta/SmoothDelta. Positive = right. |
| `--delta-y` | number | `0` | Delta Y in pixels for MoveDelta/SmoothDelta. Positive = up. |
| `--scroll-x` | number | `0` | Horizontal scroll delta for Scroll action. |
| `--scroll-y` | number | `0` | Vertical scroll delta for Scroll action. Positive = up, negative = down. Typically 120 per notch. |
| `--dry-run` | flag | - | Query 3D physics at `--x`/`--y` without injecting mouse input. Works in EditMode and without the Input System package. Skips PlayMode / Input System preflight. |
| `--layer-mask` | number | Unity default raycast layers | Physics layer mask used by the raycast. Effective only with `--dry-run`. |
| `--max-distance` | number | `1000` | Maximum raycast distance in world units. Effective only with `--dry-run`. |

### Actions

| Action | What it injects | Description |
|--------|----------------|-------------|
| `Click` | Mouse.current button press → release | Inject a button click so runtime logic detects `wasPressedThisFrame` |
| `LongPress` | Mouse.current button press → hold → release | Hold a button for `--duration` seconds |
| `MoveDelta` | Mouse.current.delta | Inject mouse movement delta one-shot |
| `SmoothDelta` | Mouse.current.delta (per-frame) | Inject mouse delta smoothly over `--duration` seconds (human-like camera pan) |
| `Scroll` | Mouse.current.scroll | Inject scroll wheel input |

### Pause Point Inspection (Standard for E2E)

For standard frame proof when this input drives a state transition, follow the `uloop-pause-point` skill — it covers line placement and interruption semantics. Tool-specific note: if `InterruptedByPausePoint: true`, Unity is paused; read `PressDeliveredToGame` before retrying. Clear inspection-only pause points (`uloop clear-pause-point --all`) before final validation.

## When to use this vs simulate-mouse-ui

All rows below assume the New Input System is installed.

| Scenario | Tool |
|----------|------|
| Click a Unity UI Button (IPointerClickHandler) | `simulate-mouse-ui` |
| Runtime logic reads `Mouse.current.leftButton` | `simulate-mouse-input` |
| Runtime logic reads right-click | `simulate-mouse-input --button Right` |
| Drag a UI slider | `simulate-mouse-ui --action Drag` |
| Runtime logic reads `Mouse.current.delta` | `simulate-mouse-input --action MoveDelta` |
| Runtime logic reads `Mouse.current.scroll` | `simulate-mouse-input --action Scroll` |

## Dry-run (3D physics hit check)

Use `--dry-run` to check what a top-left Game View coordinate hits in 3D physics before clicking.
This path does not require PlayMode or the Input System package.

- Requires an active `Camera.main`.
- Uses Unity Physics raycasts, not UI EventSystem raycasts.
- `--x` / `--y` use the same top-left Game View input coordinates as Click/LongPress.

```bash
# Check what is under a screenshot coordinate
uloop simulate-mouse-input --dry-run --x 960 --y 540

# Check only specific layers
uloop simulate-mouse-input --dry-run --x 960 --y 540 --layer-mask 1
```

## Examples

```bash
# Left-click at a representative Game View point (--button Right for right-click)
uloop simulate-mouse-input --action Click --x 400 --y 300

# Hold left-click for 2 seconds
uloop simulate-mouse-input --action LongPress --x 400 --y 300 --duration 2.0

# Scroll up (negative --scroll-y scrolls down)
uloop simulate-mouse-input --action Scroll --scroll-y 120

# Smooth camera pan right over 0.5 seconds
uloop simulate-mouse-input --action SmoothDelta --delta-x 300 --delta-y 0 --duration 0.5
```

## Coordinate System

`--x`/`--y` use top-left Game View coordinates: pass `AnnotatedElements[].SimX/SimY`
directly, convert raw screenshot pixels with `ScreenshotToInputFormula`, and never flip Y
in the caller (the tool converts internally). Conversion formula, Device Simulator
notes, and how the injected position reads back inside Unity are in
`references/output-and-coordinates.md`.

## Prerequisites

- For real mouse injection: Unity must be in **PlayMode**, and the **Input System package** (`com.unity.inputsystem`) must be installed; game code must read input via Input System API (e.g. `Mouse.current.leftButton.wasPressedThisFrame`).
- For `--dry-run`: an active `Camera.main` is required; PlayMode and the Input System package are not required.

## Output

The response reports `Success`, `Message`, `Action`, the target/injected coordinates,
`InterruptedByPausePoint`/`PausePointHits`, and — for `--dry-run` — the resolved camera
(`CameraName`/`CameraPath`; check these first when a `No physics hit` looks wrong) plus
the physics hit details. Field-by-field semantics are in
`references/output-and-coordinates.md`. Verify visual outcome with a follow-up
screenshot.
