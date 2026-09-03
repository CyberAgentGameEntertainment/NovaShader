# Pause Point Troubleshooting

Read this when a wait times out, `HitCount` stays `0`, or `enable-pause-point` fails.

## Timeout Diagnosis

A `PAUSE_POINT_EXPIRED` error carries the same `Error.Details.Hint` as a timeout plus a shell-neutral `Error.Details.RecommendedNextAction`. Inspect `Error.Details.Status`, `HitCount`, `Generation`, `EnabledAtUtc`, `EditorState`, `ElapsedSinceEnabledMilliseconds`, and `RemainingMilliseconds` to distinguish input not being consumed, stale evidence from an older marker generation, runtime conditions not being met, an id mismatch, or Unity already being paused. `ElapsedSinceEnabledMilliseconds` is measured from `enable-pause-point`, not from `await-pause-point`. A wait timeout that is not waiting for a new hit on a continuous/trace marker auto-clears the marker; `Error.Details.MarkerClearedByThisCommand` is true when this command did that.

The `--timeout-seconds` countdown freezes only while a pause-point hit holds the Editor paused; the elapsed pause duration is credited back onto the marker's expiry on resume, so inspecting a paused hit for as long as you need does not erode the remaining timeout budget. A manual pause without a hit does not stop the countdown.

A `PAUSE_POINT_EXPIRED` error means the marker's own `enable-pause-point --timeout-seconds` window (measured from enable, not from wait) ran out first — re-enable the pause point with its original selector (`--file`/`--line`, or `--id` only for a named marker) and a longer `--timeout-seconds`; clearing the expired marker first is not required. When `--trigger` was passed, the expired envelope also carries `Error.Details.TriggerResult` (with `Completed: false` and no `Error` field when the trigger's outcome was still unknown at expiry) — such a result carries an `Explanation` field stating that the wait settled first and the trigger may still have delivered its input.

Expired responses include `MethodEntryCount`: `0` means the armed method was never invoked; a positive value means the method ran but never reached the armed line (branch not taken). For `async` and iterator methods the count is state-machine `MoveNext` entries, so each `await` resumption (async) or iteration step (iterator) increments it. This interpretation applies to `--file`/`--line` markers only — a named `UloopPausePoint.Pause` marker enabled with `--id` has no instrumented method, always reports `MethodEntryCount: 0`, and expires with the generic message.

## Hit Preconditions

A pause point hits only when control flow reaches the patched line (or the `Pause(id)` call). `simulate-keyboard` returning `PressEdgeObserved=true` means the input edge was observed, not that your target game logic has reached the pause line yet.

If a `simulate-*` command instead returns a failure whose message says PlayMode is paused, suspect a pause point hit rather than an unrelated failure: an active pause point can make PlayMode paused mid-simulation, and the `simulate-*` call surfaces that as a preflight failure. The failure response names the responsible marker in `RejectedByActivePausePointId`. Check `uloop pause-point-status --id <id>` first to confirm the hit before treating it as a bug in the simulated action itself.

## Locating Where Control Flow Stops

To locate where control flow stops before an unhit line, bisect with a second pause point on the method's entry (its first executable line). If the entry point hits while the target line stays at `HitCount=0`, an early return or a branch between the two lines is filtering execution — inspect the guard values in the entry hit's `CapturedVariables` instead of retrying the original line.

## JIT Inlining

Mono can inline very small target methods into callers, and the pause point then never fires even though the line runs. If a line demonstrably runs but the pause point stays unhit and nothing else explains it, move the pause point into the calling method.

## Physics Message Methods and One-Hop Helpers

Physical Unity message methods (`OnCollisionEnter2D`, `OnTriggerEnter2D`, and similar callbacks) can silently miss: a GameObject that already existed at enable time may keep calling the pre-patch code, so `HitCount` stays `0` even though the method body runs. The condition is environment-dependent. On `enable-pause-point --await`, that enable-time patch diagnostic appears as top-level `EnableTimeWarning` (omitted when empty) — it is independent of whether the marker later hits, and it is not folded into hit-time `Warning`. On a non-hit failure, the same text is under `Error.Details.EnableWarning`. The same applies one hop out — a helper called from a physics message method in the same compiled assembly; deeper call chains or callers in other assemblies are not detected by the warning but can fail the same way.

Recovery order:

