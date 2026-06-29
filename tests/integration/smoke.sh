#!/usr/bin/env bash
# CI smoke: stage plugin, start Jellyfin, assert plugin in /Plugins.
set -euo pipefail

ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
INTEGRATION="$(cd "$(dirname "$0")" && pwd)"
cd "$INTEGRATION"

CI_DATA="$INTEGRATION/.ci-data"
CONFIG="$CI_DATA/config"
CACHE="$CI_DATA/cache"
export JELLYFIN_CONFIG="$CONFIG"
export JELLYFIN_CACHE="$CACHE"
export JELLYFIN_PORT="${JELLYFIN_PORT:-8096}"
export JELLYFIN_IMAGE_TAG="$(
  grep -oP '(?<=<JellyfinSdkVersion>)[^<]+' "$ROOT/versions.props" | tr -d '[:space:]'
)"

PLUGIN_NAME="$(
  python3 -c "import json; print(json.load(open('$ROOT/Jellyfin.Plugin.Template/meta.json'))['name'])"
)"
PLUGIN_VERSION="$(
  python3 -c "import json; print(json.load(open('$ROOT/Jellyfin.Plugin.Template/meta.json'))['version'])"
)"
export PLUGIN_NAME
export PLUGIN_STAGE="$ROOT/artifacts/plugin/$PLUGIN_NAME"

cleanup() {
  docker compose -f docker-compose.yml logs jellyfin 2>/dev/null || true
  docker compose -f docker-compose.yml down -v 2>/dev/null || true
}
trap cleanup EXIT

rm -rf "$CI_DATA"
mkdir -p "$CONFIG" "$CACHE"

"$ROOT/scripts/stage-plugin.sh"
docker compose -f docker-compose.yml up -d

BASE="http://localhost:$JELLYFIN_PORT"
deadline=$((SECONDS + 240))
until docker compose -f docker-compose.yml logs jellyfin 2>/dev/null \
  | grep -Fq "Loaded plugin: $PLUGIN_NAME $PLUGIN_VERSION"; do
  if (( SECONDS > deadline )); then
    echo "smoke.sh: plugin load timeout (expected log: Loaded plugin: $PLUGIN_NAME $PLUGIN_VERSION)" >&2
    exit 1
  fi
  sleep 3
done

until docker compose -f docker-compose.yml logs jellyfin 2>/dev/null \
  | grep -Fq "Startup complete"; do
  if (( SECONDS > deadline )); then
    echo "smoke.sh: Jellyfin startup complete timeout" >&2
    exit 1
  fi
  sleep 3
done

until curl -fsS "$BASE/health" >/dev/null 2>&1; do
  if (( SECONDS > deadline )); then
    echo "smoke.sh: Jellyfin health timeout" >&2
    exit 1
  fi
  sleep 3
done

SMOKE_API_TOKEN="$(./bootstrap.sh "$BASE")"

PLUGINS="$(
  curl -fsS "$BASE/Plugins" \
    -H "X-Emby-Token: $SMOKE_API_TOKEN"
)"

python3 - "$PLUGINS" "$PLUGIN_NAME" "$PLUGIN_VERSION" <<'PY'
import json, sys
plugins = json.loads(sys.argv[1])
name, version = sys.argv[2], sys.argv[3]
for p in plugins:
    if p.get("Name") == name and p.get("Version") == version:
        print(f"smoke OK: {name} {version}")
        sys.exit(0)
print(f"smoke FAIL: plugin {name} {version} not in /Plugins", file=sys.stderr)
print(json.dumps(plugins, indent=2)[:2000], file=sys.stderr)
sys.exit(1)
PY
