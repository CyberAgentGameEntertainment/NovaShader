# CapturedVariables Semantics

Read this before interpreting unexpected, missing, or truncated captured values, nested previews, `continuous`-mode history, or when you need live references while Unity is still paused.

## Snapshot Timing

- The snapshot is taken **before** the resolved line executes, exactly like an IDE breakpoint on that line. To inspect a value after an assignment, place the pause point on the following line.
- The pause itself only takes effect at the next frame boundary: the frame that hit the pause point still runs to completion first, so any event that fires later in that same frame (a chained collision, a cascading destroy) has already happened by the time Unity actually stops. Trust `CapturedVariables` (the pre-line snapshot) as evidence for what was true up to the patched line; do not assume the paused state still matches it for events later in the same frame.
- Rigidbody values read inside a physics callback (`OnCollision*`/`OnTrigger*`) can be mid-solver intermediates — `velocity` may capture as `(0.0, 0.0)` at the callback even though the body visibly moves. `CapturedVariables` faithfully records that intermediate value; a later `execute-dynamic-code` read returning something different means the physics solver has since finished the step, not that the capture was wrong.
- `execute-dynamic-code` during the pause sees the interrupted method's **post-interrupt** state, not this pre-line snapshot. Use `CapturedVariables` for pre-line evidence; use the raw capture API below when you need live references while paused. If you suspect a captured value is stale or wrong, cross-check it against the live scene object with `execute-dynamic-code` (for example reading `transform.position` off the instance found via `UnityObjectPath`) rather than trusting either source alone. `execute-dynamic-code` responses also carry `EditorPaused` and `ActivePausePointId` — these fields appear only while the Editor is paused, so a call made while a pause point still has Unity paused is unambiguous instead of looking like a stale or buggy result. `EditorPlaying` is always present and reports whether Play Mode is running, so a stopped versus playing Editor is visible even when `EditorPaused` is omitted.

## Scopes and the `this` Entry

- `Scope` is `Local`, `Parameter`, `InstanceField`, or `This`. `InstanceField` entries come from a reflection walk of the paused instance's declared type, not from the method's IL usage, so a field the method never reads can still appear — and `MaxCapturedVariableCount` still caps the total entry count across all scopes, so a field-heavy type can push some instance fields out of the snapshot. If a specific field you want is missing, read it directly from the live instance instead of waiting on the capped snapshot: while still paused, `UloopPausePoint.TryGetCapturedValue("this")` returns the live `this` reference, so `execute-dynamic-code` can read any field or property off it regardless of the cap.
- The snapshot also includes a synthetic `this` entry (Scope `This`) for the paused instance itself, so you can tell which instance or GameObject was hit via its `UnityObjectPath` and `UnityObjectInstanceId`. For an async or coroutine method it resolves to the original outer instance, not the compiler-generated state machine, and static methods emit no `this` entry. While Unity is still paused, `UloopPausePoint.TryGetCapturedValue("this")` returns the live instance reference (for example so a watch expression can read `transform.position`).
- async and coroutine methods work: hoisted locals and the original `this` fields appear under their normal names.
- Auto-implemented properties are captured as instance fields under the property name (the compiler-generated backing field is un-mangled), so you do not need to rewrite them as explicit fields for verification.
- If the patched method ran off the main thread, values degrade to type names with a `(captured off main thread)` note; the hit itself is still recorded.

## Value Rendering, Previews, and Caps

