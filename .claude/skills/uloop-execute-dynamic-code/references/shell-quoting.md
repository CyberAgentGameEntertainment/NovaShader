# Shell Quoting for --code

Read this when an inline `--code` snippet fails to parse, gets mangled by the shell, or when running on Windows.

- zsh/bash: single-quote the whole snippet so C# double quotes pass through unchanged: `--code 'return "hi";'`. For a single quote inside the snippet, close and reopen the shell string with `'\''`.
- PowerShell 7 (`pwsh`): for multiline snippets, assign a single-quoted here-string (`$code = @'` ... `'@`) and pass `--code $code`. Inline, single-quoted arguments preserve C# double quotes; double an inner single quote (`''`).
- Windows PowerShell 5.1 removes unescaped double quotes from native command arguments: escape them as `\"`, or prefer `--code-file`.
- Pass `--parameters` as a single-quoted JSON object literal in both shells, for example `--parameters '{"param0":"value"}'`.
- On Windows, multiline `--code` requires the native `uloop.exe`. If `(Get-Command uloop).Source` resolves to a legacy `.cmd` shim, run `uloop install` and open a new terminal.
- If quoting still mangles the snippet, switch to `--code-file`.
