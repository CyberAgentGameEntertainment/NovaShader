---
name: uloop-set-game-view-size
toolName: set-game-view-size
description: "Set or inspect the Unity Game View custom rendering resolution."
---

# uloop set-game-view-size

Set or inspect the Unity Game View custom rendering resolution.

## Usage

```bash
uloop set-game-view-size [--width <width> --height <height>]
```

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `--width` | integer | unset | Target Game View rendering width in pixels. Provide with `--height` to change the resolution. |
| `--height` | integer | unset | Target Game View rendering height in pixels. Provide with `--width` to change the resolution. |

## Output

Returns JSON containing:

- `Success`: Whether the request completed.
- `PreviousWidth` / `PreviousHeight`: Resolution before the request.
- `CurrentWidth` / `CurrentHeight`: Resolution after the request.
- `Changed`: Whether the resolution changed.
- `Message`: Human-readable status.

## Examples

```bash
# Read the current custom rendering resolution
uloop set-game-view-size

# Set a Full HD custom rendering resolution
uloop set-game-view-size --width 1920 --height 1080
```

## Notes

- Width and height must be provided together when changing the resolution.
- The tool uses Unity's public `UnityEditor.PlayModeWindow` API and does not require reflection.