- Nested previews stop at `MaxCollectionPreviewDepth` (2 levels) below each captured variable: past that, an object or collection renders as type-name-only text instead of expanding — a type name where you expected contents means you hit this cap, not a bug. The budget is counted per captured variable, so reaching a value through `this` costs one extra level compared to reading it as a direct local: `this.CurrentPiece.Origin` bottoms out as a type name, while a `dropped` local holding the same piece expands to `{Kind, RotationState, Origin: {X, Y}}`. When the value you need sits too deep, pick a pause point line where it is a direct local or parameter — as its own top-level entry it starts with a fresh full budget. Primitive leaves (numbers, strings, booleans, and any type that overrides `ToString()`) always render regardless of depth; only nested objects and collections get cut off.
- A value's `Value` string is not always its plain `ToString()`. A materialized collection (`List<T>`, arrays, dictionaries, ...) previews as a shallow JSON array/object instead of the default type-name text. A custom struct/class whose declared type does not override `ToString()` previews the same way — a shallow JSON object of its fields — so you do not need to add a temporary `ToString()` override just to see its contents. A type that does override `ToString()` keeps using that result unchanged. Either kind of preview is capped by depth, element count, and length like any other captured value; the element-count cap (default 10) and the preview's character budget both scale with `enable-pause-point --max-preview-elements` (1–1000). Raising it scales the character budget proportionally, so each element keeps the same ~100-character share it has at the default — plenty for numeric or boolean cells, but individually long elements can still be clipped by the scaled budget. The enable response echoes the effective `MaxPreviewElements`.
- A captured `Collision2D` is previewed as `{"Collider":{"Name":...,"UnityObjectPath":...},"OtherCollider":{...},"RelativeVelocity":...,"ContactCount":...}` — read `UnityObjectPath` to identify both colliding objects without an extra `execute-dynamic-code` round-trip. Each of `Collider` / `OtherCollider` is either that object form or the string `"(none)"` when the collider is null or destroyed.
- A multidimensional array (`int[,]`, `int[,,]`, ...) previews as `{"Shape":"Int32[2,3]","TotalElements":6,"PreviewedElements":6,"ElementOrder":"row-major (last dimension fastest)","Elements":[...]}` instead of a bare JSON array, since `Elements` flattens every rank in row-major order (last dimension fastest) and would otherwise look like an empty or 1D collection; when `--max-preview-elements` cuts the list, the preview also includes `"ElementsTruncated":true` and a smaller `PreviewedElements`. A `T[]` or jagged `T[][]` array is unaffected and still previews as a plain JSON array.
- `CapturedVariablesTruncated=true` means at least one value was clipped (value-length cap or collection preview element cap) or the variable-count cap stopped enumeration; clipped values are still present up to the cap.
- `TruncatedVariableCount` is the exact number of variables that were dropped whole by the variable-count cap or whose value preview was clipped (`Truncated: true` on the entry). `TruncatedVariableNames` lists that union in capture order, at most 20 names; the count stays exact when more than 20 were affected. The invariant is `CapturedVariablesTruncated == (TruncatedVariableCount > 0)` and `TruncatedVariableCount >= TruncatedVariableNames.Length`.

## Unity Object Values

- `UnityEngine.Object` values additionally carry `UnityObjectKind` (`SceneObject`, `PrefabAsset`, `Asset`, `RuntimeInstance`, or `Destroyed`), `UnityObjectPath`, and `UnityObjectInstanceId`. These three fields appear only for Unity object values; a non-Unity-object variable (an `int`, a `string`, a plain class) omits all three from the JSON entirely instead of sending them as empty/zero. Check whether `UnityObjectKind` is present to tell the two cases apart. Use the fields as handles for the next dig: a `SceneObject` path feeds `get-hierarchy`/`find-game-objects`, an asset path locates the asset, and the InstanceID works with `execute-dynamic-code`.
- A captured `UnityEngine.Object` value's `Value` string is only the object's `name` — its fields never appear there, and its `ToString()` is not consulted either. A `MonoBehaviour` parameter therefore reads as something like `Block(Clone)`, indistinguishable from every other clone, with none of its `[SerializeField]` values visible. To tell instances apart in snapshots, assign distinguishing names when you create them (for example `gameObject.name = $"Block_{blockId}"`). To read a specific field, stay paused and read it off the live instance with `execute-dynamic-code` (via `UnityObjectPath`/`UnityObjectInstanceId`, or `UloopPausePoint.TryGetCapturedValue("this")` for the paused instance itself).

## Pulling More Than the Default Response Carries

The hit and status responses are push-first and kept lean by default: no field is ever a re-summary of another field, and a variable's `Value` is the only per-entry cost. For a class with dozens of `[SerializeField]` fields, a `continuous` marker's history still multiplies entry count by `MaxHistory` (default 20), which can be a lot of `Value` strings to carry around when you only need to know which names were captured.

Pull only what you need instead of paying for it all up front:

- `--captured-variables names` on `await-pause-point`/`pause-point-status` drops `Value` from every captured variable (including every history frame) and keeps `Name`/`Scope`/`TypeName`. Use it first on a field-heavy class, then fetch specific values afterward.
- `uloop pause-point-status --id <id>` returns the full response again, including every `Value`, whenever you need it — call it plain (no `--captured-variables`) for the complete picture after a lightweight `names` scan.

## Step Sessions

To inspect value changes one Editor Step at a time, enable a `continuous` pause point on a line inside `Update` or `FixedUpdate`, trigger the first hit, then run:

```bash
uloop control-play-mode --action Step
uloop pause-point-status --id "Assets/Scripts/Enemy.cs:42"
```

