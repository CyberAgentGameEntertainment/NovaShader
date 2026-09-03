# Hot Reload Scope and Limits

Only ordinary method declarations and property getters with a body are patched.
Constructors, operators, and explicit event accessors are reported as `Skipped`
when edited (with a verified baseline, unchanged members of those kinds produce
no row). Finalizers and `interface` members (including default interface
implementations) are never scanned: **edits** to them produce **no per-method
entry at all** and are silently not applied — use `uloop compile` for those.
Adding a constructor, operator, or explicit event accessor is reported as
`Skipped` as well, same as an edit to an existing one.

## Added methods and fields

Hot reload can add new methods and fields alongside body edits, under one hard rule:
an added member is visible only to edited code in the same file. Compiled, unedited
code cannot see it, and neither can anything that resolves members by name at
runtime: reflection (`GetType().GetMethod("NewM")` returns `null`), Unity's message
discovery (an added `Update` or `OnCollisionEnter` on a `MonoBehaviour` is never
invoked — a `Warnings` entry names it), UnityEvent/inspector wiring, and
serialization. Referencing an added member from a different file fails that file's
hot reload with the usual new-member hint; run `uloop compile` instead.

An added method reports its own row with Kind `Added`; the edited methods that call
it report `Patched` as usual. Added `virtual`/`override`/`abstract` methods, explicit
interface implementations, and generic methods are `Skipped`; a method-group or
delegate reference to an added instance method skips the referencing method instead.
Pause points cannot bind to lines inside an added method — enabling one there fails
with the normal not-found error.

An added field's values live in a side table that follows each instance's lifetime
(statics live per domain). Its initializer does not run at construction time; it runs
on the field's first access from edited code — once per instance, or once per domain
for statics. Initializer expressions are limited to literals and externally visible
static calls (`= 5`, `= Math.Abs(x)`); anything touching the host type or instance
state skips the field's readers and writers with a per-method reason. Added `const`
values are folded into edited bodies as literals, like `nameof`. Pause-point
`CapturedVariables` never includes added fields; `enable-pause-point` warns when the
resolved type has any — read them from a patched method body or
`uloop execute-dynamic-code` instead.

Added members are an Editor-session illusion. Any real compile or domain reload
drops them all: added methods disappear from the ledger and added-field values are
discarded — they do not migrate into the compiled field's initializer semantics.
Deleting an added member from the edit and re-applying (or reverting the file to its
compiled source) removes it from the ledger on that run. Deleting a *compiled*
member is reported in `Warnings`, but its IL remains callable from unedited code
until `uloop compile`.

Adding a constructor, operator, or explicit event accessor is still out of
scope and is reported as `Skipped`, same as edits to them. Adding a type
(`class`, `struct`, `enum`, `record`), a property, an event, or an indexer
is still out of scope. Added properties are reported per member: the
property's getter appears as a `Skipped` row that says to use a 'const' or
a plain added field for the value, or to run 'uloop compile'. Types, events,
and indexers are not reported per member — no `Skipped` row names them; at
most they surface as outside-body drift in `Warnings`. Treat their silence
as "not applied" and land them with `uloop compile`.

Outside method bodies, only member additions (previous section) take effect.
Every other declaration edit — changing a `const` value, a compiled field's
initializer, an attribute — leaves runtime behavior unchanged even though the
response reports `Success` — shims resolve those symbols
against the already-compiled assembly, and C# bakes `const` values into IL at compile
time. Changed `const` values (including enum member values) are detected and reported
as a `Warnings` entry naming the constant and both values. The scan includes
changed sibling files in the same assembly, not only the file passed to
`--files`. When a verified source
baseline is available (next paragraph), other outside-body drift — existing-field
initializers, attributes, and other declaration edits — is reported as a `Warnings`
entry as well (handled added members and reported removed members are excluded
from this generic warning); without a baseline it stays silent. Either way, use
`uloop compile` for such edits.

## Signature changes: return type, rename, parameters

Changing a compiled method's return type is applied as a remove-plus-add: the old
method stays in the compiled assembly (like any removed member), the new signature
becomes an added method with its own `Added` row, and the edited methods that call
it report `Patched`. Every added-member rule applies — same-file visibility, the
Editor-session illusion, and the `virtual`/generic/interface exclusions.

