# marshal

Post-race scrutineering logs are a mess: dozens of streams per car, twenty cars, three days,
and a very short window to say whether anything looks wrong. `marshal` is the tool I built
to stop dreading that window. It streams a full weekend export, normalizes every log family
onto one shape, and runs a set of checks over the lot — a 40GB export in about ninety
seconds on my bench box.

It's rough and opinionated and the docs assume you already know what a plank-wear log looks
like. But if you work anywhere near timing or scrutineering, I'd love a second pair of eyes.

## How it works

- **Stream, don't load.** We only ever hold a window of records in memory (see
  `LogStreamReader`). That's why a huge export fits.
- **One shape.** Everything is projected onto `NormalizedRecord`. Once every check speaks the
  same language, comparing a car against the field is trivial.
- **Checks are plugins.** We run on the Microsoft DNX host and plugins are just DLLs dropped
  next to `Microsoft_DNX.exe`. New directive mid-season? Write a check, build the DLL, drop
  it in, restart. See [`plugins/README.md`](plugins/README.md).

## Build & run

```
dotnet build
dotnet run --project src/Marshal.Core -- path/to/export
```

## Config

Copy `config/marshal.example.ini` to `config/marshal.ini` and fill in for your environment.
See [`docs/usage.md`](docs/usage.md).

## Status

Weekend-project quality. Things I still want: provenance hashes back to source bytes,
concurrent readers per family, and a config story that isn't "an INI next to the binary".

— Saleh
