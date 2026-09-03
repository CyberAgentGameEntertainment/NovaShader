---
name: uloop-control-play-mode
toolName: control-play-mode
description: "Control Unity Editor Play Mode. Use to Play (or Resume, its alias), Stop, Pause, or Step Play Mode, or query Status without side effects, for runtime behavior checks and frame inspection."
---

# uloop control-play-mode

Control Unity Editor play mode (play/stop/pause/step) or query its state without side effects (status).

## Usage

```bash
uloop control-play-mode [options]
```

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `--action` | string | `Play` | `Play` - start Play Mode, `Stop` - stop Play Mode, `Pause` - pause Play Mode, `Step` - advance one frame while paused, `Status` - report current state without changing anything, `Resume` - alias of Play in every state, including starting Play Mode when stopped |
| `--timeout-seconds` | integer | `180` | Maximum seconds to wait for the requested play mode state |

## Output

Returns JSON with the current play mode state:

- `IsPlaying`: Whether Unity is currently in play mode
- `IsPaused`: Whether play mode is paused
- `Changed`: Whether the requested action changed the current play mode state
- `WasAlreadyStopped`: Whether `Stop` was requested while Play Mode was already stopped
- `ResumedFromPause`: Whether `Play` resumed a paused Play Mode session instead of starting a new one
- `Message`: Description of the action performed
- `Warning` (string, optional): Set when the action carries a caveat. A fresh `Play` start always notes that the session started from Edit-time scene state; additionally, when active hot-reload patches or enabled pause points exist and Domain Reload is enabled, it reports how many of them the Play-entry domain reload will discard. `Status` also reports when Play Mode is running while the Unity Editor is unfocused, because progress may be throttled; run `uloop focus-window`, or use the `pause-point --await`/`--trigger` flow instead of polling for progress.
- `StoppedBy` (string, optional): Why Play Mode last stopped: `cli-control-play-mode`, `cli-compile-stop-setting`, `cli-run-tests-cancel`, `script-compilation`, or `unknown`. Present on `Stop` when Play Mode was already stopped, and on `Status` when Play Mode is not running. Omitted when this Editor session has no confirmed stop.
- `StoppedAt` (string, optional): UTC ISO 8601 timestamp of that stop. Omitted together with `StoppedBy` when no stop is recorded.

## Notes

- Stop on an already-stopped Editor sets `Changed: false`, `WasAlreadyStopped: true`
- `Play` on an Editor that is already playing is a no-op: it sets `Changed: false` and leaves the current session (its accumulated state, spawned objects, progress) untouched instead of restarting it. If you need a clean state for verification, explicitly `Stop` then `Play` rather than relying on `Play` alone to reset anything.
- `Play` while Play Mode is paused resumes the same session: it sets `Changed: true`, `ResumedFromPause: true`, and `Message: "Play mode resumed"` — the session is not restarted.
- `Resume` is an alias of `Play` and behaves identically in every state, including starting Play Mode when stopped.
- `Step` advances exactly one frame and leaves PlayMode paused (the Editor's Next Frame button); it is independent of `Time.timeScale` and requires PlayMode to be running
- The command waits for the requested state before returning. Increase `--timeout-seconds` for projects with slow PlayMode entry.
- Before relying on PlayMode behavior as verification evidence, check `uloop get-logs --log-type Error` for pre-existing errors. An error already present when PlayMode starts can otherwise be mistaken for one caused by the action under test.
- `Status` reads the current state with no side effects: `Changed` is always `false`, no waiting, no scene saving, and it is never rejected by compile errors. It reports whether compile errors would currently block `Play` (`BlockedByCompileErrors` with the `CompileErrors` list), read from the last compile result without triggering a new compile. It does not predict unsaved-changes blocking: `BlockedByUnsavedChanges` describes a failed save attempt during a `Play` request, and `Status` never attempts one.
- `Play` fails immediately with a `CONTROL_PLAY_MODE_UNSAVED_CHANGES` error when unsaved changes cannot be saved quietly — most commonly an Untitled scene, which has no path to save to. The error message lists exactly which scenes or prefab stages blocked it; save the Untitled scene to an explicit path (or discard the changes), then retry.
- `Play` from Edit mode triggers a domain reload (unless Enter Play Mode Options disable it), which discards all active hot-reload patches and enabled pause points. The response `Warning` reports the counts being dropped. Edits that were only hot-reloaded are not part of the compiled assemblies, so the new session runs the last compiled code — run `uloop compile` before `Play` to keep them, or re-apply `uloop hot-reload` after Play Mode starts.
