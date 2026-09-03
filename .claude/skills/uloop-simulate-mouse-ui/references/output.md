# simulate-mouse-ui Output Fields

Returns JSON with:

- `Success`: Whether the operation succeeded
- `Message`: Status message (e.g. "Hit element: ButtonStart" or "No UI element under (x, y)")
- `Action`: Echoes which action was executed (`Click`, `Drag`, `DragStart`, `DragMove`, `DragEnd`, or `LongPress`)
- `HitGameObjectName`: Name of the GameObject that received the pointer events (nullable string; null if nothing was hit). It is the event-handler target resolved up the hierarchy from the raycast hit (the first `IPointerDownHandler`, else the first `IPointerClickHandler`, for `Click`/`LongPress`; the first `IDragHandler` for drags), so it can be an ancestor of the topmost raycast hit — a Button's child Text reports the Button. With `--bypass-raycast` it is the handler resolved the same way from `--target-path` (also possibly an ancestor), not the raycast hit and not necessarily the raw path GameObject itself
- `PositionX`: Target X coordinate that was used
- `PositionY`: Target Y coordinate that was used
- `EndPositionX`: Drag end X coordinate (nullable float; populated for drag actions only)
- `EndPositionY`: Drag end Y coordinate (nullable float; populated for drag actions only)
- `InterruptedByPausePoint` / `PausePointId` / `PausePointHitCount` / `PausePointHits`: Pause-point interruption info (all nullable except the boolean). `PausePointHits` lists every marker hit during this input in hit order; `PausePointId` only names the latest one. See the Pause Point Inspection section in SKILL.md

Verify the visual outcome with a follow-up `uloop screenshot --capture-mode rendering --annotate-elements`.

Note: Click and LongPress on empty space (no UI element) still return `Success = true` with `HitGameObjectName = null`. Drag actions on empty space return `Success = false`.
