---
name: uloop-replay-input
toolName: replay-input
description: "Replay recorded PlayMode keyboard and mouse input. Use for exact gameplay reproduction, E2E runs, or consistent demos from JSON recordings."
---

# uloop replay-input

Replay recorded keyboard and mouse input during PlayMode. Loads a JSON recording and injects input frame-by-frame via Input System with zero CLI overhead. Supports looping and progress monitoring.

Create recording JSON files in the Unity Editor from **Window > Unity CLI Loop > Recordings** with **Start Recording** and **Stop Recording**. There is no CLI command for recording input.

## Usage

```bash
# Start replay (auto-detect latest recording)
uloop replay-input --action Start

# Start replay with specific file
uloop replay-input --action Start --input-path scripts/my-play.json

# Start replay with looping
uloop replay-input --action Start --loop

# Check replay progress
uloop replay-input --action Status

# Stop replay
uloop replay-input --action Stop
```

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `--action` | enum | `Start` | `Start` - begin replaying, `Stop` - stop mid-way, `Status` - check progress |
| `--input-path` | string | auto | Path to the recording JSON. When empty, auto-detects the latest recording in `.uloop/outputs/InputRecordings/` |
| `--no-show-overlay` | flag | - | Hide replay progress overlay |
| `--loop` | flag | - | Loop continuously |

## Deterministic Replay

Replay injects the exact same input frame-by-frame, but the game must also be deterministic to produce identical results. If replay output must be compared across runs, read [references/deterministic-replay.md](references/deterministic-replay.md) before interpreting failures.

## Prerequisites

- Unity must be in **PlayMode**
- **Input System package** (`com.unity.inputsystem`) must be installed; this tool only works with the New Input System.

## Output

Returns JSON with:

- `Success`: Whether the operation succeeded
- `Message`: Status message
- `Action`: Echoes which action was executed (`Start`, `Stop`, or `Status`)
- `InputPath`: Path to recording file (nullable string; populated on `Start` only)
- `CurrentFrame`: Current replay frame index (nullable int)
- `TotalFrames`: Total frames in the recording (nullable int)
- `Progress`: Replay progress (nullable float in 0.0 – 1.0)
- `IsReplaying`: Whether replay is currently active (nullable bool)
