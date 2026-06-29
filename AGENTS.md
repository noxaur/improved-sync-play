# AGENTS.md — router

Lean entrypoint for coding agents building **your Jellyfin plugin**. Load deep docs on demand; do not treat this file as a tutorial.

## Read first

| Doc | Purpose |
|-----|---------|
| [CONTEXT.md](CONTEXT.md) | Glossary and vocabulary |
| [docs/agents/ci-cd.md](docs/agents/ci-cd.md) | Verify tiers and CI |
| [docs/LOGGING.md](docs/LOGGING.md) | Logging and optional-plugin patterns |

## Verify

Run from **repository root** (your plugin repo after bootstrap) before committing.

### Tier 0 — static (CI + local)

GitHub Actions: [.github/workflows/ci.yml](.github/workflows/ci.yml) (`build-test` job).

```bash
dotnet restore
dotnet build -c Release
dotnet test -c Release
./scripts/validate-catalog.sh
./scripts/validate-catalog.sh --self-check
./scripts/regenerate-agents-index.sh --check
```

### Tier 1 — Docker Jellyfin

```bash
./scripts/jellyfin-dev.sh up          # local test server
./scripts/jellyfin-dev.sh deploy      # re-stage after code changes
cd tests/integration && ./smoke.sh    # CI-equivalent smoke
```

Requires Docker Compose v2. See [docs/COMPATIBILITY.md](docs/COMPATIBILITY.md) for server/SDK/image alignment.

## Fork rename checklist

Complete before shipping. Replace every `Template` placeholder.

1. **Project identity** — rename folder, `.csproj`, `.sln`, and test project: `Jellyfin.Plugin.Template` → `Jellyfin.Plugin.<YourName>`
2. **Namespaces** — update `namespace` / `using` in all `.cs` files
3. **[Plugin.cs](Jellyfin.Plugin.Template/Plugin.cs)** — set `Name`; generate a new `Id` GUID (do not keep `eb5d7894-8eef-4b36-aa6f-5d124e828ce1`)
4. **[meta.json](Jellyfin.Plugin.Template/meta.json)** — `name`, `guid`, `version`, `description`, `overview`, `owner`, `targetAbi`
5. **[build.yaml](build.yaml)** — `name`, `guid`, `version`, `artifacts` (DLL filename), descriptions, `owner`, `category`
6. **[versions.props](versions.props)** — `JellyfinSdkVersion` and `JellyfinTargetAbi` aligned with your target Jellyfin server

## Do not edit

| Path | Reason |
|------|--------|
| `.env` | Local overrides (created by dev scripts when tier 1 lands) |
| Secrets / credentials files | Security |
| `bin/`, `obj/`, `artifacts/` | Build outputs |
| Catalog index marker region below | CI-regenerated — run `./scripts/regenerate-agents-index.sh` |

## Catalog index

Ecosystem plugins — deep docs load on demand.

<!-- catalog-index:start -->
| Slug | Category | Tier | Doc |
|------|----------|------|-----|
| `catalog-stub` | pattern | pattern | [docs/catalog/catalog-stub.md](docs/catalog/catalog-stub.md) |
| `file-transformation` | ui-extension | soft_runtime | [docs/catalog/file-transformation.md](docs/catalog/file-transformation.md) |
| `home-sections` | ui-extension | soft_runtime | [docs/catalog/home-sections.md](docs/catalog/home-sections.md) |
| `intro-skipper` | community | soft_runtime | [docs/catalog/intro-skipper.md](docs/catalog/intro-skipper.md) |
| `jellyfin-loader` | cross-plugin | loader | [docs/catalog/jellyfin-loader.md](docs/catalog/jellyfin-loader.md) |
| `plugin-referenceable` | cross-plugin | referenceable | [docs/catalog/plugin-referenceable.md](docs/catalog/plugin-referenceable.md) |
| `tmdb` | metadata | soft_runtime | [docs/catalog/tmdb.md](docs/catalog/tmdb.md) |
<!-- catalog-index:end -->

## Deep docs

| Doc | When to read |
|-----|--------------|
| [docs/LOGGING.md](docs/LOGGING.md) | Logging and optional-plugin patterns |
| [docs/agents/ci-cd.md](docs/agents/ci-cd.md) | Verify tiers and CI |
| [docs/agents/upgrading.md](docs/agents/upgrading.md) | Absorb template improvements from upstream |
| [docs/COMPATIBILITY.md](docs/COMPATIBILITY.md) | Server / SDK / Docker image matrix |
| [docs/archetypes/](docs/archetypes/) | Optional copy-paste patterns (config page, webhook, metadata) |
| [docs/agents/optional-plugins.md](docs/agents/optional-plugins.md) | Cross-plugin integration |
| [CONTRIBUTING.md](CONTRIBUTING.md) | Catalog contribution conventions |

## Contributing catalog entries

See [CONTRIBUTING.md](CONTRIBUTING.md).
