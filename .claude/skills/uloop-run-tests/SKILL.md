---
name: uloop-run-tests
toolName: run-tests
description: "Run Unity Test Runner and report detailed results. Use for EditMode/PlayMode tests, change verification, or failure diagnosis."
---

# uloop run-tests

Execute Unity Test Runner. When tests fail, NUnit XML results with error messages and stack traces are automatically saved. Read the XML file at `XmlPath` for detailed failure diagnosis.

`uloop run-tests` automatically compiles pending script changes before running tests. Pass `--skip-compile` only while validating active hot-reload patches, because the compile clears those patches; otherwise let the default compile surface errors and run against current scripts. `--skip-compile` skips only the CLI-side compile: Unity still imports script edits saved since the last compile, and that import reloads the domain as soon as the run releases its assembly lock, discarding active patches and ending the request.

Before executing tests, `uloop run-tests` handles unsaved loaded Scene and Prefab Stage changes according to `--unsaved-changes` (default `save`): `save` writes them first, `fail` stops if any remain, and `discard` reloads disk state so tests run against saved files. Untitled scenes cannot be discarded and fail. If the chosen mode cannot proceed, it returns `Success: false`, keeps `TestCount` at `0`, lists the items in `Message`, and does not start the Unity Test Runner.

Active pause points are automatically cleared (the underlying code patches are removed as well) before test execution begins. Cleared IDs are reported in the response's `ClearedPausePointIds` field.

A test run can end by discarding active hot-reload changes: script edits imported during the run are compiled when the test runner releases its assembly-reload lock, and that deferred domain reload wipes the patches even though the tests themselves ran patched. The response's Warning field reports this; re-apply 'uloop hot-reload' or bake the edits in with 'uloop compile' before the next Play or test run.

`NoTestsFound` means zero tests matched — not a test failure. Check `NoTestsFoundExplanation` and `Message` for asmdef hints. When an unfiltered run finds no tests and the project has no test assembly for the TestMode, `ProposedTestAsmdef` carries a ready-to-write `.asmdef`: save `Content` at `AssetPath`, move the test scripts under that folder, then compile and rerun.

## Usage

```bash
uloop run-tests [options]
```

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `--test-mode` | string | `EditMode` | Test mode: `EditMode`, `PlayMode` |
| `--filter-type` | string | `all` | Filter type: `all`, `exact`, `regex`, `assembly`, `class` |
| `--filter-value` | string | - | Filter value (test name, pattern, assembly, or class name) |
| `--unsaved-changes` | string | `save` | `save` writes unsaved Scene/Prefab Stage changes; `fail` stops if any remain; `discard` reloads disk state (Untitled scenes fail) |
| `--skip-compile` | flag | - | Skip the CLI-side compile before running tests; use only while validating active hot-reload patches. Unity still imports script edits saved since the last compile. |
| `--timeout-seconds` | integer | `600` | Maximum seconds to wait for RunFinished before canceling the await (max `1500`). Increase for long suites; on timeout the Test Runner may still be running until stop handling lands |
| `--respect-enter-play-mode-settings` | flag | - | PlayMode only: keep the project's Enter Play Mode settings instead of forcing Domain Reload off. A Domain Reload during the run is survived; the result is recovered after the reload. Use for projects whose libraries require a Domain Reload on Play entry. |

By default PlayMode still forces Domain Reload off. With `--respect-enter-play-mode-settings`, a Domain Reload may run and the command takes longer; pause-point and hot-reload notes are omitted from a result recovered after reload. Canceling the CLI (Ctrl-C) does not stop the Unity-side run on this path.

exact matches the full test name (Namespace.Class.Method). class runs every test of one class by bare or namespace-qualified name, e.g. --filter-type class --filter-value PlayerTests; the name is matched literally and whole, so PlayerTests does not run EnemyPlayerTests. regex matches a .NET regex against full test names, e.g. --filter-type regex --filter-value '^MyGame\.Tests\.'

## Output

Returns JSON with:

- `Success` (boolean): Whether all tests passed
- `Status` (string): Machine-readable execution status such as `Passed`, `Failed`, `NoTestsFound`, or `ExecutionFailed`
- `HasFailures` (boolean): Whether any discovered test failed
- `Message` (string): Summary message
- `NoTestsFound` (boolean): Whether Unity Test Runner discovered zero matching tests
- `NoTestsFoundExplanation` (string): Agent-facing explanation when `NoTestsFound` is true; empty otherwise
- `CompletedAt` (string): ISO timestamp when the run finished
- `TestCount` (number): Total tests executed
- `PassedCount` (number): Passed tests
- `FailedCount` (number): Failed tests
- `SkippedCount` (number): Skipped tests
- `XmlPath` (string): Path to NUnit XML result file. Empty string when no XML was saved (typically on `Success: true`); populated only when tests failed and the XML file exists on disk.
- `ClearedPausePointIds` (string[], optional): IDs of pause points that were cleared before test execution. Omitted from JSON when no pause points were active.
- `FailedTests` (array, optional): Up to 10 failed leaf tests with `FullName`, `Message`, and when the stack trace contains a path:line location, `File` and `Line`. Omitted when no tests failed. When `FailedCount` is greater than 10, `Message` ends with `first 10 of N failures listed; see XmlPath for full results.`
- `SkippedTests` (string[], optional): Up to 10 full names of skipped leaf tests. Omitted when no tests were skipped. When `SkippedCount` is greater than 10, only the first 10 names are listed.
- `ProposedTestAsmdef` (object, optional): `AssetPath` and `Content` of a ready-to-write test `.asmdef` (test-assembly wiring plus references to the project's assemblies under test). Present only when an unfiltered run found no tests and no test assembly exists for the TestMode.
- `CompileNote` (string, optional): States that the automatic compile ran and succeeded before the tests and names `--skip-compile` as the opt-out. When the compile response carried a Warning (for example active hot-reload changes dropped by the domain reload), the note repeats it. Omitted when `--skip-compile` was passed; a failed compile returns the compile error response instead.

### XML Result File

When tests fail, NUnit XML results are automatically saved to `{project_root}/.uloop/outputs/TestResults/<timestamp>.xml`. The XML contains per-test-case results including:

- Test name and full name
- Pass/fail/skip status and duration
- For failed tests: `<message>` (assertion error) and `<stack-trace>`
