---
name: uloop-find-game-objects
toolName: find-game-objects
description: "Find or inspect Unity GameObjects, especially objects the user currently selected in the Hierarchy. Use for details, components, tags, layers, or name/path searches."
---

# uloop find-game-objects

Find GameObjects with search criteria or get details for currently selected Hierarchy objects.

Use this before `execute-dynamic-code` when identifying or inspecting selected GameObjects. Use `get-hierarchy` instead when you need the child tree, parent-child structure, or descendants under the selection.

## Usage

```bash
uloop find-game-objects [options]
```

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `--name-pattern` | string | - | Name pattern to search |
| `--search-mode` | string | `Exact` | Search mode: `Exact`, `Path`, `Regex`, `Contains`, `Selected` |
| `--required-components` | array | - | Required components |
| `--tag` | string | - | Tag filter |
| `--layer` | integer | - | Layer filter (layer number) |
| `--max-count` | integer | `20` | Maximum number of results |
| `--include-inactive` | flag | - | Include inactive GameObjects |
| `--include-inherited-properties` | flag | - | Include inherited properties in results |

## Search Modes

| Mode | Description |
|------|-------------|
| `Exact` | Exact name match (default). **Trap**: `--name-pattern "Camera"` will not match a GameObject named `"Main Camera"` — use `Contains` or `Regex` for partial matching. A zero-hit `Exact` search returns a `Message` hint pointing this out. |
| `Path` | Hierarchy path search (e.g., `Canvas/Button`) |
| `Regex` | Regular expression pattern |
| `Contains` | Partial name match |
| `Selected` | Get currently selected GameObjects in Unity Editor |

## Output

Returns JSON with:

- `Results` (array): Matching GameObjects, each containing:
  - `Name` (string): GameObject name
  - `Path` (string): Hierarchy path (e.g., `Canvas/Panel/Button`)
  - `IsActive` (boolean): Active state in hierarchy
  - `Tag` (string): GameObject tag
  - `Layer` (number): Layer index
  - `Components` (array): Each entry has `Type` (short name, e.g., `Rigidbody`), `FullTypeName` (e.g., `UnityEngine.Rigidbody`), and `Properties` (array of Inspector-visible `{Name, Type, Value}` pairs)
- `TotalFound` (number): Results returned (after `--max-count` clipping). For multi-selection file export, this is the number exported.
- `ErrorMessage` (string): Top-level failure summary (empty on success)
- `ProcessingErrors` (array): Selected-mode per-GameObject serialization failures, each `{GameObjectName, GameObjectPath, Error}`. Omitted/null or empty on clean runs.

### Multi-selection file export

For `Selected` mode with **multiple** successfully serialized GameObjects, inline `Results` is not populated and the data is written to a file instead. Two extra fields appear:

- `ResultsFilePath` (string): Relative path under `.uloop/outputs/FindGameObjectsResults/`
- `Message` (string): Human-readable summary (e.g., "5 GameObjects exported")

Single-selection and search-mode calls (`Exact`, `Path`, `Regex`, `Contains`) always return inline. No selection (`Selected` mode with empty selection) returns empty `Results` plus a `Message`.