1. Confirm the body actually ran after arming, via evidence from fresh contact — a stale pre-arm counter or log proves nothing.
2. `clear-pause-point` the marker, `enable-pause-point` it again, and wait for the next fresh contact.
3. Recreate the GameObject after enabling.
4. Embed `UloopPausePoint.Pause("<id>")` in the method body and use an id-only marker.

A one-way cross-check: hot-reload a temporary log line into the method (`uloop hot-reload`) and re-trigger — the log appearing proves the body ran even though the marker missed; the log staying absent proves nothing, because the same cached dispatch can bypass the hot-reload patch too.

## Pre-Bound Delegates

A method already bound into a delegate or event before `enable-pause-point` may not fire through that delegate: the pre-bound invocation path can bypass the patch. Workarounds: enable the pause point before the delegate is created, recreate the subscribing GameObject, or re-bind the delegate (e.g. via `execute-dynamic-code`) after enabling.

## Hot-Reload Line Resolution

`enable-pause-point` works on hot-reload patched methods: the marker resolves against the patched body, and `RetargetedToHotReloadPatch: true` in the response confirms it is armed on the edited code — that flag is not a problem; it means the marker follows the patched body and keeps firing at the edited line. Methods the reload did not patch are the opposite case: `--line` on them resolves against the last compiled source, not the edited file, so line drift from the edit can silently arm a different method. Pass `--method` with the simple method name or `Type.Method` to keep `--line` inside that method and fail instead of arming a neighbor. The response carries a Warning when this applies — check `ResolvedMethod` and `ResolvedLineText` before trusting the marker, or run `uloop compile` and re-enable. When the statement text at the resolved line is identical in the edited file, the Warning says so and no manual comparison is needed. When the compiled statement at the resolved line differs from the edited file, the response also includes a compiled-line drift Warning and RecommendedNextAction. `CapturedVariables` never includes fields added by hot reload (their values live in a side table); enable-pause-point warns when the resolved type has any.

Successful `--file`/`--line` enables also report `LineBasis`: `EditedFile` means `--line` was resolved against the edited file (the line falls inside a hot-reload patched method), while `LastCompiledSource` means it was resolved against the last compiled source — including when no hot reload has run at all.

`PAUSE_POINT_PATCHED_BY_HOT_RELOAD` is returned only when the requested line cannot be mapped onto the patched body — the file's line map is stale or the patch belongs to a superseded hot-reload generation. Pick a line inside the edited method body, run `uloop hot-reload --revert-all`, or run `uloop compile`, then retry. When the compiled line range of the patched method is known, the failure message also reports it, so you can see how far the edited file's line numbers have shifted from the compiled source.

`SuppressedByHotReload: true` on a status response means a later hot-reload transition (apply, a newer generation, or revert) could not re-target the armed marker; the reason is in `SuppressedByHotReloadReason` and surfaced as the status `Warning`. The marker is not cleared — it fires again once a transition restores its line, or after `uloop compile` and a re-enable. Recover by reverting the patch (`uloop hot-reload --revert-all`), editing so the line exists again and re-running `uloop hot-reload`, or running `uloop compile` and re-enabling the marker.

## Enable Failures

If `enable-pause-point` fails, branch on the failure `ErrorCode` and follow `RecommendedNextAction`; `Message` explains the rejection in prose. Codes: `INVALID_ARGUMENT` (fix the rejected argument and re-run), `PAUSE_POINT_RELEASE_CODE_OPTIMIZATION` (automatic Debug switch and recompile did not leave the Editor in Debug; retry after a successful compile), `PAUSE_POINT_RESOLVE_FAILED` (the file:line could not be mapped to a patch location), `PAUSE_POINT_PATCH_FAILED` (the resolved method cannot be patched).

If enable fails with `PAUSE_POINT_RESOLVE_FAILED` while the file has active hot-reload patches, `--line` was resolved against the last compiled source (the editor shows the edited file). `ResolvedMethod` and `ResolvedLineText` stay empty on that failure — follow `RecommendedNextAction` rather than those fields. Recompute the line against the last compiled source, or run `uloop compile` and re-enable.

If enable fails with a "No sequence point found" error even for clearly executable lines, that script's assembly lacks debug sequence points and no line in the file can be patched. Move the pause point to a script in an assembly that carries them, such as a script under `Assets/`.
