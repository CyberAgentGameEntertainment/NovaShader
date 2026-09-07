---
name: uloop-get-hierarchy
toolName: get-hierarchy
description: "Get the Unity scene hierarchy as a structured tree. Use for parent-child structure, descendants, roots, or subtrees under objects the user currently selected."
---

# uloop get-hierarchy

Get Unity Hierarchy structure from the whole scene, a root path, or selected Hierarchy objects.

Use this for hierarchy structure, especially descendants under the current selection. Use `find-game-objects --search-mode Selected` when you need selected object details or component properties.

## Usage

```bash
uloop get-hierarchy [options]
```

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `--root-path` | string | - | Root GameObject path to start from |
| `--max-depth` | integer | `-1` | Maximum depth (-1 for unlimited) |
| `--no-include-components` | flag | - | Exclude component information |
| `--no-include-inactive` | flag | - | Exclude inactive GameObjects |
| `--include-paths` | flag | - | Include full path information |
| `--use-components-lut` | string | `auto` | Use LUT for components (`auto`, `true`, `false`) |
| `--use-selection` | flag | - | Use selected GameObject(s) as root(s). When set, `--root-path` is ignored. |

## Output

Returns JSON with:

- `Message` (string): Human-readable guidance pointing at the saved file
- `HierarchyFilePath` (string): Filesystem path to the JSON file that contains the actual hierarchy data

The hierarchy itself is **not** in the response — it is written to the file at `HierarchyFilePath`. Open that file to read the `Context` and `Hierarchy` payload (GameObject tree, components, etc.).
