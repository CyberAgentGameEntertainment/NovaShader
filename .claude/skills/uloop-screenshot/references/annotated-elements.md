# Annotated Elements and Coordinates

Read this when using `uloop screenshot --capture-mode rendering --annotate-elements` or `--annotate-raycast-grid` output to find coordinates for `simulate-mouse-ui` or `simulate-mouse-input` (including `--dry-run`).

Device Simulator is supported for this flow: prefer `--capture-mode rendering` (not `window`) so coordinates match the simulated device resolution.

## AnnotatedElements Fields

`AnnotatedElements` is empty unless `--annotate-elements` is used, or unless `--annotate-raycast-grid` adds clustered 3D collider candidates (with or without `--raycast-layer-mask`). UI entries are sorted by z-order, frontmost first. That sort uses `SortingOrder` then `SiblingIndex`, so parent-crossing overlaps can disagree with the actual draw order — see `SiblingIndex` below. Each item contains:

- `Label`: Index label in JSON (`A` = first in that sort, `B` = next, ...). Treat it as a stable handle, not as proof of what is actually in front: when overlapping elements cross parents, pick the target by `Path` and confirm the result with `simulate-mouse-ui`'s `HitGameObjectName` — the handler that received the events, resolved up the hierarchy from the raycast hit (or from `--target-path` in bypass mode), so it can name an ancestor of the element you clicked. Screenshot labels also include the interaction hint, such as `A / CLICK` or `B / DRAG`.
- `Name`: Element name
- `Path`: Hierarchy path from the scene root, for example `Canvas/Panel/Button`. Use this as `simulate-mouse-ui --target-path` when bypassing raycast blockers.
- `Type`: Element type (`Button`, `Toggle`, `Slider`, `Dropdown`, `InputField`, `Scrollbar`, `Draggable`, `DropTarget`, `Selectable`, `PhysicsCollider`)
- `Interaction`: Derived interaction category (`Click`, `Drag`, `Drop`, `Text`) or `Raycast` for clustered physics collider entries. Use this to choose between `simulate-mouse-ui --action Click`, drag actions, or `simulate-mouse-input` (including `--dry-run`).
- `Layer`: Physics layer name for `PhysicsCollider` entries. Empty for UI entries.
- `Components`: Collider and MonoBehaviour component type names from the hit GameObject for `PhysicsCollider` entries. Empty for UI entries.
- `SimX`, `SimY`: Target click position in top-left Game View coordinates. For UI entries this is the element center when that center is raycast-reachable; otherwise it is a reachable probe point inside the bounds (center, then four interior quarter points). For `PhysicsCollider` entries this is a representative sampled hit. Use these directly with `simulate-mouse-ui --x/--y` or `simulate-mouse-input --x/--y` (`--dry-run` optional).
- `BoundsMinX`, `BoundsMinY`, `BoundsMaxX`, `BoundsMaxY`: Bounding box in the same coordinates as `SimX/SimY`. For `PhysicsCollider` entries, this is the axis-aligned sampled-cell coverage box from reachable raycast hits, not a guarantee that every interior point is clickable.
- `SortingOrder`: Canvas sorting order. Higher values are in front.
- `SiblingIndex`: Transform sibling index under the element's direct parent. Do not use it as a reliable z-order signal across nested UI hierarchies; the frontmost-first sort above can then disagree with draw order.

## Exclusion rules (by design)

The annotator lists elements that share the same EventSystem raycast path as `simulate-mouse-ui`. The following are omitted on purpose:

- Elements under a Canvas that has no enabled `GraphicRaycaster` are not clickable through EventSystem, so they are not listed.
- Elements on a World Space or Camera Space Canvas whose camera cannot be resolved from `worldCamera`, the root canvas, or `Camera.main` have no screen coordinates, so they are not listed.
- Elements whose center and four interior quarter probe points are all covered by another raycast hit are not listed.

### PhysicsCollider Entries Per Closed Region

When a single `PhysicsCollider` GameObject's reachable raycast samples form multiple 4-connected regions on screen — typically because UI occlusion splits them apart — each region becomes its own `AnnotatedElements` entry. `Path`, `Name`, `Layer`, and `Components` are identical across those entries (they describe the same GameObject), while `Label`, `Bounds`, `SimX`, `SimY`, and `RaycastOutlineSegments` are independent per region so each closed area is separately addressable.

When multiple entries share the same `Path`, use `SimX`/`SimY` (or the label position drawn on the PNG) to click a specific region. `simulate-mouse-ui --target-path <Path>` still works and reaches the GameObject through whichever region is clickable, so use it when the region choice does not matter.

