# TMDb metadata

**Integration tier:** soft runtime  
**Repo:** [jellyfin/jellyfin](https://github.com/jellyfin/jellyfin) (server-bundled metadata providers)

## What it does

Jellyfin ships **TMDb** (The Movie Database) metadata providers for movies, shows, and people. Your plugin may rely on TMDb IDs, images, or metadata already on library items — not on a separate plugin install.

## When to use

- Your plugin reads `ProviderIds` containing `Tmdb` / `TmdbCollection`
- You enrich items that were identified via official metadata workflows
- You call Jellyfin's metadata APIs that assume standard providers ran at library scan time

## Never trust installed

TMDb can be **disabled** in library settings or fail to return IDs for manual/legacy items. Do not assume every item has a TMDb ID.

## Soft runtime guard pattern

```csharp
public bool TryGetTmdbId(BaseItem item, out string? tmdbId)
{
    tmdbId = null;
    if (item.ProviderIds.TryGetValue(MetadataProvider.Tmdb.ToString(), out var id)
        && !string.IsNullOrWhiteSpace(id))
    {
        tmdbId = id;
        return true;
    }

    _logger.Debug($"No TMDb id for {item.Name}; skipping TMDb-specific feature.");
    return false;
}
```

## What if missing

- Skip TMDb-specific API calls (trailers, collections, external links)
- Offer manual ID entry in plugin settings when appropriate
- Never block playback or core plugin features

## Related

- [LOGGING.md](../LOGGING.md)
- Official docs: Jellyfin metadata manager and provider configuration
