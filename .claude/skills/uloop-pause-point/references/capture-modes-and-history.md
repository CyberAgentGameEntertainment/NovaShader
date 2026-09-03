# Capture Modes and History

Choose the capture mode when enabling a pause point:

- `single-shot` is the default. The first hit pauses Unity and disarms the marker.
- `continuous` pauses Unity on every hit and remains armed.
- `trace` remains armed and records each hit without pausing Unity.
- In every mode, `CapturedVariables` holds the latest hit and `CapturedVariableHistory` holds only strictly older frames, so with a single hit the history is empty (for `single-shot` it always is). When the latest-hit frame is excluded, `CapturedVariableHistoryNote` explains that the latest hit's variables are in `CapturedVariables`.
- Prefer tracing a line that executes conditionally: a line that runs every frame fills the capped history within a fraction of a second and drops everything recorded before it. When only an every-frame line is available, gate it with `--hit-when` (see [Conditional capture](#conditional-capture-with---hit-when)).
- On every Hit, the response carries a StatusNote. In trace mode it says Play Mode was not paused (the marker fired while the game kept running). In single-shot and continuous it says Unity pauses at the next frame boundary, so live reads after the hit reflect post-frame state; use CapturedVariables for at-line values.
- An Expired response carries a RecommendedNextAction: re-enable the pause point with a longer --timeout-seconds (default 30) and trigger the code path again; clearing the expired marker first is not required (clear only when you also want the leftover code patch removed).
- Expired responses include `MethodEntryCount`: `0` means the armed method was never invoked; a positive value means the method ran but never reached the armed line (branch not taken). For `async` and iterator methods the count is state-machine `MoveNext` entries, so each `await` resumption (async) or iteration step (iterator) increments it.
- This interpretation applies to `--file`/`--line` markers only — a named `UloopPausePoint.Pause` marker enabled with `--id` has no instrumented method, always reports `MethodEntryCount: 0`, and expires with the generic message.
- For an already-hit `continuous` or `trace` marker, `await-pause-point` waits for a **new** hit after the wait starts (`LastHitSequence` advancing). It does not return the stale hit that is already present. Read the current hit with `pause-point-status` instead. If await times out while waiting for that new hit, the error stays `PAUSE_POINT_WAIT_TIMEOUT` and `Details.Hint` tells you to pass `--resume-play` (or resume Play Mode) so another hit can occur. A freshly enabled marker (including `enable-pause-point --await`) has no prior hit, so the first hit satisfies the wait as before.

`--max-history` defaults to 20 and accepts values from 1 through 100. When the limit is exceeded, the oldest frames are dropped and `HistoryDroppedCount` reports how many were removed. `pause-point-status` returns the current `Mode`, `MaxHistory`, history frames, and dropped count.

## Conditional capture with `--hit-when`

`--hit-when "<name> <op> <literal>"` (file:line markers only) evaluates the named captured variable — a local, parameter, field, or `this` — every time the line executes, and turns only matching executions into hits: non-matching executions do not pause Unity, do not enter `trace` history, and are counted in `HitWhenSkippedCount` instead.

- Operators are `==`, `!=`, `>`, `>=`, `<`, `<=`. Literals are `null`, `true`/`false`, an invariant-culture number, or a quoted string (`'…'` or `"…"`, no escape syntax). Ordering operators require a numeric literal.
- The comparison runs against the live runtime value, not the serialized `Value` string: string literals compare ordinally to string variables, and numeric literals compare to numeric primitives (enums and chars do not qualify). Numeric comparisons are evaluated in `double`, so `==` on floats or on integers beyond double's exact integer range can miss — prefer `>=`/`<=` ranges there.
- Evaluation problems fail open: a missing variable name or a type mismatch still captures the hit, and `HitWhenErrorNote` reports the first such error.
- Responses echo the armed condition in `HitWhen`. When the line executed but nothing matched, `pause-point-status` adds `HitWhenNote`, and `await-pause-point` timeout/expiry errors report the skip count instead of claiming the line never ran.

On a line that runs every frame, raising `--max-history` cannot preserve an arbitrary later frame: at 60 fps even `--max-history 100` holds only about 1.6 seconds before the oldest frames drop. When a condition identifies the frames you care about, arm with `--hit-when` so only matching hits consume history.

Re-enabling the same `file:line` replaces the marker instead of updating it: the
marker starts a new generation and the previous `CapturedVariableHistory` is
discarded, so a re-enable never carries frames over. Read the history you still
need with `pause-point-status` before re-enabling (for example before raising
`--max-preview-elements` or changing the mode).

To inspect value changes one Editor Step at a time, pair a `continuous` marker with a `control-play-mode --action Step` + `pause-point-status` loop — see [captured-variables.md](captured-variables.md) for the loop and its caveats.

For multi-step verification, avoid repeating enable→await→clear cycles with the default single-shot mode: pass `--mode continuous` to `enable-pause-point`, or enable several file:line markers at once — markers are independent and can stay armed simultaneously.
