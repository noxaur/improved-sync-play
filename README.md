# Jellyfin.Plugin.ImprovedSyncPlay

Jellyfin server plugin that adds a SyncPlay session share button to the web UI (via optional File Transformation integration).

## Install from Jellyfin

1. **Dashboard → Plugins → Repositories → +**
2. Add repository URL:

   ```
   https://raw.githubusercontent.com/noxaur/improved-sync-play/master/manifest.json
   ```

3. **Dashboard → Plugins → Catalog** — find **Improved SyncPlay** and install.

Requires Jellyfin **10.11+** and the optional [File Transformation](https://github.com/IAmParadox27/jellyfin-plugin-file-transformation) plugin for the share button UI.

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
| [docs/template-feedback.md](docs/template-feedback.md) | Template bootstrap friction notes |

Agents: start at [AGENTS.md](AGENTS.md). Devcontainer: `.devcontainer/devcontainer.json`.

## License

GPL-3.0 — see [LICENSE](LICENSE).
