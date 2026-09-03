---
name: uloop-compile
toolName: compile
description: "Compile the Unity project and report errors/warnings. Use after C# edits."
---

# uloop compile

Execute Unity project compilation.

## Usage

```bash
uloop compile [--force-recompile] [--no-wait-for-domain-reload] [--stop-on-external-scene-changes] [--timeout-seconds <seconds>]
```

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `--force-recompile` | flag | - | Full recompile plus domain reload. Almost never needed: a plain compile already detects externally edited files, and the forced reload can freeze large projects and come back as `COMPILE_RESULT_UNKNOWN`. |
| `--no-wait-for-domain-reload` | flag | - | Return before Domain Reload completion |
| `--stop-on-external-scene-changes` | flag | - | Stop before compilation if open Scene files changed externally instead of auto-reloading them |
| `--timeout-seconds` | integer | 600 | Maximum seconds the CLI waits for the compile to finish before returning COMPILE_WAIT_TIMEOUT (default 600). Values above 1200 exceed the Unity-side result retention window (20 minutes) and weaken post-timeout recovery. |

## When to use --force-recompile

`--force-recompile` is almost never needed. Detecting changed files is Unity's job: even when
files were edited outside the Editor, a plain `uloop compile` refreshes assets and runs every
recompilation the changes require. "The files were changed externally, so recompile everything
just in case" is not a valid reason.

Why to avoid it:

- On large projects a full recompile plus domain reload can freeze Unity for a long time.
- The result crosses a domain reload, so it often comes back as `COMPILE_RESULT_UNKNOWN` and
  does not work as a verification step.
- It puts the Editor into the unstable just-after-reload state for no benefit.

The one legitimate use case: you need warnings hidden by other asmdefs surfaced by a full
build. Otherwise always run plain `uloop compile`.

## Output

Returns JSON:

- `Success`: boolean or null
- `ErrorCount`: number or null
- `WarningCount`: number or null
- `Message`: string
- `ErrorCode`: string or null. `COMPILE_ALREADY_IN_PROGRESS` when Unity is already compiling, `COMPILE_EDITOR_UPDATING` when the editor is updating, `COMPILE_RESULT_UNKNOWN` after a forced recompile that did not return a definitive result.
- `NextActions`: string array or null. Corrective steps derived from the errors, e.g. the assembly that declares an unresolved namespace (CS0234), or, for CS0246 in a script under an asmdef, a reminder to check that asmdef's references (the type may instead be a typo — decide from the error).

When Unity stops compiling before the finish callback (`Success: null`, indeterminate), `Message` keeps the get-logs pointer and appends `Recent Console errors:` with the last few Console errors (typically the asmdef or compiler error that aborted the compile), so fix from that list before reaching for `uloop get-logs`.

## Troubleshooting

When Unity's API Updater asks for consent to rewrite source files during a CLI compile (the 'Script Updating Consent' dialog), uloop declines automatically — source files are never rewritten without explicit user consent — and the response's Warning discloses the decline. The obsolete-API errors the updater would have fixed appear in Errors; fix them in code, or have the user accept the dialog in an interactive Unity session. Outside CLI compiles, and for the separate 'API Update Required' dialog, the modal can still appear and uloop cannot click it: if compile times out while the Editor looks idle, ask the user to answer the dialog — never auto-dismiss it.

A compile that outlives your shell's output window keeps running inside Unity: the call may return with empty or truncated output while the compile is still in progress, and the next uloop command is rejected with "Unity is busy running 'compile'" until it finishes. That rejection is normal single-flight behavior, not a failure — wait and rerun the command; do not restart Unity or rerun compile to recover.