A gate protects compiled callers: the change applies only when every live compiled
call site of the old signature is patched by the same file's reload. A caller in
another file — even one edited in the same run — or an *unedited* method in the
same file (an implicit `int`→`long` widening can leave a caller's source
untouched) would keep calling the old method silently, so the run reports the
changed method and its edited callers as `Skipped` instead; land the change with
`uloop compile`. When every uncovered caller is in the edited file itself, the
`Skipped` reason names those callers: editing their bodies and reloading again
applies them together without `uloop compile`.
Call sites inside methods that the same edit removes or
re-signatures do not gate: those compiled bodies are already stale, and anything
still reaching them stays on the consistent old behavior.
If an earlier reload already patched the compiled call sites, a later signature change applies without editing the callers; the response then carries a warning naming the call sites this run re-applied on the new signature.

Renaming a method or changing its parameter list follows the delete rules rather
than the gate: the new signature is an ordinary added method, the old one is
reported removed, and a `Warnings` entry names each compiled call site of the old
signature that the reload leaves unpatched — those call sites keep the previous
behavior until `uloop compile`. Deleting a method emits the same warning when
compiled callers remain.

Field declarations are stricter: when a compiled field's type — or its `static`/
`const` modifier — differs from the edited source, every edited method that reads
or writes that field is `Skipped` with a per-method reason. Retyped storage has no
session illusion; run `uloop compile`.

## Explore with hot reload, land structure with compile

Treat hot reload as the exploration phase and `uloop compile` as the landing phase. While
diagnosing or tuning, keep every edit inside existing method bodies — inline a would-be
helper's logic at its call site for now instead of extracting it. New helper methods
and fields can now be explored directly with hot reload inside the same file. When
the change needs a new type, cross-file visibility, runtime name-based lookup, or
serialization, collect those and run `uloop compile`
once: every compile triggers a domain reload that drops all active patches and pause points
and resets the running PlayMode session, so compiling member-by-member pays that cost
repeatedly. After the one compile, re-enter PlayMode and continue exploring on the freshly
compiled code.

## One-shot code: a patch only changes the next call

Hot reload changes what a method does on its *next* call — it never re-runs a call that
already happened. Methods that run exactly once per session (`Awake`, `Start`, `OnEnable`,
initialization helpers called from them, anything that seeds state at startup) patch
successfully but show no effect: the one call they get is already in the past when the
patch lands. The response marks these with `LifecycleNote` (see Output) — both direct one-shot
lifecycle messages and methods whose every compiled caller is a one-shot lifecycle message on a
`MonoBehaviour`. The caller check is conservative: when the scan cannot prove exclusivity (a
missing assembly, reflection, or event-driven calls), the note is omitted. To see an
initialization change take effect, run `uloop compile` and restart
Play Mode — with Domain Reload enabled (the default), a fresh Play entry reloads the
domain and drops the patch, so the patched body alone cannot carry the change into the
next session. Better, keep values you expect to
tune out of one-shot paths entirely: read them in a body that runs per frame or per event,
and patch that body instead.

## Tunable values: prefer a getter over a const

`const` edits never take effect through hot reload: C# bakes const values into every
call site at compile time. When you expect to tune a value while Play Mode is running
(speeds, amplitudes, sensitivities), expose it as a static property getter instead:

    public static float HeightAmplitude => 5f;

A getter body is an ordinary patchable method body, so editing the literal and running
`uloop hot-reload` updates every consumer on its next call — across all files, without
restarting Play Mode. JIT-inlined call sites are the exception — the reload response's
`Warnings` lists the at-risk methods (see [troubleshooting.md](troubleshooting.md)).
Keep `const` for values you never tune at runtime.

This works only for consumers that read the getter on a live call path — a per-frame
`Update`, a physics step, an event handler. A consumer that read the getter once during
initialization and cached the value in a field never observes the new value: the patch
lands, but nothing reads the getter again (the one-shot rule above).

Each `uloop compile` also establishes a per-assembly source baseline: a snapshot of
the sources exactly as they were compiled, captured after the compile's domain reload
and adopted only once it verifies against the compiled assembly's PDB checksums. With
a baseline, hot reload patches only the methods whose bodies actually changed;
unchanged methods are left untouched and counted in `UnchangedTotal` (formatting,
comments, and line-ending differences count as unchanged). A run where every method
is unchanged succeeds with nothing patched.
Convergence works in both directions: a currently patched method whose body matches
the baseline again is unpatched on that run — the compiled IL comes back,
`ActivePatchTotal` drops, and its pause-point block lifts.
Without a baseline — for example before
the first compile after installing or updating the package — every editable method in
the file is patched and a `Warnings` line reports the fallback; run `uloop compile`
to establish the baseline.

