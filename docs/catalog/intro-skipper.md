# Intro Skipper

**Integration tier:** soft runtime  
**Repo:** [intro-skipper/intro-skipper](https://github.com/intro-skipper/intro-skipper)

## What it does

Community plugin that detects and skips TV intros/credits using chapter or detection data. Popular example of a **co-installed community plugin** your feature might integrate with.

## When to use

- Your plugin coordinates with intro/credit skip markers
- You expose skip hints or read Intro Skipper state for automation workflows
- You document a community integration pattern (detect-or-degrade)

## Never trust installed

Intro Skipper is **not** part of Jellyfin core. Users may uninstall it, run an old version, or disable it per library. Always detect at runtime.

## Detect-or-degrade

```csharp
private static readonly Guid IntroSkipperPluginId = new("…"); // from intro-skipper meta.json

public bool CanUseIntroSkip()
{
    if (_guard.IsInstalled(IntroSkipperPluginId))
    {
        return true;
    }

    _logger.Info("Intro Skipper not installed; skip integration disabled.");
    return false;
}
```

Resolve the real GUID from [intro-skipper meta.json](https://github.com/intro-skipper/intro-skipper) when implementing — do not ship the placeholder GUID above.

## What if missing

- Hide skip-related UI and API endpoints
- Do not write Intro Skipper config files directly unless upstream documents a stable contract
- Log at Info on first use, not on every request

## Related

- [LOGGING.md](../LOGGING.md)
- [optional-plugins.md](../agents/optional-plugins.md)
