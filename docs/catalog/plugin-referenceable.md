# Jellyfin.Plugin.Referenceable

**Integration tier:** referenceable  
**Repo:** [IAmParadox27/jellyfin-plugin-referenceable](https://github.com/IAmParadox27/jellyfin-plugin-referenceable)

## What it does

Adds **source-generated cross-plugin references** so plugins can call each other's services across Jellyfin's isolated assembly load contexts — without fragile reflection.

## When to use

- You **expose** services other plugins should call (extensible plugin)
- You **consume** another plugin's Referenceable NuGet package (e.g. File Transformation 1.2+)
- Hard reflection against foreign assemblies is brittle for your use case

## When not to use

- Simple optional feature flags → use **soft runtime** + `OptionalPluginGuard`
- Hard assembly load-order deps → see [JellyfinLoader](jellyfin-loader.md)

## Never trust installed

Even with Referenceable, the **consumer** plugin may be missing. NuGet references compile; runtime still requires the target plugin installed and version-aligned.

## Install (consumer)

```xml
<PackageReference Include="Jellyfin.Plugin.FileTransformation" Version="1.2.2" />
```

Match versions documented by the exposing plugin. Mismatched Referenceable versions cause subtle runtime failures.

## Expose services (provider)

Follow upstream README: mark outputs with Referenceable attributes, publish a NuGet package, document `OutputItemType` and minimum Referenceable host version.

## Version alignment warning

Referenceable plugins pin a **host Referenceable version**. When Jellyfin SDK or Referenceable bumps, verify all participating plugins in release notes. Renovate SDK PRs should run tier 1 smoke when available.

## What if missing

- Consumer: catch missing service registration; degrade feature
- Provider: document required companion plugins in `meta.json` overview only — do not crash server at startup for optional integrations

## Related

- [File Transformation](file-transformation.md) — Referenceable NuGet path
- [LOGGING.md](../LOGGING.md) — tiered logs when degrading
