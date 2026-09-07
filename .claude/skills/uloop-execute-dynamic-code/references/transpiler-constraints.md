# Known Transpiler Constraints

- Literals inside recognized static local function bodies are kept inline automatically. Unsupported header shapes (generic `where` clauses, tuple return types, statement lambdas inside expression bodies) may still hoist literals and surface CS8421; remove `static` or rewrite the helper.
- Static lambdas (`static x => ...`) cannot reference hoisted literals and surface CS8820; remove `static` from the lambda or use a non-static local function.
- Integer literals are hoisted as `int` values. APIs that require `byte` components (for example `new Color32(255, 0, 0, 255)`) need explicit casts such as `(byte)255` even when plain Unity scripts accept uncast numeric literals.
