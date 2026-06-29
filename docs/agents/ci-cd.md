# CI/CD

How verify tiers run in GitHub Actions and how to adjust workflows without committing secrets.

## Verify tiers

| Tier | When | Where |
|------|------|-------|
| 0 — static | Every PR and push to `main` | `.github/workflows/ci.yml` → `build-test` job |
| 1 — Docker smoke | Every PR (when implemented) | `integration-smoke` job in same workflow |

### Tier 0 — build, unit tests, and catalog (live)

Workflow: [.github/workflows/ci.yml](../../.github/workflows/ci.yml), job `build-test`.

Steps: `dotnet restore` → `dotnet build -c Release` → `dotnet test -c Release` → `./scripts/validate-catalog.sh` → `./scripts/validate-catalog.sh --self-check` → `./scripts/regenerate-agents-index.sh --check`.

### Tier 1 — Jellyfin integration

Not implemented yet. Adds a separate `integration-smoke` job and local scripts:

- `./scripts/jellyfin-dev.sh` — local test server
- `tests/integration/smoke.sh` — CI smoke (plugin loads in container)

## Local parity

Run the same commands as CI from the repository root:

```bash
dotnet restore
dotnet build -c Release
dotnet test -c Release
./scripts/validate-catalog.sh
./scripts/validate-catalog.sh --self-check
./scripts/regenerate-agents-index.sh --check
```

After adding catalog entries locally, run `./scripts/regenerate-agents-index.sh` (without `--check`) and commit the **Catalog index** region in AGENTS.md.

## Adjusting workflows

Common fork changes:

| Change | Where |
|--------|-------|
| Run CI on other branches | Edit `on.pull_request.branches` / `on.push.branches` in `ci.yml` |
| Skip CI on draft PRs | Add `types: [opened, synchronize, reopened, ready_for_review]` under `pull_request` |
| Add a job | New `jobs.<name>` block; use `needs:` for ordering |
| Run tier 1 only on `main` | Gate `integration-smoke` with `if: github.event_name == 'push'` |

Keep tier 0 fast; do not inline Docker steps into `build-test`.

## Secrets

Never commit secret values. Store them in **GitHub → Settings → Secrets and variables → Actions**.

Reference in workflow YAML only:

```yaml
env:
  EXAMPLE_TOKEN: ${{ secrets.EXAMPLE_TOKEN }}
```

Do not edit `.env` in the repo for CI secrets — `.env` is for local dev overrides.

## Extension points

**Catalog** — live in `build-test` after build/test: catalog validation and index check scripts.

**Integration smoke** — separate `integration-smoke` job; `needs: build-test`; runs `tests/integration/smoke.sh`.

**Release** — separate workflow on tag push; may use `secrets.RELEASE_TOKEN`.

## GitHub vs GitLab

[`.gitlab-ci.yml`](../.gitlab-ci.yml) mirrors the same shell commands as GitHub Actions.

Open devcontainer: VS Code / Cursor → **Reopen in Container** (requires `.devcontainer/devcontainer.json`).

## Release on tag

Workflow [`.github/workflows/release.yml`](../../.github/workflows/release.yml) runs Docker smoke, then uploads a plugin zip to GitHub Releases.

Manual Jellyfin manifest publish (v1): after tagging, run `./scripts/update-manifest.sh Improved-SyncPlay-<version>.zip v<version>`, commit `manifest.json`, and push. The release workflow also uploads `manifest.json` and the zip to GitHub Releases. Users add this repository URL in Jellyfin:

```
https://raw.githubusercontent.com/noxaur/improved-sync-play/master/manifest.json
```
