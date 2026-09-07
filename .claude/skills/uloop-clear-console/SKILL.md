---
name: uloop-clear-console
toolName: clear-console
description: "Clear Unity Console entries. Use before compile, tests, or debugging when stale logs would hide the current result."
---

# uloop clear-console

Clear Unity console logs.

## Usage

```bash
uloop clear-console [--add-confirmation-message]
```

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `--add-confirmation-message` | flag | - | Add confirmation message after clearing |

## Output

Returns JSON with:

- `Success` (boolean): Whether the clear operation succeeded
- `ClearedLogCount` (number): Total number of log entries that were cleared
- `ClearedCounts` (object): Breakdown by log type
  - `ErrorCount` (number): Errors cleared
  - `WarningCount` (number): Warnings cleared
  - `LogCount` (number): Info logs cleared
- `Message` (string): Description of the result. On failure, this field carries the error summary.
