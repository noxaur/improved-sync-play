#!/usr/bin/env bash
# Build Release and stage plugin DLL + meta.json for Docker mount.
set -euo pipefail

ROOT="$(cd "$(dirname "$0")/.." && pwd)"
cd "$ROOT"

PLUGIN_NAME="$(
  python3 -c "import json; print(json.load(open('Jellyfin.Plugin.Template/meta.json'))['name'])"
)"
OUT_DIR="$ROOT/Jellyfin.Plugin.Template/bin/Release/net9.0"
STAGE="$ROOT/artifacts/plugin/$PLUGIN_NAME"

if [[ "${STAGE_SKIP_BUILD:-}" == "1" ]]; then
  if [[ -f "$STAGE/Jellyfin.Plugin.Template.dll" && -f "$STAGE/meta.json" ]]; then
    echo "staged (skip build): $STAGE"
    exit 0
  fi
  echo "stage-plugin.sh: STAGE_SKIP_BUILD=1 but $STAGE is missing" >&2
  exit 1
fi

dotnet build -c Release Jellyfin.Plugin.Template.sln

rm -rf "$STAGE"
mkdir -p "$STAGE"
cp "$OUT_DIR/Jellyfin.Plugin.Template.dll" "$STAGE/"
cp "$OUT_DIR/meta.json" "$STAGE/"

echo "staged: $STAGE"
