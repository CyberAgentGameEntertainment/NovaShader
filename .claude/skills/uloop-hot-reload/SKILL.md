---
name: uloop-hot-reload
toolName: hot-reload
description: "Hot reload applies method-body edits and can add new methods and fields (added members are visible only to edited code in the same file); it can also change signatures: a return-type change applies only when the same reload (or an earlier one) covers the old signature's compiled callers, while a rename or parameter change applies as an added method and warns about compiled callers it leaves on the old signature. New types, or members other files must reference, require 'uloop compile'."
---

# uloop hot-reload

Replaces method bodies in the running Editor (EditMode or PlayMode) directly from edited
project source files — no domain reload, no attributes, no source markers. Private/internal
member access, static methods, return values, async methods, and iterators all work within
the limits below — including private access inside async, iterator, lambda, local-function,
and LINQ-query bodies. Methods that cannot be patched are reported per method as `Skipped`
or `Failed`; one unpatchable method never aborts the rest of the run.

## Usage

```bash
uloop hot-reload --files Assets/Scripts/Enemy.cs
uloop hot-reload --files Assets/Scripts/Enemy.cs,Assets/Scripts/Boss.cs
uloop hot-reload
uloop hot-reload --revert-all
```

Multiple files are passed as one comma-separated value (or a JSON array); array options
consume exactly one value token.

A brand-new script — or any script under a brand-new `.asmdef` — cannot be hot-reloaded
before its first import: Unity has not compiled it into any assembly yet. Run
`uloop compile` once to import new files, then iterate on them with hot reload.

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `--files` | array | - | Project-relative `.cs` paths whose method bodies should be hot-reloaded. When omitted or empty on apply, selects the `.cs` sources whose bytes changed since the last compile snapshot, capped at 50 changed files per assembly with a warning when the cap trims the list; run `uloop compile` first when no snapshot exists, or pass explicit paths when no changed source is found |
| `--revert-all` | flag | - | Remove every active hot-reload patch and clear the patch ledger. When set, `--files` is ignored |
| `--status` | flag | - | Lists the currently active changes (patched methods and added members) without applying or reverting anything. |

When `--files` is omitted or empty, a source is selected only when its compilation assembly has a
snapshot directory and that source has its own snapshot file. A missing per-file snapshot is
left out rather than guessed as changed, so pass the file explicitly or run `uloop compile`
to establish a complete baseline.

## Status

`uloop hot-reload --status` lists the currently active changes without applying or
reverting anything; it cannot be combined with `--files` or `--revert-all`. Patches are
static Editor state, so after a domain reload it authoritatively reports zero. Each
`Active` row's `InvocationCount` counts calls into the patched body since the patch was
applied — read it as a reachability signal only while the code is actually being driven;
interpretation rules are in `references/troubleshooting.md`.

## How It Works

Each file resolves to its compiled assembly, every editable method body is rewritten
into a static shim by an out-of-process Roslyn worker (private/internal access becomes
accessor delegates where needed), the shims compile against publicized reference copies
and load into the Editor domain, and each original method is patched with a Harmony
transpiler (ID `io.github.hatayama.uloop.hot-reload`). Re-running after a real edit
replaces the patch; an unchanged file after a fully applied reload reports
`AlreadyActive` rows and changes nothing. With a compile-time source baseline, only
methods whose bodies actually changed are patched (`UnchangedTotal` counts the rest),
and a patched body that matches the baseline again is unpatched on that run.

## Scope in Brief

- Patched: ordinary method bodies and property getters with a body.
- Added members: new methods and fields apply, visible only to edited code in the same
  file, and vanish on any compile or domain reload (an Editor-session illusion). New
  types, cross-file references, reflection, serialization, and Unity message discovery
  need `uloop compile`.
- Signature changes (return type, rename, parameters) follow the added-member rules. A
  return-type change is gated: it is `Skipped` unless the same reload — or an earlier one —
  has patched every live compiled caller of the old signature. A rename or parameter-list
  change is not gated: it follows the delete rules — the old signature is reported removed,
  and a `Warnings` entry names each compiled call site left on the old behavior until
  `uloop compile`.
- Constructors, operators, setter/init/indexer accessors, and event accessors are
  `Skipped`; finalizers and interface members are silently not applied. `const` and
  other outside-body edits never change runtime behavior (drift is warned where
  detectable).
- A reload applies each file all-or-nothing: any `Failed` method leaves that file
  unapplied; patches in other files still apply.

Full rules and the `Skipped`/`Failed` condition tables: `references/scope-and-limits.md`.

## Workflow

Treat hot reload as the exploration phase and `uloop compile` as the landing phase:
keep edits inside the edited files, collect structural changes, and compile once —
every compile drops all patches and pause points and resets the PlayMode session.
One-shot methods (`Awake`, `Start`, initialization helpers) patch successfully but show
no effect on the call that already ran; the response marks them with `LifecycleNote`.
For values you expect to tune while playing, expose a static property getter instead of
a `const`.

## Reference Guides

All files live in `references/` beside this skill; read the one whose trigger matches:

- `references/scope-and-limits.md` — full scope rules: added members, signature changes, `Skipped`/`Failed` tables, source baselines, one-shot code, tunable getters.
- `references/mechanism-and-lifecycle.md` — patch mechanism, convergence, what survives which reload, Editor-code iteration without PlayMode.
- `references/troubleshooting.md` — `Patched` but no behavior change, JIT inlining, reading `--status` and `InvocationCount`.
- `references/pause-point-interaction.md` — how patches re-target or suppress armed pause points; one-way reachability checks.
- `references/output.md` — every response field: `ErrorCode`, `NextActions`, `Methods` rows, `Warnings`, totals.
