# Hot Reload and Pause Points

Both patch shapes discard the original IL and any prior transpiler output on the patched
method, so armed source pause points cannot survive a patch unchanged. Instead of
enforcing exclusivity, every patch transition re-targets them:

- Applying a patch re-resolves each armed marker on the edited method against the
  patched body. A marker whose line still resolves keeps firing at the edited line —
  the apply response reports those ids in a `Warnings` entry and `pause-point-status`
  shows `RetargetedToHotReloadPatch: true`. A marker whose line no longer resolves is
  suppressed instead: the apply response lists it, and status shows
  `SuppressedByHotReload: true` with the reason in `SuppressedByHotReloadReason`.
- Enabling a new pause point on a currently patched method resolves against the patched
  body directly. `PAUSE_POINT_PATCHED_BY_HOT_RELOAD` is returned only when the line
  cannot be mapped onto it (a stale line map or a superseded generation).
  When the compiled line range of the patched method is known, the failure message also reports it, so you can see how far the edited file's line numbers have shifted from the compiled source.
- `uloop hot-reload --revert-all` (or reverting a method's patch) re-targets armed
  markers back onto the compiled body; a marker whose line no longer resolves there
  stays suppressed with a reason until `uloop compile` and a re-enable.

Suppressed markers are never cleared automatically — they keep their identity and fire
again as soon as a transition restores their line. The practical workflow: iterate with
hot reload and place pause points on edited lines in either order — enable then patch,
or patch then enable. `uloop compile` is needed only when a marker stays suppressed
because its line no longer resolves in any live body.

A pause point inside a Unity physics message (`OnCollisionEnter2D`, `OnTriggerEnter`,
and similar) or inside a method already bound into a delegate before enable can stay
at zero hits even though the body runs: Unity may have resolved that dispatch path
before the marker was armed (the pause-point skill's troubleshooting covers recovery).
Hot-reloading a temporary log line into the same body gives a one-way reachability
check — the log appearing (read it with `uloop get-logs`) proves the body ran even
though the marker missed. The log staying absent proves nothing, because the same
cached dispatch can bypass a hot-reload patch too.