Repeat the Step/status pair to inspect the history tail. A new frame is captured only when the patched line executes during that frame; event handlers such as `OnCollisionEnter` update only when the event occurs again. Use a longer `--timeout-seconds` for a Step session because the enable-time timeout does not extend after hits.

## Choosing the Right Evidence Source

Three different sources answer three different questions about a captured variable; pick by what you actually need:

| Need | Source | Notes |
|---|---|---|
| A value type's value at capture time | `UloopPausePoint.TryGetCapturedValue("name")` | Faithful: value types are a boxed copy taken at capture time, so this never drifts. |
| A reference type's *live* current state | `UloopPausePoint.TryGetCapturedValue("name")` | The reference itself is live, so the object it points to may have changed since capture (or been destroyed/resumed away). Only available while Unity is still paused. |
| A reference type's state *as it was at capture time* | `uloop pause-point-status --id <id>` | The only faithful source for this: the response is a formatted string snapshot taken at capture time and stored in the registry, so it never drifts and stays retrievable after resume until the next clear or domain reload. |

Capturing a deep copy at hit time was deliberately not adopted: it would cost hot-path performance and risk getter side effects, so the formatted-string snapshot (`pause-point-status`) remains the only way to get capture-time-faithful evidence for reference types.

## Raw Capture API While Paused

Add `using io.github.hatayama.UnityCliLoop.Runtime;` in `execute-dynamic-code` snippets before calling `UloopPausePoint.TryGetCapturedValue` / `GetCapturedNames` / `GetCapturedPausePointId`.

While Unity is paused on a hit, `execute-dynamic-code` can read live captured references through `UloopPausePoint`:

- `TryGetCapturedValue(string name)` returns `(bool Found, object Value)` for the latest hit only. When multiple captured variables share the same name, the last one wins.
- `GetCapturedNames()` lists captured variable names from that snapshot.
- `GetCapturedPausePointId()` returns the pause-point id for the held snapshot.

Deconstruct the tuple before use:

```csharp
(bool found, object value) = UloopPausePoint.TryGetCapturedValue("this");
if (!found) { return "capture missing"; }
return value;
```

The references are live objects in their frame-completed state: anything the hit's method changed — or destroyed — after the patched line is already applied, so a captured object can read as destroyed/null here even though `CapturedVariables` shows its pre-line field values intact.
Concrete example: with a marker on the line right before `Destroy(obj)`, `TryGetCapturedValue("obj")` returns the frame-completed object — already destroyed — while the pre-destroy field values are still readable in `CapturedVariables`.

The holder clears when Unity resumes (not when you `Step` while still paused), when the matching pause point is cleared, when a new hit replaces the snapshot, or when PlayMode exits. After resume, `TryGetCapturedValue` returns `Found=false`. Re-enabling the same pause point while still paused (for example to refresh its timeout during a step session) keeps the held references, because a re-enable does not resume Unity.