## RaycastLayerSummaries Fields

`RaycastLayerSummaries` is always populated when `--annotate-raycast-grid` is used, regardless of `--raycast-layer-mask`. It is built from a dense 40x40 raycast sample pass over `Physics.DefaultRaycastLayers` (fixed, independent of `--raycast-layer-mask`), so it always tells you what else is hittable across every default-visible layer, even when you narrowed `AnnotatedElements` down to one layer with `--raycast-layer-mask`.

- `Layer`: Physics layer name to pass to `--raycast-layer-mask`
- `LayerIndex`: Unity physics layer index
- `HitCount`: Dense raycast hit count for the layer
- `RepresentativeObjectPath`: Hierarchy path for the object with the most hits on that layer. Ties are resolved alphabetically by path.

Entries are sorted by `HitCount` descending, then `LayerIndex` ascending.

## RaycastLayerNamesChecked Fields

`RaycastLayerNamesChecked` is populated when `--annotate-raycast-grid` is used. It lists the physics layer names that were actually eligible to produce `AnnotatedElements` `PhysicsCollider` entries in this response: the clustering mask (`--raycast-layer-mask` if set, otherwise `Physics.DefaultRaycastLayers`) intersected with `Camera.main.cullingMask`. Use it to diagnose why an expected layer produced no `PhysicsCollider` entries — if the layer name is missing here, the active camera cannot see it this frame regardless of `--raycast-layer-mask`.

This is a different mask than `RaycastLayerSummaries`, which always reports against the fixed `Physics.DefaultRaycastLayers` set. `RaycastLayerNamesChecked` tracks what was actually clustered; `RaycastLayerSummaries` is a constant discovery aid for "what else could I filter to next."

## Coordinate Conversion

When `ImageCoordinateSystem` is `"top-left-game-view"`, convert raw image pixel coordinates from `screenshot --capture-mode rendering` with the formula returned in `ScreenshotToInputFormula`:

```text
simulate_mouse_x = image_x / resolutionScale
simulate_mouse_y = image_y / resolutionScale + imageToInputOffsetY
```

When `ResolutionScale` is `1.0` and `imageToInputOffsetY` is `0` for rendering captures, raw image pixel coordinates already match mouse-input coordinates. `AnnotatedElements[].SimX/SimY` is already a mouse-input coordinate in that mode, so pass it directly.

For `PhysicsCollider` entries, `SimX/SimY` is a real sampled raycast hit nearest to the reachable cluster centroid. This avoids synthetic center points that may fall into empty space for L-shaped or ring-shaped collider coverage. Always use `SimX/SimY` for clicking; use `BoundsMinX/Y` and `BoundsMaxX/Y` only as a sampled coverage guide.

`--raycast-layer-mask` filters by the requested physics layers and `Camera.main.cullingMask`. A layer that is requested but hidden from the active camera is treated as not visible and will not produce `PhysicsCollider` entries.

For clustered `PhysicsCollider` entries, points where the frontmost EventSystem hit comes from a `GraphicRaycaster` UI element are treated as covered by UI. This includes world-space Canvas UI. `PhysicsRaycaster` and other non-uGUI hits are not treated as UI occlusion. Bounds, screenshot outlines, and `SimX/SimY` are derived from the remaining reachable samples; if every sampled hit in that collider cluster is covered, the collider is omitted from `AnnotatedElements`.

`PhysicsCollider` bounds expand each reachable sample by half the dense raycast sampling step in X and Y, then clamp the result to the captured Game View area. The screenshot overlay draws only the outer edges of those reachable sample cells, so angled, L-shaped, separated, and partially UI-covered hit regions do not become one large rectangle. `BoundsMinX/Y` and `BoundsMaxX/Y` are still an axis-aligned bbox for JSON consumers, may extend up to half a sample step past the visible collider edge, and do not guarantee that every interior point is clickable.

`simulate-mouse-input` (including `--dry-run`) converts internally to Unity Input System coordinates:

```text
unity_x = input_x
unity_y = gameViewHeight - input_y
```

Do not flip Y in the caller.

## Annotation Readability

Annotated screenshots compensate border thickness for `ResolutionScale`, so the saved PNG keeps the intended outline width after downscaling. The neutral contrast borders are 2 output pixels each, and the colored middle border is 4 output pixels. Label outlines are also compensated and are separated from element borders by a 4 output pixel gap.
