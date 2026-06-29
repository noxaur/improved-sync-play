# Config page archetype

**Complexity:** High vs minimal scaffold — adds embedded HTML settings UI and API controller.

## When to use

- Administrators need in-browser configuration
- Settings are too rich for environment variables alone

## When to stay minimal

- Few settings → use `PluginConfiguration` defaults or a single JSON file
- No UI needed → skip this archetype

## Steps to add

1. Add `Configuration/PluginConfiguration.cs` properties for each setting
2. Add `Api/PluginConfigController.cs` inheriting Jellyfin API controller patterns
3. Add `Configuration/configPage.html` embedded resource (see upstream jellyfin-plugin-template for reference — **verify routes against pinned SDK**)
4. Register controller in plugin startup if required by your Jellyfin version

## Illustrative snippet

```csharp
[Authorize(Policy = Policies.RequiresElevation)]
[ApiController]
[Route("Plugin/Template")]
public class PluginConfigController : ControllerBase
{
    // GET/POST configuration — match Jellyfin.Controller patterns for your SDK pin
}
```

## Verify after adding

```bash
dotnet build -c Release
./scripts/jellyfin-dev.sh deploy
# Dashboard → Plugins → your plugin → settings page loads
```

## Related

- [LOGGING.md](../LOGGING.md)
- Official [jellyfin-plugin-template](https://github.com/jellyfin/jellyfin-plugin-template) config samples (trimmed from this scaffold intentionally)
