---
name: uloop-pause-point
description: "Pauses Unity playback at any source file:line without editing code or recompiling, and returns a snapshot of the locals, parameters, and instance fields at that exact frame. Use for bug investigation, PlayMode/E2E verification, checking variable values at a specific frame, or confirming that a code path executed."
---

# uloop pause-point

A pause point captures locals at an exact frame without a source edit. Prefer it over sleeps or after-the-fact reads when input delivery, event ordering, or transition-frame fidelity matters.

## Quick Check

1. Enter PlayMode. If the game progresses on its own, pause it with `control-play-mode --action Pause`, arrange the scenario while paused, and add `--resume-play` in step 2.
2. Arm the marker, fire the input, and wait for the hit in one foreground command:

```bash
uloop enable-pause-point --file Assets/Scripts/Enemy.cs --line 42 --timeout-seconds 60 --await --trigger "simulate-keyboard --action Press --key Space"
# paused the game in step 1? add --resume-play, or the input never lands
```

3. Read `CapturedVariables` in the hit response first, then gather extra evidence while still paused (`execute-dynamic-code`, one screenshot).
4. A `single-shot` marker (the default) disarms after the hit; clear other modes with `uloop clear-pause-point`.

Only `CapturedVariables` is evidence of the values at the patched line. Before deviating, read `references/quick-check-template.md`.

## Parameters

Tables list Unity-accepted parameters plus clear's CLI-only `--file`/`--line`; other CLI-only flags (`--await`, `--trigger`, `--resume-play`, `--expect`, capture filters) are in the references.

### enable-pause-point

Enable a pause point so Unity pauses when that code path is reached, either by a named UloopPausePoint.Pause marker (Id) or by resolving a source file and line (File+Line)

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `--id` | string | - | Named pause point id passed to UloopPausePoint.Pause. Mutually exclusive with File/Line |
| `--file` | string | - | Project-relative source file path to patch a pause point into. Requires Line; mutually exclusive with Id |
| `--line` | integer | - | 1-based source line to resolve within File. Requires File; mutually exclusive with Id |
| `--timeout-seconds` | integer | `30` | Seconds before the enable request expires and stops pausing late hits |
| `--mode` | enum | `single-shot` | Capture mode: single-shot pauses once, continuous pauses on every hit, trace records hits without pausing |
| `--hit-when` | string | - | Conditional capture expression (`<name> <op> <literal>`). Only matching hits are captured; requires File and Line |
| `--max-history` | integer | `20` | Maximum number of captured hit frames to retain (1-100) |
| `--max-preview-elements` | integer | `10` | Maximum elements in a captured collection's preview (1-1000). Also caps previews in later pause-point-status responses; status cannot change it. |
| `--max-caller-frames` | integer | `2` | Maximum caller stack frames recorded per hit (0-8). 0 disables capture. Also caps later pause-point-status responses; status cannot change it. |
| `--method` | string | - | Optional method simple name or `Type.Method`. When set, `--line` resolves only inside matching methods |
| `--snapshot-timing` | enum | `pre-line` | pre-line captures before the resolved line runs; post-line captures after that line's statement finished, without arming the next line |

### clear-pause-point

Clear one or all named UloopPausePoint.Pause markers. The response field `ClearedCount` is the number of markers this call transitioned to Cleared: 0 or 1 for `--id`, and the number transitioned for `--all`. Auto-disarmed and expired markers still count as 1; the record stays readable via `pause-point-status` (`StatusBeforeClear` keeps the prior state). Clearing the marker that owns the current pause releases the Editor pause and lets the game consume any state you arranged while paused: arm the next marker first, then clear the old id, or re-arm it with `--await --resume-play` instead of clearing.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `--id` | string | - | Named pause point id to clear |
| `--file` | string | - | Project-relative source file of a file:line pause point. Requires --line; mutually exclusive with --id and --all |
| `--line` | integer | - | 1-based source line of a file:line pause point. Requires --file; mutually exclusive with --id and --all |
| `--all` | flag | - | Clear every non-cleared pause point marker (armed, auto-disarmed, or expired) |

### enable-watch

Register a C# expression to evaluate on each paused Play Mode step

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `--id` | string | - | Unique watch expression identifier |
| `--expression` | string | - | C# expression returning an object; UloopPausePoint.TryGetCapturedValue can read the latest raw capture |
| `--max-history` | integer | `20` | Maximum number of watch evaluations to retain (1-100) |

### get-watch-values

Show registered watch expression values and bounded evaluation history

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `--id` | string | - | Optional watch expression identifier; omit to return all watches |

### clear-watch

Clear one or all registered C# watch expressions

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `--id` | string | - | Watch expression identifier to clear |
| `--all` | flag | - | Clear every registered watch expression |

## Status, Timeouts, Hot Reload

`uloop pause-point-status` with no target lists every marker; inspect one with `--id "<file>:<line>"` or with `--file`/`--line` together (never both). `await-pause-point` and `clear-pause-point` take the same two forms; await always needs a target.

On a wait timeout, `PAUSE_POINT_EXPIRED`, or an enable failure, read `Error.Details.Hint` or the failure `ErrorCode`, then `RecommendedNextAction`. After hot reload, use `--method <Type.Method>` to constrain `--line`.

## Requirements & Safety

- On the automatic Debug-switch warning: the pause point is already armed - do not interrupt or ask mid-flow. The switch reverts on every Editor restart, so at the next stopping point propose `uloop set-code-optimization debug --startup` (session-only without `--startup`); only if the user approves. See the troubleshooting reference.

- Patches do not survive compiles or domain reloads, including a Play entry with Domain Reload enabled (the enable response warns) — re-enable afterwards. `uloop compile` during PlayMode also resets the session.
- Physics message methods, their helpers, and pre-bound delegates can miss hits on pre-existing GameObjects; the enable response warns where detectable.
- An `--id` marker waits on a hand-written `UloopPausePoint.Pause(id)` call; its hits record no `CapturedVariables`.
- For scripts under `Packages/`, pass the package-id path form (`Packages/<package-id>/...`); physical checkout paths do not resolve.

## Reference Guides

Read the one whose trigger matches:

- `references/quick-check-template.md` — full `--trigger`/`--await`/`--resume-play` loop, timeouts, hit fields.
- `references/captured-variables.md` — captures, name filters, `--expect` value forms, caller frames, raw values.
- `references/capture-modes-and-history.md` — modes, history, `--max-history`, `--hit-when`.
- `references/line-placement.md` — choosing the line: every-frame lines, held input, input+N frames.
- `references/watch-expressions.md` — watch rules.
- `references/condition-triggered-pause.md` — runtime-condition pauses.
- `references/fast-progressing-games.md` — freezing self-progressing games, `--resume-play`.
- `references/troubleshooting.md` — timeouts, missed hits, hot reload, failure codes.
