---
name: uloop-simulate-keyboard
toolName: simulate-keyboard
description: "Simulate keyboard input in PlayMode through Unity Input System. Use for key presses, holds (via Press --duration or KeyDown/KeyUp), releases, and game controls such as WASD or Space. Requires the Input System package (com.unity.inputsystem)."
---

# Task

Simulate keyboard input on Unity PlayMode.

## Workflow

1. Ensure Unity is in PlayMode (use `uloop control-play-mode --action Play` if not)
2. Execute the needed `uloop simulate-keyboard` commands
3. Inspect the result with the lightest useful evidence: runtime state, logs, or a screenshot
4. If exact-frame proof would reduce uncertainty, treat Pause Point inspection as an optional follow-up using the section below
5. Report what happened and which evidence was used

## Tool Reference

```bash
uloop simulate-keyboard --action <Press|KeyDown|KeyUp> --key <key> [options]
uloop simulate-keyboard --action ReleaseAll
```

### Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `--action` | enum | `Press` | `Press` - one-shot key tap (Down then Up), `KeyDown` - hold key down, `KeyUp` - release held key, `ReleaseAll` - force-release every tracked and device-pressed key (allowed while PlayMode is paused; use after a pause-point interruption leaves key state inconsistent) |
| `--key` | string | (required except `ReleaseAll`) | Key name matching Input System Key enum (e.g. `W`, `Space`, `LeftShift`, `A`, `Enter`). Case-insensitive. Digit keys use `Digit0`-`Digit9` or `Numpad0`-`Numpad9`, not bare `0`-`9`. Not used by `ReleaseAll`. |
| `--duration` | number | `0` | Hold duration in seconds for Press action (0 = one-shot tap, max 30). Ignored by KeyDown/KeyUp/ReleaseAll. |

### Actions

| Action | Behavior | Use Case |
|--------|----------|----------|
| `Press` | KeyDown → wait → KeyUp | One-shot tap (jump, use item) |
| `KeyDown` | KeyDown only (held until KeyUp) | Start continuous movement, hold sprint |
| `KeyUp` | KeyUp only (release held key) | Stop movement, release sprint |
| `ReleaseAll` | Injects a release for every tracked and device-pressed key and resets the hold tracker; confirm the device via `ReleasedKeyStates` | Recover a clean keyboard state after a pause-point interruption |

There is no separate hold action: to hold a key, use `Press --duration <seconds>` (fixed-time hold) or `KeyDown` followed later by `KeyUp` (open-ended hold).

Use `Press` for edge-triggered keyboard code such as `Keyboard.current.spaceKey.wasPressedThisFrame`.
`KeyDown` emits one initial press edge, then only keeps the key held. It does not keep `wasPressedThisFrame` true while the key remains held.
If a successful `Press` or `KeyDown` leaves `Keyboard.current.<key>.isPressed` true but runtime state does not change, do not immediately rewrite the user's runtime code to `isPressed`. First verify that the target component is active during the command, that it polls input in the configured Input System update phase, and that a missed `KeyDown` edge is followed by `KeyUp` before retrying.

`ReleaseAll` is a recovery action, not part of normal gameplay simulation: after a pause-point interruption, bookkeeping and the Input System device can disagree, or a stale press latch can keep `isPressed` true after resume. `ReleaseAll` resets the tracker and injects a release for every such key; it works while Unity is still paused and does not clear pause-point captures. `Success: true` alone is not proof the device is clean: read `ReleasedKeyStates` in the response, and for any entry with `DeviceIsPressedAfterRelease: true` resume PlayMode (the deferred latch sync runs on the next gameplay input update) and re-check with `pause-point-status`, a log, or `Keyboard.current.<key>.isPressed` via `execute-dynamic-code` before sending the next key. For ordinary releases during gameplay simulation, keep using `KeyUp`.

### Pause Point Inspection (Standard for E2E)

For standard frame proof when this input drives a state transition, follow the `uloop-pause-point` skill — it covers line placement and interruption semantics. Tool-specific note: if `InterruptedByPausePoint: true`, Unity is paused and input bookkeeping was safely released; `PressEdgeObserved` is still reported on pause-point interruptions. Interruption detection covers the whole press lifetime: a pause landing while `Press` is holding the key (during the duration wait) also returns promptly with `InterruptedByPausePoint: true`, and the pause takes precedence even when the requested duration had already elapsed — treat such a response as the pause reporting in, not as a delivery failure. Clear inspection-only pause points (`uloop clear-pause-point --all`) before final validation. If a later key action still reports inconsistent state after an interruption, recover with `--action ReleaseAll` instead of retrying `KeyUp`.

### KeyDown/KeyUp Rules

- `KeyDown` fails if the key is already held
- `KeyUp` fails if the key is not currently held
- Multiple keys can be held simultaneously (e.g. W + LeftShift for sprint)
- All held keys are automatically released when PlayMode exits
- To hold a key for a fixed duration, prefer `--action Press --duration <seconds>` (one-shot, blocks until release). For multi-key holds (e.g. Shift+W), issue separate `KeyDown` calls, then `sleep <seconds>` between them and the `KeyUp` calls.

## Examples

```bash
# One-shot key press
uloop simulate-keyboard --action Press --key W

# One-shot action key
uloop simulate-keyboard --action Press --key Space

# Hold a key for 2 seconds
uloop simulate-keyboard --action Press --key W --duration 2.0

# Hold two keys, then release them
uloop simulate-keyboard --action KeyDown --key LeftShift
uloop simulate-keyboard --action KeyDown --key W
uloop screenshot --capture-mode rendering
uloop simulate-keyboard --action KeyUp --key W
uloop simulate-keyboard --action KeyUp --key LeftShift

# Recover a clean keyboard state (works while paused; e.g. after a pause-point interruption)
uloop simulate-keyboard --action ReleaseAll
```

## Output

The response reports `Success`, `Message`, `Action`, and `KeyName`, plus the fields that
gate how to read a run: `PressEdgeObserved` (`false` means gameplay polling most likely
missed the edge — read the `PressEdge*` diagnostics before retrying, and check
`pause-point-status` first when a single-shot marker is armed), `Warning` (set when the
press edge was missed while the Unity Editor is unfocused — run `uloop focus-window`
before retrying), `InterruptedByPausePoint`/`PausePointHits`, and the `ReleaseAll` /
key-state readback fields. Field-by-field semantics and the `PressEdge*` diagnostics
table are in `references/output.md` beside this skill — read it before retrying any
input whose response looks inconsistent.

## Prerequisites

- Unity must be in **PlayMode**. `Press`/`KeyDown`/`KeyUp` additionally require PlayMode to be unpaused; `ReleaseAll` is allowed while paused.
- **Input System package** (`com.unity.inputsystem`) must be installed; this tool only works with the New Input System.
- Game code must read input via Input System API (e.g. `Keyboard.current[Key.W].isPressed`), not legacy `Input.GetKey()`
