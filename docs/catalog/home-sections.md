# Home Screen Sections

**Integration tier:** soft runtime  
**Repo:** [IAmParadox27/jellyfin-plugin-home-sections](https://github.com/IAmParadox27/jellyfin-plugin-home-sections)

## What it does

Replaces the default Jellyfin home screen with **server-driven sections** (dynamic rows, modular layout). Plugins can expose section providers consumed by the Home Screen Sections (HSS) plugin.

## When to use

- Your plugin adds home-screen rows or modular content
- You want Netflix-style sections without editing jellyfin-web directly

## Never trust installed

HSS and its prerequisites (File Transformation, Plugin Pages) may be absent. **Never assume** they are installed or enabled. Detect at runtime and degrade.

## Prerequisites (soft)

Upstream documents File Transformation and Plugin Pages as prerequisites for full HSS functionality. Treat each as soft runtime:

1. Detect HSS via plugin manager GUID or name
2. If missing, hide modular-home features; keep core plugin behavior

## Sample pattern

```csharp
private static readonly Guid HomeSectionsPluginId = new("…"); // from HSS meta.json

public void RegisterSectionIfAvailable()
{
    if (!_guard.TryGetPlugin(HomeSectionsPluginId, out var plugin))
    {
        _logger.Info("Home Screen Sections not installed; section registration skipped.");
        return;
    }

    // Register via HSS extension API or reflection per upstream docs
}
```

## Referenceable relationship

Newer Paradox plugins may expose services via [Jellyfin.Plugin.Referenceable](plugin-referenceable.md). When integrating with HSS, read upstream docs for the current API (reflection vs Referenceable NuGet).

## What if missing

- Do not fail plugin startup
- Omit home UI extensions; document in admin settings that HSS unlocks the feature
- Log once at Info when detection fails

## Related

- [File Transformation](file-transformation.md)
- [plugin-referenceable](plugin-referenceable.md)
