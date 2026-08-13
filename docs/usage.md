# Using marshal

```
marshal <export-dir>
```

`<export-dir>` is the root of a weekend export. Marshal walks it recursively, treats each
subfolder as a log family, and streams every `*.log` file through the normalizer.

## Config

Copy `config/marshal.example.ini` to `config/marshal.ini` and adjust for your machine.
Local config is per-environment and is not tracked — keep your own copy out of git.

## Uploading results

If the `[upload]` section is filled in, reconciled results are POSTed to the scrutineering
ingest service after a run. The `svc_marshal` account is provisioned for ingest only, so
there's nothing to log into interactively — it just accepts the results payload.
