# JellyfinLoader

**Integration tier:** loader  
**Repo:** [stenlan/JellyfinLoader](https://github.com/stenlan/JellyfinLoader)

## What it does

Manages **plugin load order** and shared assembly load contexts so plugins with hard dependencies on other plugin DLLs can run — a workaround for Jellyfin's default isolation.

## When to use

- You have a **hard assembly dependency** on another plugin's types at runtime
- Soft runtime detection and reflection are insufficient
- You accept operational complexity (loader config, load order, breakage on Jellyfin upgrades)

## When not to use

- Optional features → **soft runtime** + `OptionalPluginGuard`
- Cross-plugin APIs → prefer [Referenceable](plugin-referenceable.md)

## Never trust installed

JellyfinLoader itself may be absent. Plugins should still start without it when the dependency is optional. When loader is required, fail with a clear admin message — not a silent stack trace.

## loader.json

JellyfinLoader uses `loader.json` to declare dependencies and load contexts. See upstream repo for schema. Keep loader config documented in your plugin README for homelab operators.

## Load contexts

Loader merges or bridges contexts so dependent assemblies resolve. This is fragile across Jellyfin major versions — pin tested combinations in your release notes.

## What if missing

- **Optional hard dep:** disable dependent feature; log Error once with install link
- **Required hard dep:** throw `RequiredPluginMissingException` (see `Infrastructure/OptionalPluginGuard.cs`) only when the user explicitly enabled a feature that cannot work

## Related

- [plugin-referenceable](plugin-referenceable.md) — preferred for new integrations
- [optional-plugins.md](../agents/optional-plugins.md)
