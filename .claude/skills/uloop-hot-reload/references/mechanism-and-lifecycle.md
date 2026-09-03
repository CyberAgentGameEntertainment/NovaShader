# Hot Reload Mechanism and Lifecycle

## How a Reload Applies

1. Resolves each file to its compiled assembly via `CompilationPipeline`.
2. Rewrites each editable method body into a static shim in an out-of-process Roslyn worker. When an async, iterator, lambda, local-function, or LINQ-query body touches private/internal members, those accesses are rewritten to accessor delegates so the body can compile and run from the shim assembly (the delegation shape in step 4).
3. Compiles the shims against publicized reference copies, loads the result into the Editor domain, and binds every shim type's accessor delegates (`__BindAccessors`) before any patch is applied.
4. Patches each original method with a Harmony transpiler (ID `io.github.hatayama.uloop.hot-reload`) in one of two shapes: transplant copies the shim's IL into the original method, while delegation rewrites the original to forward its arguments to the shim, which runs as normally compiled code.

Re-running on the same method after a real edit replaces its previous patch;
`ActivePatchTotal` tracks the ledger across runs. Reloading a file whose source is
unchanged since the last fully applied reload (a run with no Skipped or Failed
outcomes) is a no-op: each still-active method is reported
as `AlreadyActive`, the existing patch stays in place, and the row carries the live `InvocationCount`.
Edit the file and reload again to apply new changes.

## Convergence and Lifecycle

- The input is the real project source file, so a later `uloop compile` (real compile +
  domain reload) lands the exact same edit permanently. There is nothing to undo first;
  behavior converges by construction.
- Patches and loaded shim assemblies are static Editor state and disappear on the next
  domain reload — that includes entering Play Mode with Domain Reload enabled (the
  default), `uloop compile`, and `uloop run-tests`. `uloop control-play-mode --action Play` warns with
  the counts when it is about to drop patches or pause points. There is no persistence
  and no automatic re-apply.
- Never reflected by hot reload: initializer changes on compiled fields and new
  types. Those always need `uloop compile`. Signature changes — return type,
  rename, parameter list — are handled through the added-member rules and the
  return-type gate in [scope-and-limits.md](scope-and-limits.md): same file, same Editor session, compiled callers
  protected by skip or warning.
  (Added methods and fields are reflected per the rules in [scope-and-limits.md](scope-and-limits.md), but only for the
  current Editor session and only within their own file.)
- A run with `Failed` outcomes still applies the patches from other files — a
  file containing a `Failed` method is left unapplied as a whole, and there is no
  run-level rollback. `Methods` is the authoritative record of which bodies changed.

## Editor-Code Iteration Without PlayMode

Hot reload also patches static methods in Editor assemblies. Combined with
`uloop execute-dynamic-code` invoking the patched method, this gives a
compile-free loop for editor tooling: edit the method body, run
`uloop hot-reload --files <file>`, then re-invoke it via `execute-dynamic-code`
and read the returned value.
