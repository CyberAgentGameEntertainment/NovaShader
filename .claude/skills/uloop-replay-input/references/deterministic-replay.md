# Deterministic Replay

Replay injects input frame-by-frame, so the game must produce identical results given identical input on each run. The following patterns break determinism and should be avoided in replay-targeted code:

| Avoid | Use Instead | Why |
|-------|-------------|-----|
| `Time.deltaTime` for movement | Fixed per-frame constant (e.g. `MOVE_SPEED = 0.1f`) | Delta time varies between runs even at the same target frame rate |
| `Random.Range()` / `UnityEngine.Random` | Seeded random (`new System.Random(fixedSeed)`) or remove randomness | Random sequences differ between runs |
| `Rigidbody` / Physics simulation | Kinematic movement via `Transform.Translate` | Physics is non-deterministic across runs |
| `WaitForSeconds(n)` in coroutines | Frame counting | Real-time waits depend on frame timing |
| `Time.time` / `Time.realtimeSinceStartup` | Frame counter (`Time.frameCount - startFrame`) | Time values drift between runs |
| `FindObjectsOfType` without sort | Serialized object lists or sorting by a stable project-defined key | Unity object iteration order is not replay-stable |
| `async/await` with `Task.Delay` | Frame-based waiting | Real-time delays are non-deterministic |

Set `QualitySettings.vSyncCount = 0` before `Application.targetFrameRate = 60` or another fixed target to reduce frame timing variance. See `InputReplayVerificationController` for a complete example of deterministic game logic.
