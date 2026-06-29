#!/usr/bin/env bash
# Regenerate manifest.json from meta.json and a release zip checksum.
#
# Usage:
#   ./scripts/update-manifest.sh <zip-path> [tag]
#   ./scripts/update-manifest.sh Improved-SyncPlay-1.0.0.0.zip v1.0.0
set -euo pipefail

ROOT="$(cd "$(dirname "$0")/.." && pwd)"
META="$ROOT/Jellyfin.Plugin.ImprovedSyncPlay/meta.json"
MANIFEST="$ROOT/manifest.json"

usage() {
  echo "Usage: $0 <zip-path> [tag]" >&2
  exit 1
}

[[ $# -ge 1 ]] || usage
ZIP_PATH="$1"
TAG="${2:-}"

if [[ ! -f "$ZIP_PATH" ]]; then
  echo "update-manifest.sh: zip not found: $ZIP_PATH" >&2
  exit 1
fi

if [[ ! -f "$META" ]]; then
  echo "update-manifest.sh: missing $META" >&2
  exit 1
fi

CHECKSUM="$(md5sum "$ZIP_PATH" | awk '{print $1}')"
ZIP_NAME="$(basename "$ZIP_PATH")"

python3 - "$META" "$MANIFEST" "$ZIP_NAME" "$CHECKSUM" "$TAG" <<'PY'
import json
import sys
from datetime import datetime, timezone
from pathlib import Path

meta_path, manifest_path, zip_name, checksum, tag = sys.argv[1:6]
meta = json.loads(Path(meta_path).read_text())
version = meta["version"]

if not tag:
    parts = version.split(".")
    tag = "v" + ".".join(parts[:3]) if len(parts) == 4 and parts[-1] == "0" else "v" + version

source_url = f"https://github.com/noxaur/improved-sync-play/releases/download/{tag}/{zip_name}"
timestamp = datetime.now(timezone.utc).replace(microsecond=0).isoformat().replace("+00:00", "Z")

entry = {
    "guid": meta["guid"],
    "name": meta["name"],
    "description": meta["description"],
    "overview": meta["overview"],
    "owner": meta.get("owner", "noxaur"),
    "category": meta.get("category", "General"),
    "versions": [
        {
            "version": version,
            "changelog": meta.get("changelog", ""),
            "targetAbi": meta["targetAbi"],
            "sourceUrl": source_url,
            "checksum": checksum,
            "timestamp": timestamp,
        }
    ],
}

Path(manifest_path).write_text(json.dumps([entry], indent=2) + "\n")
print(f"updated: {manifest_path} (tag {tag})")
PY
