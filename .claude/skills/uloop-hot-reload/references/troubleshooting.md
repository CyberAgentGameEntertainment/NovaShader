# Hot Reload Troubleshooting

## Reading `--status` and `InvocationCount`

`uloop hot-reload --status` lists the methods whose bodies are currently replaced,
without applying or reverting anything. It cannot be combined with `--files` or
`--revert-all`. Patches are static Editor state, so the answer is authoritative: after
a domain reload it reports zero patched methods, which is exactly when an
`ActivePatchTotal` remembered from an earlier response has gone stale.

Each `Active` row's `InvocationCount` counts calls into the patched body since that patch
was applied. Reloading the same source with no edits after a fully applied reload (a run
with no Skipped or Failed outcomes) reports `AlreadyActive` and the row carries
the live `InvocationCount`; re-running after a real edit replaces the patch and resets it to zero. When `InvocationCount` is 0 on an `Active` row, `Reason` notes that the method has not run since this patch was applied: calls that already finished do not re-run, and the patched body takes effect the next time this method is called. For initialization-only methods it also names how to trigger that next call. While Unity is
paused — including while a pause-point hit holds the game — the player loop does not
advance, so game-driven calls stop and the count freezes; calls you make yourself (for
example through `uloop execute-dynamic-code`) still increment it. A frozen count during a
pause only means game-driven calls are not running; it says nothing about whether call
sites reach the patch. Resume first
(`uloop control-play-mode --action Resume`, or clear the owning pause point), drive the
game, and only then read `InvocationCount` as a reachability signal.

## When a Patch Reports `Patched` but Behavior Does Not Change

Run `uloop get-logs` first. An exception thrown inside the patched body, or an
error logged while the reload applied, appears there immediately and explains
"Patched but no visible change" faster than any marker-based digging.

`Patched` means the method body was replaced, not that the method ran. Before suspecting
the patch, confirm the method is actually reached: arm `uloop enable-pause-point --mode
trace` on a line inside the edited method body — it resolves against the patched body
directly (see [pause-point-interaction.md](pause-point-interaction.md)) — drive the game, and check the hit
count: zero hits usually means the calling path never reached the method, which no patch
(or compile) can fix — but cached dispatch (a physics message or a pre-bound delegate
resolved before arming) can bypass the marker, so treat zero hits as inconclusive there
and use the log-line fallback in [pause-point-interaction.md](pause-point-interaction.md). To chase an early return inside the method, arm a second marker on the
suspected early-return line. The other known cause is JIT inlining, which the response flags
with a single aggregated warning listing the at-risk methods: `[AggressiveInlining]` methods
always, tiny bodies only when the Editor's Code Optimization mode is Release (the default
Debug mode does not inline them). If `uloop hot-reload --status` shows the method's
`InvocationCount` increasing, the calls you exercised are reaching the patched body and the
warning did not apply to them — call sites you have not exercised may still run inlined old
code. Take both readings while the code is actually being driven — PlayMode running, or your own
`uloop execute-dynamic-code` invocation for Editor-assembly methods; a count frozen during
a pause is not evidence either way.
