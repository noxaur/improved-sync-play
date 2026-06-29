# Jellyfin.Plugin.Template

Minimal Jellyfin server plugin scaffold. Rename placeholders before shipping (see [AGENTS.md](AGENTS.md#fork-rename-checklist)).

## Build

```bash
dotnet build -c Release
dotnet test -c Release
```

## Rename before shipping

1. Project folder and `.csproj` / solution: `Jellyfin.Plugin.Template` → `Jellyfin.Plugin.YourName`
2. Namespaces in all `.cs` files
3. `Plugin.cs`: `Name` and new `Id` GUID
4. `meta.json`, root `build.yaml`, `versions.props` (SDK / ABI aligned to your Jellyfin server)

Full checklist: [AGENTS.md](AGENTS.md#fork-rename-checklist).

## Local test server

Prerequisites: Docker, Compose v2, .NET 9 SDK.

```bash
./scripts/jellyfin-dev.sh up
# http://localhost:8096 — complete first-run wizard; check Dashboard → Plugins
./scripts/jellyfin-dev.sh deploy   # after code changes
```

CI equivalent: `cd tests/integration && ./smoke.sh`. Server/SDK matrix: [docs/COMPATIBILITY.md](docs/COMPATIBILITY.md).

## Docs

| Doc | Purpose |
|-----|---------|
| [AGENTS.md](AGENTS.md) | Agent router — verify, rename checklist |
| [CONTEXT.md](CONTEXT.md) | Glossary |
| [docs/LOGGING.md](docs/LOGGING.md) | Logging and optional-plugin patterns |
| [docs/catalog/](docs/catalog/) | Ecosystem integration guides |
| [docs/archetypes/](docs/archetypes/) | Optional patterns (config page, webhook, metadata) |

Agents: start at [AGENTS.md](AGENTS.md). Devcontainer: `.devcontainer/devcontainer.json`.

## License

GPL-3.0 — see [LICENSE](LICENSE).
