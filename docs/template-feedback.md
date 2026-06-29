# Template bootstrap feedback

Notes from bootstrapping **improved-sync-play** from [jellyfin-plugin-template](https://github.com/jellyfin/jellyfin-plugin-template) (`template/` subtree). Intended for upstream template maintainers.

## Rename / scaffold friction

| Area | Issue | Impact |
|------|-------|--------|
| Fork checklist | AGENTS.md lists rename steps but no scripted rename or single “project name” variable | Manual find/replace across ~15 paths (`.sln`, `build.yaml`, `meta.json`, stage scripts, CI, smoke tests) |
| Partial bootstrap | Work started in `Jellyfin.Plugin.ImprovedSyncPlay/` while `Jellyfin.Plugin.Template*` remained | `dotnet build` still compiled Template; easy to ship wrong assembly name |
| File Transformation | Catalog doc describes reflection registration but template ships no `FileTransformationRegistrar`, `IndexHtmlTransform`, or embedded Web resource pattern | Authors must copy from ecosystem plugins or invent callback shape |
| Embedded Web assets | Template `.csproj` has no `EmbeddedResource` example for `/web` injection | Unclear how to pack client JS for File Transformation callbacks |
| Newtonsoft.Json | File Transformation payload uses `JObject`; template `.csproj` does not reference Newtonsoft.Json explicitly | Works at Jellyfin runtime via host; tests/plugin code need explicit package ref for compile |
| Plugin startup | Template `Plugin.cs` only demonstrates optional-plugin guard, not calling a registrar on startup | Easy to forget `TryRegister` wiring after adding infrastructure files |
| CSS | No guidance on inline vs separate CSS for web injection | Minor; Jellyfin material-icons often suffice |

## What worked well

- `OptionalPluginGuard` + `PluginLogger` patterns are copy-paste friendly
- Catalog entry for File Transformation documents soft-runtime detection
- Tier 0 verify scripts (`validate-catalog.sh`, `regenerate-agents-index.sh`) run unchanged after rename
- `stage-plugin.sh` + Docker smoke path is clear once meta.json path is updated

## Suggested template improvements

1. Add optional archetype under `docs/archetypes/` or catalog tool: **file-transformation-web-inject** with `IndexHtmlTransform.cs`, `FileTransformationRegistrar.cs`, and sample embedded JS
2. Extend fork rename checklist with **scripts** and **CI workflow** paths (not only `.csproj` / `meta.json`)
3. Consider a `scripts/rename-plugin.sh YourPluginName` that updates sln, build.yaml, stage paths, and smoke references
4. Document that bootstrap may leave dual project folders until rename completes — add validate script that fails if `Jellyfin.Plugin.Template` still exists

## Blockers encountered

None for tier-0 static verify. Tier-1 Docker smoke not re-run in this session (requires Docker + File Transformation plugin for full UI validation).
