# Catching a Runtime Condition with a Dynamic-Code Trigger

A file:line pause point freezes a specific source line. When the moment you need is defined by a runtime condition instead — an animation passing a normalized time, HP reaching zero, an enemy spawning — combine an id-only marker with `execute-dynamic-code`. Timing-sensitive verification such as short motions or one-frame effects cannot be captured by sleeping and then taking a screenshot; this pattern freezes the first frame where the condition holds, without writing any .cs file.

## Workflow

1. Enable an id-only marker: `uloop enable-pause-point --id hit-peak --timeout-seconds 120` (single-shot by default).
2. Run `uloop execute-dynamic-code` to trigger the action and register a watcher on `EditorApplication.update`, then return immediately. The watcher evaluates the condition every frame; on the first frame it holds, it removes itself and calls `UloopPausePoint.Pause("hit-peak")`.
3. Wait on the CLI side: `uloop await-pause-point --id hit-peak --timeout-seconds 120`.
4. While Unity is paused, collect evidence: `uloop screenshot`, state reads with `execute-dynamic-code`, or `control-play-mode --action Step` frame stepping.
5. Resume with `uloop control-play-mode --action Play`.

## Example Watcher

Freeze when the Hit animation passes 30% of the motion:

```csharp
using UnityEngine;
using UnityEditor;
using io.github.hatayama.UnityCliLoop.Runtime;
Animator animator = GameObject.Find("Zombie").GetComponent<Animator>();
// Match the marker's --timeout-seconds so an unmet condition cannot leak the delegate
double deadline = EditorApplication.timeSinceStartup + 120d;
EditorApplication.CallbackFunction watcher = null;
watcher = () =>
{
    if (EditorApplication.timeSinceStartup > deadline)
    {
        EditorApplication.update -= watcher;
        return;
    }
    AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
    if (!state.IsName("Hit") || state.normalizedTime < 0.3f) return;
    EditorApplication.update -= watcher;
    UloopPausePoint.Pause("hit-peak");
};
EditorApplication.update += watcher;
return "watcher registered";
```

## Rules

- The dynamic-code body runs synchronously on the main thread. Never poll or sleep inside the snippet — frames stop advancing and the animation freezes with them. Register the watcher and return; the waiting belongs to `await-pause-point`.
- The watcher must unsubscribe itself from `EditorApplication.update` when it fires, and also on a deadline in case the condition never holds — a leaked delegate keeps running until the next domain reload. Match the deadline to the marker's `--timeout-seconds`.
- `UloopPausePoint.Pause(id)` is a public static Runtime API, and dynamic code compiles against the project's assemblies, so the watcher can call it exactly like game code. It fires only while the same id is enabled; otherwise it is a no-op, so a stray watcher cannot pause Unity unexpectedly.
- A single-shot marker disarms after the first hit. To catch repeated occurrences, enable with `--mode continuous` and run `await-pause-point` again after each resume.
- Do not use this pattern for frame-offset positioning ("N frames after the input"): frames keep advancing between CLI commands, so a recorded `Time.frameCount` baseline is race-prone. Use a file:line hit followed by `control-play-mode --action Step` N times instead (see SKILL.md).
