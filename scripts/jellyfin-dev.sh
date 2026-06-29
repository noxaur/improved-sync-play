#!/usr/bin/env bash
# Local Jellyfin test server with staged plugin mounted.
# Usage: jellyfin-dev.sh up|down|deploy|logs
set -euo pipefail

ROOT="$(cd "$(dirname "$0")/.." && pwd)"
COMPOSE="$ROOT/tests/integration/docker-compose.yml"
PLUGIN_NAME="$(
  python3 -c "import json; print(json.load(open('$ROOT/Jellyfin.Plugin.Template/meta.json'))['name'])"
)"

export JELLYFIN_CONFIG="${JELLYFIN_CONFIG:-$ROOT/.jellyfin/config}"
export JELLYFIN_CACHE="${JELLYFIN_CACHE:-$ROOT/.jellyfin/cache}"
export PLUGIN_STAGE="${PLUGIN_STAGE:-$ROOT/artifacts/plugin/$PLUGIN_NAME}"
export PLUGIN_NAME
export JELLYFIN_PORT="${JELLYFIN_PORT:-8096}"

export JELLYFIN_IMAGE_TAG="$(
  grep -oP '(?<=<JellyfinSdkVersion>)[^<]+' "$ROOT/versions.props" | tr -d '[:space:]'
)"

cmd="${1:-}"
case "$cmd" in
  up)
    "$ROOT/scripts/stage-plugin.sh"
    mkdir -p "$JELLYFIN_CONFIG" "$JELLYFIN_CACHE"
    docker compose -f "$COMPOSE" up -d
    echo "Jellyfin: http://localhost:$JELLYFIN_PORT"
    echo "Complete first-run wizard, then check Dashboard → Plugins."
    ;;
  down)
    docker compose -f "$COMPOSE" down
    ;;
  deploy)
    "$ROOT/scripts/stage-plugin.sh"
    docker compose -f "$COMPOSE" restart jellyfin
    echo "Plugin re-staged; Jellyfin restarted (plugins load at startup)."
    ;;
  logs)
    docker compose -f "$COMPOSE" logs -f jellyfin
    ;;
  *)
    echo "Usage: $0 up|down|deploy|logs" >&2
    exit 1
    ;;
esac
