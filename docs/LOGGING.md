# Jellyfin plugin logging standard

Use this standard in every fork so logs stay consistent and optional-plugin failures are diagnosable.

## Core principle

**Never assume another plugin is installed.** Detect at runtime, log clearly, then degrade or abort.

## Canonical helpers

| Helper | Location | Use for |
|--------|----------|---------|
| `PluginLogger` | `Infrastructure/PluginLogger.cs` | All plugin logging |
| `OptionalPluginGuard` | `Infrastructure/OptionalPluginGuard.cs` | Co-installed plugin presence checks |

Construct a `PluginLogger` from `ILogger` and your plugin display name. Pass it to `OptionalPluginGuard` alongside `IPluginManager`.

## Log prefix

Every message is prefixed with `[PluginName]` — for example `[Template] Library scan started`.

Do not invent alternate prefixes or raw `Console.WriteLine` for operational logs.

## Log tiers

| Tier | MS log level | When to use |
|------|--------------|-------------|
| `Verbose` | `Trace` | High-volume diagnostics; off in normal production |
| `Debug` | `Debug` | Developer troubleshooting; safe for homelab |
| `Info` | `Information` | Normal lifecycle events (startup, config saved, job completed) |
| `Warning` | `Warning` | Degraded behavior, retries, **optional plugin missing** |
| `Error` | `Error` | Failures, exceptions, **hard-required plugin missing** |

## Optional vs required plugins

### Soft dependency (optional)

When a co-installed plugin is missing and your feature can degrade:

1. Call `OptionalPluginGuard.IsInstalled` or `TryGetPlugin`.
2. If absent, call `PluginLogger.LogOptionalPluginMissing(pluginId, action, userMessage)`.
3. Skip or degrade the feature — **do not throw**.

### Hard dependency (required)

When your plugin cannot function without another plugin:

1. Call `OptionalPluginGuard.RequirePlugin(pluginId, action)`.
2. The guard logs `Error` and throws `RequiredPluginMissingException`.

Reserve hard requirements for rare cases. Prefer soft dependencies and graceful degradation.

## Example

```csharp
if (!_guard.TryGetPlugin(otherPluginId, out var otherPlugin))
{
    _logger.LogOptionalPluginMissing(otherPluginId, "enrich metadata", "Metadata enrichment disabled.");
    return;
}

// Use otherPlugin when present
```

## Related docs

- [UPSTREAM.md](UPSTREAM.md) — Jellyfin plugin-manager API used by `OptionalPluginGuard`
- `docs/agents/optional-plugins.md` — integration tiers (spec 11)
