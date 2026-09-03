---
name: uloop-get-logs
toolName: get-logs
description: "Read current Unity Console entries from a running Editor. Use during bug investigation after compile, tests, PlayMode, dynamic code, or immediately after `uloop-pause-point`."
---

# uloop get-logs

Retrieve logs from Unity Console.

## Usage

```bash
uloop get-logs [options]
```

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `--log-type` | string | `All` | Log type filter: `Error`, `Warning`, `Log`, `All` |
| `--max-count` | integer | `100` | Maximum number of logs to retrieve |
| `--search-text` | string | - | Text to search within logs |
| `--include-stack-trace` | flag | - | Include stack trace in output |
| `--use-regex` | flag | - | Use regex for search |
| `--search-in-stack-trace` | flag | - | Search within stack trace |

## Output

Returns JSON with:

- `TotalCount` (number): Total logs available before max-count clipping
- `DisplayedCount` (number): Logs returned in this response (≤ `--max-count`)
- Input filters (`LogType`, `MaxCount`, `SearchText`, `IncludeStackTrace`) are echoed back in the response
- `Logs` (array): Each entry has:
  - `Type` (string): `"Error"`, `"Warning"`, or `"Log"`
  - `Message` (string): Log message body
  - `StackTrace` (string): Stack trace text. Empty when `--include-stack-trace` is `false`.
