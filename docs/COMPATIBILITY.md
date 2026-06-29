# Compatibility matrix

| Jellyfin server | SDK (`versions.props`) | Docker image tag (`jellyfin/jellyfin`) |
|-----------------|------------------------|----------------------------------------|
| 10.11.x         | 10.11.11               | 10.11.11                               |

`JellyfinTargetAbi` in `versions.props` must match your server ABI (`10.11.0.0` for 10.11 servers). Template targets **net9.0**, which requires Jellyfin 10.11+ (server runtime .NET 9).

## Bump process

1. Update `JellyfinSdkVersion` and `JellyfinTargetAbi` in `versions.props`
2. Set `JELLYFIN_IMAGE_TAG` in `tests/integration/.env.example` to match `JellyfinSdkVersion` (smoke/dev scripts derive it automatically)
3. Run tier 0 + tier 1 verify (`./scripts/jellyfin-dev.sh up` or `tests/integration/smoke.sh`)
4. Merge Renovate PR only when CI (including Docker smoke) is green

Renovate tracks SDK packages and Docker image tags — see root `renovate.json` in the maintainer monorepo.
