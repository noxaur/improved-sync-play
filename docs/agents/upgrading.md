# Upgrading from jellyfin-agent-template

How to absorb improvements from the [jellyfin-agent-template](https://github.com/jellyfin/jellyfin-plugin-template) maintainer repo into **your** plugin repository (created via bootstrap/copy).

## Merge-safe paths (usually take upstream)

These paths are infrastructure — safe to merge or cherry-pick when you want template fixes:

| Path | Why merge-safe |
|------|----------------|
| `Infrastructure/` | Logging and optional-plugin guards |
| `scripts/` | Catalog validation, staging, Jellyfin dev/smoke |
| `tests/integration/` | Docker smoke stack |
| `.github/workflows/` | CI and release workflows |
| `docs/agents/ci-cd.md`, `docs/COMPATIBILITY.md`, `docs/LOGGING.md` | Operational docs |
| `docs/catalog/` | Ecosystem entries (review for relevance) |
| `schemas/`, `plugins.catalog.yaml` | Catalog machinery |
| `versions.props` defaults | SDK alignment — **verify** against your Jellyfin server before merging |

## Fork-owned paths (keep yours)

| Path | Why fork-owned |
|------|----------------|
| Plugin project folder / `.csproj` / `.sln` names | Your plugin identity |
| `Plugin.cs` `Name`, `Id` GUID | Your plugin identity |
| `meta.json`, `build.yaml` | Your manifest and release metadata |
| Business logic under your plugin project | Your feature code |
| `README.md` | Your product docs |

## Suggested workflow

1. Add maintainer repo as a remote: `git remote add template-upstream <url>`
2. Fetch: `git fetch template-upstream`
3. Cherry-pick or merge only merge-safe paths:

   ```bash
   git checkout template-upstream/main -- scripts/ Infrastructure/
   # resolve conflicts; never overwrite your Plugin.cs GUID/name blindly
   ```

4. Run tier 0 + tier 1 verify from `AGENTS.md`
5. Commit with a note of which upstream commit you absorbed

## Getting template updates

The maintainer ships changes in `template/`. Compare:

```bash
git diff template-upstream/main -- template/
```

Apply relevant files into your repo root (your bootstrap copy maps `template/` → `.`).

## When in doubt

- **Infrastructure / CI / docs** → prefer upstream
- **Identity / features** → keep yours

See [optional-plugins.md](optional-plugins.md) and [LOGGING.md](../LOGGING.md).
