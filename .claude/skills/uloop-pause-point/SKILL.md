---
name: uloop-pause-point
description: "Pauses Unity playback at any source file:line without editing code or recompiling, and returns a snapshot of the locals, parameters, and instance fields at that exact frame. Use for bug investigation, PlayMode/E2E verification, checking variable values at a specific frame, or confirming that a code path executed."
---

# uloop pause-point

A pause point captures locals and branch reasons at an exact frame without a source edit. Use it instead of sleeps or after-the-fact reads when input delivery, event ordering, or transition-frame fidelity matters.

## Quick Check

1. Enter PlayMode. If the game progresses on its own, pause it with `control-play-mode --action Pause`, arrange the scenario while paused, and add `--resume-play` in step 2.
2. Arm the marker, fire the input, and wait for the hit in one foreground command:

```bash
uloop enable-pause-point --file Assets/Scripts/Enemy.cs --line 42 --timeout-seconds 60 --await --trigger "simulate-keyboard --action Press --key Space"
# paused the game in step 1? add --resume-play, or the input never lands
```

3. Read `CapturedVariables` in the hit response first, then gather extra evidence while still paused (`execute-dynamic-code`, one screenshot).
4. A `single-shot` marker (the default) disarms after the hit; clear other modes with `uloop clear-pause-point --id <marker-id>`.

A hit pauses Unity at the next frame boundary — the rest of that frame still runs. Only `CapturedVariables` is evidence of the values at the patched line. Before deviating from this template, read `references/quick-check-template.md`.

## Parameters

The tables list only parameters Unity accepts; CLI-only flags (`--await`, `--trigger`,
`--resume-play`, `--expect`, capture filters) are in the reference guides below.

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
| `--max-preview-elements` | integer | `10` | Maximum number of elements to include in a captured collection's preview (1-1000). The value set at enable time also caps the previews in every later pause-point-status response for that marker; status has no flag to change it. |
| `--max-caller-frames` | integer | `2` | Maximum number of caller stack frames to record on each hit (0-8). 0 disables capture (`CallerFrames` stays an empty array). The value set at enable time also caps every later pause-point-status response for that marker; status has no flag to change it. |
| `--method` | string | - | Optional method simple name or `Type.Method`. When set, `--line` resolves only inside matching methods |

### clear-pause-point

Clear one or all named UloopPausePoint.Pause markers. The response field `ClearedCount` is the number of markers this call transitioned to Cleared: 0 or 1 for `--id`, and the number transitioned for `--all`. Auto-disarmed and expired markers still count as 1; the record stays readable via `pause-point-status` (`StatusBeforeClear` keeps the prior state).

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `--id` | string | - | Named pause point id to clear |
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

`uloop pause-point-status` with no target lists every marker; inspect one with `--id "<file>:<line>"` or with `--file`/`--line` together (never both). `await-pause-point` takes the same two forms but always requires a target.

On a wait timeout or `PAUSE_POINT_EXPIRED`, read `Error.Details.Hint`, then `RecommendedNextAction`; see `references/troubleshooting.md` for diagnosis. After hot reload, use `--method <Type.Method>` to constrain `--line`.

## Requirements & Safety

- On the automatic Debug-switch warning: the pause point is already armed - do not interrupt the task or ask the user mid-flow. The setting reverts on every Editor restart and each re-switch costs a full script recompile, so at the next natural stopping point, propose making Debug the startup default; only if the user approves, run `uloop set-code-optimization debug --startup` (without `--startup` the switch is session-only). It is a machine-wide preference affecting every Unity project; only the project's C# scripts run slower, mainly in Play Mode - the Editor itself is not slowed.

- Patches do not survive compiles or domain reloads — re-enable afterwards (a Play entry with Domain Reload enabled removes every source pause point; the enable response warns). `uloop compile` during PlayMode also resets the session.
- Physics message methods, their helpers, and pre-bound delegates can miss hits on pre-existing GameObjects; the enable response warns where detectable.
- An `--id` marker waits on a hand-written `UloopPausePoint.Pause(id)` call; its hits record no `CapturedVariables`.
- On an enable failure, branch on the failure `ErrorCode` and follow `RecommendedNextAction`.
- For scripts under `Packages/`, pass the package-id path form (`Packages/<package-id>/...`); physical checkout paths do not resolve.

## Reference Guides

All live in `references/` beside this skill; read the one whose trigger matches:

- `references/quick-check-template.md` — full `--trigger`/`--await`/`--resume-play` loop, timeouts, hit fields.
- `references/captured-variables.md` — captures, name filters, `--expect` value forms, caller frames, raw values.
- `references/capture-modes-and-history.md` — modes, history, `--max-history`, `--hit-when`.
- `references/line-placement.md` — choosing the line: every-frame lines, held input, input+N frames.
- `references/watch-expressions.md` — watch rules.
- `references/condition-triggered-pause.md` — runtime-condition pauses.
- `references/fast-progressing-games.md` — freezing self-progressing games, `--resume-play`.
- `references/troubleshooting.md` — timeouts, missed hits, hot reload, failure codes.
