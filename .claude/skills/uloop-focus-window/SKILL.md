---
name: uloop-focus-window
description: "Bring the Unity Editor window to front. Use when Unity must be visible for visual checks or user-facing interaction."
---

# uloop focus-window

Bring Unity Editor window to front using OS-level commands.

## Usage

```bash
uloop focus-window
```

## Output

Returns JSON with:

- `Success`: Whether the focus operation succeeded
- `Message`: Status message (e.g. `Unity Editor window focused (PID: 12345)`, or the failure reason such as `Unity project not found` / `No running Unity process found for this project` / `Failed to focus Unity window: <reason>`)

## Notes

- Useful before `uloop screenshot` to ensure the target window is visible
