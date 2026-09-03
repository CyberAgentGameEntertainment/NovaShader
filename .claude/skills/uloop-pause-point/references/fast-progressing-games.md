# Fast-Progressing Games

Read this when the game advances on its own (timers, gravity, spawners, a ball that keeps bouncing, pieces that keep falling) and CLI round-trips are slower than the game's own tick.

## Freeze → Build → Resume in One Call

Any state you arrange while PlayMode runs live can be consumed by the game before your next command arrives. Freeze first, build while paused, then resume and fire the input in one call:

```bash
# 1) Freeze the whole player loop before arranging anything
uloop control-play-mode --action Pause

# 2) While paused, build the exact scenario (production methods preferred; see below)
uloop execute-dynamic-code --code '...'

# 3) One call: confirm the marker armed, resume PlayMode, fire the input, await the hit
uloop enable-pause-point --file Assets/Scripts/Enemy.cs --line 42 --timeout-seconds 60 \
  --await --resume-play --trigger "simulate-keyboard --action Press --key Digit3"
```

Digit keys are `Digit0`-`Digit9` or `Numpad0`-`Numpad9` — bare `0`-`9` is rejected.

## Clear Before Scenario Setup

`clear-pause-point --all` (and clearing the marker that owns the current pause) resumes Play Mode when the pause came from a pause-point hit. Run clear **before** arranging the board or other scenario setup; otherwise the resume lets the game consume your setup mid-flight. If you still need to build state after clear, re-freeze with `control-play-mode --action Pause` first, then arrange, then arm with `--resume-play`.

## --resume-play Semantics

`--resume-play` runs after the marker's arming is confirmed and before `--trigger` is dispatched: it resumes PlayMode only when PlayMode is actually paused, and reports what it did in `ResumePlayResult` (`WasPaused` / `Resumed` / `Error`; an abandoned wait adds `Repaused` / `RepauseError`). If the resume fails, the trigger is not dispatched and `TriggerResult.Error` says so. If the trigger itself is rejected before it runs, the wait is abandoned and the resume is undone: `Repaused: true` (or `RepauseError`) reports PlayMode being paused again, so gameplay cannot consume the preserved marker while the trigger value is being fixed. When the game reaches the line on its own after resuming (gravity, physics), omit `--trigger` and keep `--resume-play`.

## Do Not Use Time.timeScale = 0

Projects that read unscaled time keep advancing regardless, and the value silently persists into the next PlayMode session. Editor pause and `Step` freeze the entire player loop independent of `Time.timeScale`.

## The Residual Race

After the resume, the game runs freely for the single in-process round-trip until the trigger input lands. When even that is longer than the game's natural tick interval (for example a piece that auto-falls every 0.8 seconds), remove the race instead of trying to outrun it: temporarily overwrite the tick-interval field with `execute-dynamic-code`, run the verification, then restore the original value and confirm the restore with a re-read.

## Injecting State While Paused

Writing fields or transforms directly while the Editor is paused can silently fail to stick: `transform.position` and `Rigidbody2D.position` do not synchronize until the next simulation step, and any production `Update()` that recomputes the value will overwrite the injection on the next frame. Prefer arranging state through the game's own methods. After a direct write, verify in two stages: re-read immediately while still paused to confirm the write landed, then advance one frame with `control-play-mode --action Step` and re-read again to confirm it survives the frame — a post-Step revert means an `Update()` recompute or a deferred physics sync overwrote it, which the immediate read alone cannot reveal.
