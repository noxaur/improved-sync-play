# File Transformation

**Integration tier:** soft runtime  
**Repo:** [IAmParadox27/jellyfin-plugin-file-transformation](https://github.com/IAmParadox27/jellyfin-plugin-file-transformation)

## What it does

Lets server plugins mutate **jellyfin-web** assets in memory (JS/CSS/HTML served under `/web`) without patching files on disk. Multiple plugins can register transformations.

## When to use

- Your plugin injects UI or client behavior without forking jellyfin-web
- You need non-destructive web client customization (home sections, custom pages, etc.)

## Never trust installed

File Transformation is **optional** for most features. Detect at runtime; degrade when absent. Do not add a compile-time project reference to its assembly — Jellyfin isolates plugin load contexts.

## Soft runtime detection

Use `OptionalPluginGuard` with the plugin's published GUID, or scan loaded assemblies when integrating via reflection (upstream pattern):

```csharp
// ponytail: reflection path — upgrade when Referenceable NuGet is adopted
var asm = AppDomain.CurrentDomain.GetAssemblies()
    .FirstOrDefault(a => a.FullName?.Contains(".FileTransformation") == true);
if (asm is null)
{
    _logger.Info("File Transformation not installed; skipping web injection.");
    return;
}
```

Prefer `OptionalPluginGuard.TryGetPlugin(pluginGuid, out _)` when the target plugin exposes a stable GUID in its manifest.

## Minimal integration snippet

After confirming the plugin is present, register a transformation via reflection against `Jellyfin.Plugin.FileTransformation.PluginInterface.RegisterTransformation` (see upstream README for payload shape).

## What if missing

- Skip web injection; keep server APIs functional
- Log at Info (not Error) unless the feature is user-facing and required
- Do not throw on optional UI paths

## Related

- [Home Screen Sections](home-sections.md) — often depends on File Transformation
- [plugin-referenceable](plugin-referenceable.md) — newer NuGet-based integration path for some Paradox plugins
