#!/usr/bin/env bash
# Verify manifest.json checksum matches the zip at sourceUrl (or a local zip path).
#
# Usage:
#   ./scripts/verify-manifest.sh
#   ./scripts/verify-manifest.sh path/to/plugin.zip
set -euo pipefail

ROOT="$(cd "$(dirname "$0")/.." && pwd)"
MANIFEST="$ROOT/manifest.json"

if [[ ! -f "$MANIFEST" ]]; then
  echo "verify-manifest.sh: missing $MANIFEST" >&2
  exit 1
fi

python3 - "$MANIFEST" "${1:-}" <<'PY'
import hashlib
import json
import sys
import urllib.request
from pathlib import Path

manifest_path = Path(sys.argv[1])
local_zip = sys.argv[2] if len(sys.argv) > 2 else ""

data = json.loads(manifest_path.read_text())
if not data:
    raise SystemExit("manifest is empty")

version = data[0]["versions"][0]
expected = version["checksum"].lower()
source_url = version["sourceUrl"]

if local_zip:
    blob = Path(local_zip).read_bytes()
    label = local_zip
else:
    print(f"downloading {source_url}")
    blob = urllib.request.urlopen(source_url).read()
    label = source_url

actual = hashlib.md5(blob).hexdigest()
if actual != expected:
    print(f"FAIL: checksum mismatch for {label}", file=sys.stderr)
    print(f"  manifest: {expected}", file=sys.stderr)
    print(f"  actual:   {actual}", file=sys.stderr)
    raise SystemExit(1)

print(f"OK: checksum {actual} matches manifest ({label})")
PY
