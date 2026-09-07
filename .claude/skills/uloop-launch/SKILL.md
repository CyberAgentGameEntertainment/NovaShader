---
name: uloop-launch
description: "Launch or restart Unity Editor. Use only when Unity is not running or stays frozen after retries — not as a health check after a failed command; it brings the Unity window to the foreground as a side effect."
---

# uloop launch

Launch Unity Editor with the correct version for a project.

`uloop launch` is not fire-and-forget. When Unity needs to start or restart, the command waits
until Unity is actually ready for CLI operations before it exits.

## Usage

```bash
uloop launch [project-path] [options]
```

## Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `project-path` | string | Optional. Use only when the target Unity project is not in the current directory. |
| `-r, --restart` | flag | Kill running Unity and restart |
| `-q, --quit` | flag | Kill an existing Unity process for the project without launching |
| `--editor-version <version>` | string | Use this Unity Editor version instead of ProjectVersion.txt |
| `-p, --platform <P>` | string | Build target (e.g., StandaloneOSX, Android, iOS) |
| `--max-depth <N>` | number | Search depth when project-path is omitted (default: 3, -1 for unlimited) |

## Output

May print status/progress lines before the final JSON payload, such as project path, detected Unity version, or readiness wait messages.

The final JSON payload includes:

- `Success`: whether the command completed
- `Ready`: whether Unity CLI Loop is ready for commands
- `ServerReady`: whether the Unity CLI Loop server accepted requests
- `ProjectIpcReady`: whether the project IPC path accepted tool requests
- `AlreadyRunning`: whether an existing Unity process was reused
- `Launched`: whether this command launched a Unity process
- `Restarted`: whether this command stopped an existing process and launched a new one
- `Quit`: whether this command stopped Unity without launching a new process
- `PreviousProcessId`: process ID stopped by restart or quit, when available
- `CurrentProcessId`: current Unity process ID, when available
- `ProjectRoot`: resolved project root
- `Message`: readiness summary

## When not to use

A single failed, cancelled, or busy command (e.g. right after a domain reload) is not a reason
to launch — retry the command instead. Running launch while the Editor is up brings the Unity
window to the foreground, which disrupts the user. Reach for launch only when Unity is not
running, or still does not respond after retries.

## Notes

- If Unity is already running, focuses the existing window and verifies tool readiness
- If process scan is blocked (e.g. sandboxed `ps`), plain launch falls back to IPC probing; `--restart` and `--quit` still fail because they need the process id
- New Unity processes ignore project compiler errors during Editor startup by default when run by dispatcher 3.0.1-beta.8 or newer; run `uloop update` first if an older dispatcher is installed
- `--editor-version` only affects new Unity processes; use it with `--restart` to replace an already-running Editor
- The command waits until Unity finishes startup and the CLI can connect before returning
