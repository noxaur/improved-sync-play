# Contributing — catalog entries

Guidelines for adding ecosystem catalog entries to **your plugin repo** (or upstream to jellyfin-agent-template).

## Catalog contribution model

Schema: [schemas/plugins.catalog.schema.json](schemas/plugins.catalog.schema.json) (JSON Schema Draft 2020-12).

### One catalog entry = three parts

1. **One row** in [plugins.catalog.yaml](plugins.catalog.yaml) under `entries`
2. **One markdown doc** at `docs/catalog/<slug>.md` — self-contained integration guide
3. **Optional tool** at `tools/catalog/<slug>/` — see [tools/catalog/README.md](tools/catalog/README.md)

### PR checklist (catalog entries)

1. Add or update the row in `plugins.catalog.yaml`
2. Add or update `docs/catalog/<slug>.md`
3. Optional: add `tools/catalog/<slug>/verify.sh` and set `optional_tool_path`
4. Run `./scripts/validate-catalog.sh`
5. Run `./scripts/regenerate-agents-index.sh` and commit the updated **Catalog index** region in [AGENTS.md](AGENTS.md)
6. Run full tier 0 from [AGENTS.md](AGENTS.md) before opening the PR

### Field reference

| Field | Required | Notes |
|-------|----------|-------|
| `name` | yes | Display name |
| `slug` | yes | Lowercase kebab-case; matches doc filename |
| `category` | yes | e.g. `ui-extension`, `cross-plugin`, `metadata`, `community`, `pattern` |
| `doc_path` | yes | Must exist in repo |
| `runtime_required` | yes | Boolean |
| `jellyfin_min_version` | yes | e.g. `"10.9.0"` |
| `entry_type` | yes | `plugin` or `pattern` |
| `integration_tier` | yes | `soft_runtime`, `referenceable`, `loader`, or `pattern` |
| `repo_url` | if `entry_type: plugin` | `https://…` or `git@…` |
| `optional_tool_path` | no | Path must exist when set |

### Catalog doc guidelines

- Assume the reader has not installed the target plugin
- Show runtime detection, not compile-time references to other plugin assemblies
- Link to upstream repo when `entry_type: plugin`
- See [CONTEXT.md](CONTEXT.md) for integration tier vocabulary

To contribute catalog entries upstream, open a PR against the [jellyfin-agent-template](https://github.com/jellyfin/jellyfin-plugin-template) maintainer repo (`template/` paths).
