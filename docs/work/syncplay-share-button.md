---
status: done
phase: finish
last_phase: scaffold
depends: []
tier: feature
issue:
---

# SyncPlay share button

## Intent

Add a share button to Jellyfin's SyncPlay UI so users can copy a session invite link.
Bootstrap from jellyfin-plugin-template as `improved-sync-play` to exercise the template and surface gaps.

## Acceptance

- [x] Plugin renamed from Template placeholders to ImprovedSyncPlay
- [x] Share button appears in SyncPlay UI when File Transformation is available
- [x] Clicking share copies invite URL to clipboard (or shows copyable link fallback)
- [x] Graceful degradation when File Transformation is not installed
- [x] Tier 0 verify passes (build, test, catalog scripts)
- [x] Template friction documented in `docs/template-feedback.md`

## Verify

```bash
dotnet restore
dotnet build -c Release
dotnet test -c Release
./scripts/validate-catalog.sh
./scripts/validate-catalog.sh --self-check
./scripts/regenerate-agents-index.sh --check
```

## Interface sketch

### Types

- `ImprovedSyncPlayPlugin` — main plugin, registers web transformation
- `SyncPlayShareTransformation` — JS injection for share button in SyncPlay panel

### Seams

- File Transformation plugin (optional, soft runtime) for jellyfin-web asset mutation
- SyncPlay session API endpoints for invite link generation

### Functions

- `RegisterTransformation` — inject share button JS/CSS into SyncPlay UI
- `getShareUrl(sessionId)` — client-side helper to build copyable invite URL

### Files

- `Jellyfin.Plugin.ImprovedSyncPlay/Plugin.cs` — plugin entry, transformation registration
- `Jellyfin.Plugin.ImprovedSyncPlay/Web/syncplay-share.js` — injected client script
- `Jellyfin.Plugin.ImprovedSyncPlay/Web/syncplay-share.css` — button styles
- `docs/template-feedback.md` — template issues found during bootstrap/dev

### Deferred to 3d

- Deep-link URL scheme if Jellyfin changes SyncPlay routing
- Automated tier-1 Docker smoke for UI injection

### Notes

Entry: Plugin startup → detect File Transformation → register transformation on `/web` SyncPlay assets.
Flow: User opens SyncPlay → share button visible → click → clipboard copy of session invite URL.

## Decisions

- Use File Transformation pattern from template catalog (soft runtime, optional)
- Rename all Template placeholders per AGENTS.md checklist
- Minimal JS injection targeting SyncPlay session controls DOM

## Open questions

## Files touched

- `Jellyfin.Plugin.ImprovedSyncPlay/` — full plugin project (Plugin.cs, Configuration, Infrastructure, Web)
- `Jellyfin.Plugin.ImprovedSyncPlay.Tests/` — adapted template tests + IndexHtmlTransformTests
- `Jellyfin.Plugin.ImprovedSyncPlay.sln` — solution (replaces Template.sln)
- `build.yaml`, `meta.json` — Improved SyncPlay identity
- `scripts/stage-plugin.sh`, `scripts/jellyfin-dev.sh`, `tests/integration/smoke.sh`, `.github/workflows/release.yml`
- `README.md`, `docs/template-feedback.md`
- Removed `Jellyfin.Plugin.Template/` and `Jellyfin.Plugin.Template.Tests/`

## Verify status

| Check | Result |
|-------|--------|
| `dotnet build -c Release` | Pass (0 warnings) |
| `dotnet test -c Release` | Pass (12/12) |
| `./scripts/validate-catalog.sh` | Pass |
| `./scripts/validate-catalog.sh --self-check` | Pass |
| `./scripts/regenerate-agents-index.sh --check` | Pass |

Note: local test run required .NET 9 ASP.NET Core runtime (CI installs via `setup-dotnet@v4`).

## Drift log

- CSS skipped; share button uses Jellyfin material-icons + existing menu styles (inline in JS path).