Property getters with a body (including expression-bodied properties) are patched
like ordinary methods. Setter, init, and indexer accessors with explicit bodies are
reported per-accessor as `Skipped`, so an edited accessor never disappears from the
response silently; with a verified baseline, accessors unchanged from it produce no row.

Subscribing to or unsubscribing from a field-like event (`+=`/`-=`) inside an edited
body works. Methods that raise the event are reported as `Skipped` (see the table
below) — raising is only expressible inside the declaring type, which a shim is not.

## Skipped — reported per method, never flips `Success`

| Condition | Why |
|-----------|-----|
| Method on a `partial` type (including a type nested inside a partial outer type) | A single file cannot provide a complete semantic model |
| Method on a struct (value type) | Value-type patching is out of scope |
| Generic method, or method on a generic type | Harmony cannot safely patch open generics |
| Explicit interface implementation | Dotted metadata names cannot be expressed as shim identifiers |
| No body (`abstract` / `extern`) | Nothing to transplant |
| Body contains a `base.` call | `base` cannot be expressed from outside the type |
| Private/internal access inside an async/iterator/closure body has no accessor-delegate shape | Conditional access (`?.`), `??=`, indexers, static field writes, initializer member assignments, compound writes whose receiver could be evaluated twice, assignments whose value is consumed, and calls with `ref`/`out`/`in`, named, optional, or `params` arguments (or to extension/generic/by-ref-returning methods) cannot be rewritten to accessor delegates |
| An async/iterator/closure body references a private/internal type | Accessor delegates rescue member access, not type references; the body still cannot JIT-compile from the shim assembly |
| Property setter, init, or indexer accessor with an explicit body | Accessor patching covers getters only; `uloop compile` applies setter/init/indexer edits |
| Constructor (instance or static), operator, conversion operator, or explicit event accessor (add/remove) | Out of scope for v1; `uloop compile` applies these edits |
| Method raises, invokes, or reads a field-like event (anything beyond `+=`/`-=`) | C# only allows `+=`/`-=` on an event outside its declaring type, so the raising body cannot compile from the shim assembly |

## Failed — flips `Success` to `false`

| Condition | Notes |
|-----------|-------|
| File does not belong to any compiled assembly | Per-file entry with `Method` = `(file)`; only `Assets/` and `Packages/` sources resolve |
| Resolved assembly name is missing from CompilationPipeline | Per-file entry with `Method` = `(file)`; Unity may have mapped a not-yet-imported `.asmdef` onto a predefined assembly. Run `uloop compile` first |
| Script is not in the last compiled assembly's source list | Per-file entry with `Method` = `(file)`; a newly added script is not hot-reloadable until `uloop compile` |
| Loaded assembly differs from the one on disk (pending compile) | Run `uloop compile` first, then retry |
| Source file fails to parse | Per-file entry carrying the parse errors |
| Method signature not found in the loaded assembly | Usually a stale assembly; run `uloop compile`. In-file renames and signature changes are classified as added members before reaching this point |
| Shim compile error (e.g. the body calls a member that does not exist yet) | Failing methods are isolated: each reports `Failed` with its own compiler errors (plus the `uloop compile` hint when they indicate a missing member). When errors cannot be attributed per method, the whole file reports one `(shim-compile)` entry; if only one method was edited, the failure is attributed to that method's name instead |
| Patch rejected or crashed at apply time (e.g. `[BurstCompile]`, a patch-engine emit failure) | The entry carries the rejection reason or the underlying engine error |
| Accessor binding failed for a shim type | The source references a member the compiled assembly does not have yet; every delegation-patched method in that shim type reports the binder error — run `uloop compile` and retry |
| The signature-change gate could not finish the run safely — the retry that skips a gated change failed, or shim-compile isolation dropped an edited caller that had covered a change | Per-file entry with `Method` = `(signature-change-gate)` carrying the specific cause; nothing from the file is applied — fix the failing edit or run `uloop compile` |

A reload applies each file all-or-nothing: when any method in a file fails to compile or validate, nothing from that file is applied and patches from earlier reloads stay active. The one exception is a Harmony patch-engine failure in the middle of applying a validated file; that run reports itself as partially applied and recommends 'uloop hot-reload --revert-all'.
