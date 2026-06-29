# Optional plugins and integration tiers

Jellyfin isolates plugins in separate assembly load contexts. **Never assume another plugin is installed.**

## Three integration tiers

| Tier | Catalog value | Pattern |
|------|---------------|---------|
| Soft runtime | `soft_runtime` | Detect via `IPluginManager` / `OptionalPluginGuard`; degrade feature |
| Referenceable | `referenceable` | NuGet + source-gen cross-plugin services |
| JellyfinLoader | `loader` | `loader.json` load-order workaround for hard deps |

See [CONTEXT.md](../../CONTEXT.md) and catalog deep docs under [docs/catalog/](../catalog/).

## Guard pattern (soft runtime)

Use `Infrastructure/OptionalPluginGuard`:

```csharp
if (!_guard.TryGetPlugin(otherPluginGuid, out _))
{
    _logger.Info("Feature X disabled — OtherPlugin not installed.");
    return;
}
```

For hard-required paths only:

```csharp
_guard.RequirePlugin(requiredGuid, "import metadata");
```

## Logging

Use `PluginLogger` tiers — Info when degrading optional features, Error only when user explicitly enabled a broken required path. See [LOGGING.md](../LOGGING.md).

## Catalog examples

| Slug | Tier |
|------|------|
| [file-transformation](../catalog/file-transformation.md) | soft_runtime |
| [home-sections](../catalog/home-sections.md) | soft_runtime |
| [plugin-referenceable](../catalog/plugin-referenceable.md) | referenceable |
| [jellyfin-loader](../catalog/jellyfin-loader.md) | loader |
| [tmdb](../catalog/tmdb.md) | soft_runtime |
| [intro-skipper](../catalog/intro-skipper.md) | soft_runtime |

## Rules

1. No compile-time references to other plugin assemblies (unless Referenceable NuGet documents it)
2. Detect at runtime before calling foreign APIs
3. Do not crash server startup for optional integrations
4. Document optional features in plugin settings / README