For a self-progressing game, arranging a scenario through real input alone is a race (each `simulate-*` call is a separate CLI round trip, often longer than the game's own tick). Instead, while paused on a hit, use `TryGetCapturedValue("this")` to get the live instance and call its production methods to build the exact state, then resume and send real simulated input for only the one action you are verifying — arm the next marker with `--resume-play`, or run `control-play-mode --action Play` first, since `simulate-*` input requires an unpaused PlayMode. The setup stays deterministic while the observed action still exercises the real input path.

## Warnings and Marker Freshness

`await-pause-point`'s hit response also carries a top-level `Warning` (omitted when empty): it flags multiple hits, multiple matching logs, or truncated matching logs, so you can tell a single clean hit apart from evidence that needs closer inspection. Enable-time patch diagnostics (for example physics-callback cached dispatch) are not in `Warning`; on `enable-pause-point --await` they appear as `EnableTimeWarning` instead. `MatchingLogs` (log entries whose text contains the marker id) is still embedded, but source-derived ids rarely appear in log text, so treat `CapturedVariables` as the primary variable evidence.

Use `Generation`, `EnabledAtUtc`, and the hit sequence fields from the hit or status response to tell a fresh marker from stale evidence with the same id. `RemainingMilliseconds` and `Expired` are returned directly so you do not need to infer marker lifetime from elapsed time. HitSequence numbers come from a sequence shared by all pause points in the current Editor domain (it resets on domain reload); they order hits across markers and are not 1..HitCount for this marker.

## Caller frames

Each hit records up to `--max-caller-frames` managed caller frames (`CallerFrames`, nearest caller first; default 2, range 0–8). 0 disables capture and leaves an empty array. The value is fixed at enable time and also caps every later `pause-point-status` response for that marker; status has no flag to change it. `pause-point-status` and `await-pause-point` responses carry them top-level for the latest hit and on every history frame; `enable-pause-point` / `clear-pause-point` responses carry them on history frames only, because those payloads have no top-level capture. The field is always present — an empty array when no managed callers were captured. Selection rules:

- Runtime machinery (`System.*`, `Microsoft.*`, `Mono.*`), patching infrastructure (`HarmonyLib.*`, `MonoMod.*`), and uloop's own frames are skipped — except a Harmony patch body, which is a real application caller and is kept as described below. Unity engine and editor frames are kept because an entry point such as `UnityEditor.EditorApplication.update` is itself diagnostic.
- Async callers are reported by their logical method name (compiler state-machine frames are demangled to `Namespace.Type.Method`).
- Debug symbols (the Debug code-optimization prerequisite pause points already have) control only `File` and `Line`: a frame without symbols keeps its formatted `Method` and omits `File`/`Line`. When those fields are omitted, `Note` names the reason: a caller running as a hot-reload-patched **or pause-point-instrumented** Harmony dynamic method (`"dynamic method (patched by hot reload or pause-point instrumentation); no debug symbols"`); a frame whose assembly has no debug symbols (`"no source file information; the frame's assembly has no debug symbols"`); or a source path outside `Assets/`, `Packages/`, or `Library/PackageCache/` (`"source file is outside the Unity project"`), so the payload never carries a machine path. Do not treat a missing `File` as "outside the project" by default — that label applies only when the raw path was present and failed project-root normalization.
- The frames are the synchronous call chain at the moment the marker line ran. A marker that resumes after an `await` does not see its original awaiting caller — only dispatch machinery remains, so expect a method-only engine frame (or an empty array); the awaiting method itself never appears. After a synchronization-context resume that frame is typically `UnityEngine.UnitySynchronizationContext`; after `await Awaitable.NextFrameAsync` it is typically an Awaitable continuation such as `` UnityEngine.Awaitable+AwaitableAsyncMethodBuilder+StateMachineBox`1.DoMoveNext `` or `UnityEngine.Awaitable.RunOrScheduleContinuation`. The engine-direct case (an `Update` marker) is a plain empty array.

Capturing the frames costs on the order of 0.1 ms per hit, which also bounds the extra trace-mode overhead per recorded hit.

## Name Filters and Expectations (`--expect`, `--captured-variable-names`)

- `--captured-variables` defaults to `full`, which keeps each captured entry's `Value` (capture caps still apply — watch `CapturedVariablesTruncated`). When that dump is noisy, trim it with `--captured-variable-names` or `--captured-variables names`.
- When the response would be dominated by variables you do not need, pass `--captured-variable-names velocity,this` (comma-separated, exact match on `Name`) to keep only those entries; it composes with `--captured-variables full|names`. `CapturedVariablesTruncated` reports truncation at Unity-side capture time and is independent of this name filter — it can stay `true` even when every listed variable is complete, if a truncated variable was excluded by the filter. In that case the CLI sets `CapturedVariablesTruncatedNote`. Requested names that matched nothing are listed in `CapturedVariableNamesNotFound`, so a partial match is visible without comparing the response against the request by hand.
- Pass `--expect 'name=value'` (repeatable; on `await-pause-point`, `enable-pause-point --await`, and `pause-point-status`) to have the CLI compare captured variables against expected values; the response includes an `Expectations` array and `AllExpectationsPassed`, so you do not need to eyeball the JSON. Matching is string equality against the serialized value. On `pause-point-status` a marker that has not been hit yet reports each expectation as not found, and the verdict never changes the exit code — a polling loop reads `AllExpectationsPassed`, not the exit status.
  Serialized `value` forms that match in practice (string equality against `CapturedVariables[].Value`):
  - bool: `True` / `False` (C# form, capital first letter)
  - float: `7` when the value is exactly an integer (not `7.0`)
  - Vector2/3 and custom structs via `ToString()`: `(2.31, 6.61)` (one space after each comma)
  - enum: `Grass` (member name only)
  - `List<int>` / arrays: `[19]`, `[0,1,2,3]` (numeric elements unquoted)
  - `List<Vector2Int>` and other element-`ToString()` collections: `["(9, 3)","(9, 2)"]` (elements quoted)
  When unsure, hit once and copy the `Value` string from `CapturedVariables` into `--expect` verbatim.
