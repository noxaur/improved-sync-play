# Jellyfin.Plugin.ImprovedSyncPlay

Jellyfin server plugin that adds a SyncPlay session share button to the web UI (via optional File Transformation integration).

## Build

```bash
dotnet build -c Release
dotnet test -c Release
```

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

Agents: start at [AGENTS.md](AGENTS.md). Devcontainer: `.devcontainer/devcontainer.json`.

## License

GPL-3.0 — see [LICENSE](LICENSE).
