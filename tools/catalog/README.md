# Optional catalog verify tools

Per-slug scripts referenced from `optional_tool_path` in [plugins.catalog.yaml](../../plugins.catalog.yaml).

## Layout

```
tools/catalog/<slug>/verify.sh
```

Or any path you set in `optional_tool_path` for that catalog entry.

## Contract

- Exit **0** when checks pass
- Exit **non-zero** on failure; write a short message to stderr
- Keep scripts self-contained (no network unless the entry documents it)

## CI

CI does not run these by default. Contributors may add one example tool per catalog entry.

Contributors run locally before opening a PR when adding or changing a tool.
