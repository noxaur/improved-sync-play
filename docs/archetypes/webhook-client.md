# Webhook client archetype

**Complexity:** Medium — adds HTTP client + optional incoming webhook endpoint.

## When to use

- Your plugin calls external services (Sonarr, Radarr, custom APIs)
- You receive HTTP callbacks from third parties

## When to stay minimal

- No network I/O → skip
- Polling-only with rare calls → a single `HttpClient` field may suffice without full archetype

## Steps to add

1. Register `IHttpClientFactory` or typed `HttpClient` in DI (per Jellyfin host patterns for your SDK)
2. Store API keys in `PluginConfiguration` — **never** hardcode secrets
3. For incoming webhooks, add an API controller with auth (token header or Jellyfin user policy)
4. Validate payloads; return 4xx on bad input

## Illustrative snippet

```csharp
public async Task NotifyExternalAsync(CancellationToken ct)
{
    var client = _httpClientFactory.CreateClient(nameof(MyPlugin));
    using var response = await client.PostAsJsonAsync(_config.WebhookUrl, payload, ct)
        .ConfigureAwait(false);
    response.EnsureSuccessStatusCode();
}
```

## Secrets

- Use plugin configuration or environment overrides (`.env` local only — gitignored)
- Do not commit tokens; document required GitHub Actions secrets in `docs/agents/ci-cd.md`

## Verify after adding

```bash
dotnet build -c Release
dotnet test -c Release
# Manual: trigger webhook against local Jellyfin with jellyfin-dev.sh
```

## Related

Webhook patterns are **archetype-only** — not a catalog plugin row.
