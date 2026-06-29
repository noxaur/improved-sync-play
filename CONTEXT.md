# CONTEXT — jellyfin plugin scaffold

Glossary and vocabulary for issues, PRs, and agent docs. Use these terms consistently.

## Product terms

| Term | Definition |
|------|------------|
| **Template** | This scaffold before rename; the starting point, not a shipped plugin product. |
| **Fork** | Your plugin repository created from this template. |
| **Scaffold** | Minimal C# plugin project (`BasePlugin`, configuration, manifest files). |
| **Catalog** | Ecosystem plugin registry: `plugins.catalog.yaml` + `docs/catalog/*.md`. |
| **Archetype** | Optional copy-paste pattern doc (config page, webhook, metadata); not in default build. |
| **Upstream** | Official [jellyfin-plugin-template](https://github.com/jellyfin/jellyfin-plugin-template) and [jellyfin-agent-template](https://github.com/jellyfin/jellyfin-plugin-template) maintainer repo. |

## Jellyfin terms

| Term | Definition |
|------|------------|
| **Server plugin** | .NET assembly loaded by Jellyfin at startup; extends server behavior. |
| **Plugin manifest** | `meta.json` beside the plugin DLL; identity and version. |
| **Build manifest** | `build.yaml`; CI/release metadata for Jellyfin plugin repositories. |
| **NotSupported** | Plugin state when SDK version does not match installed Jellyfin server. |
| **Assembly load context** | Jellyfin isolates each plugin; cross-DLL references between plugins fail by default. |

## Integration tiers

| Tier | Meaning | When to use |
|------|---------|-------------|
| **Soft runtime** | Co-installed plugin optional; detect via plugin manager; degrade feature. | Most ecosystem integrations. |
| **Referenceable** | [Jellyfin.Plugin.Referenceable](https://github.com/IAmParadox27/jellyfin-plugin-referenceable) NuGet; plugins expose referenceable services. | Building extensible plugins others extend. |
| **JellyfinLoader** | [JellyfinLoader](https://github.com/stenlan/JellyfinLoader) manages load order and shared contexts. | Hard assembly dependencies between plugins. |

## Agent terms

| Term | Definition |
|------|------------|
| **Router** | Lean `AGENTS.md`; points to deep docs, not a tutorial. |
| **Verify tier 0** | Static: `dotnet build`, `dotnet test`, catalog scripts. |
| **Verify tier 1** | Docker Jellyfin smoke: plugin loads in container (`smoke.sh`). |
| **Test server** | Local Jellyfin in Docker for plugin development (`jellyfin-dev.sh`). |
| **Plugin staging** | Build output copied to `artifacts/plugin/<Name>/` before mount (`stage-plugin.sh`). |

## Avoid

| Avoid | Prefer |
|-------|--------|
| “dependency plugin” (ambiguous) | soft runtime / Referenceable / JellyfinLoader tier |
| “template plugin” (confusing) | scaffold or fork |
| Assuming plugin X is installed | optional-plugin guard + tiered log |

## Related docs

- [AGENTS.md](AGENTS.md)
- [docs/LOGGING.md](docs/LOGGING.md)
- [docs/agents/upgrading.md](docs/agents/upgrading.md)
