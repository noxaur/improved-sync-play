# Metadata provider archetype

**Complexity:** High — implements Jellyfin metadata pipeline interfaces.

## When to use

- You fetch metadata from a custom API
- You augment or replace identifiers for library items

## When to stay minimal

- You only read existing library metadata → use [tmdb](../catalog/tmdb.md) soft-runtime patterns instead

## Steps to add

1. Implement `IMetadataProvider<T>` / related interfaces for your item types (**verify exact interfaces for pinned SDK**)
2. Register provider in plugin service configuration
3. Map remote IDs in `ProviderIds` consistently
4. Handle rate limits and missing results gracefully

## Illustrative snippet

```csharp
public class MyMetadataProvider : IMetadataProvider<Movie>, IHasMetadataFeatures
{
    public string Name => "My Provider";
    // Implement GetMetadataAsync — signatures vary by Jellyfin version
}
```

## Never trust installed

Other metadata plugins may or may not run before yours. Do not assume TMDb IDs exist — see [tmdb](../catalog/tmdb.md).

## Verify after adding

```bash
dotnet build -c Release
./scripts/jellyfin-dev.sh up
# Library → scan / identify — confirm metadata appears in logs
```

## Related

- [tmdb](../catalog/tmdb.md)
- Jellyfin docs for metadata providers (check version matching `versions.props`)
