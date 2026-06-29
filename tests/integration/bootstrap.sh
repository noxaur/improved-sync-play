#!/usr/bin/env bash
# Complete Jellyfin first-run wizard for CI smoke (ephemeral admin user).
# Prints access token to stdout; status messages go to stderr.
set -euo pipefail

BASE_URL="${1:-http://localhost:8096}"
ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
SDK_VERSION="$(
  grep -oP '(?<=<JellyfinSdkVersion>)[^<]+' "$ROOT/versions.props" | tr -d '[:space:]'
)"
USER="${SMOKE_ADMIN_USER:-smokeadmin}"
PASS="${SMOKE_ADMIN_PASS:-SmokeTestPass1!}"

AUTH_HEADER="MediaBrowser Client=\"smoke-ci\", Device=\"smoke-ci\", DeviceId=\"00000000-0000-0000-0000-000000000001\", Version=\"$SDK_VERSION\""

wizard_done() {
  python3 - "$BASE_URL" <<'PY'
import json, sys, urllib.request
url = sys.argv[1] + "/System/Info/Public"
with urllib.request.urlopen(url) as r:
    info = json.load(r)
sys.exit(0 if info.get("StartupWizardCompleted") else 1)
PY
}

complete_wizard() {
  echo "bootstrap: completing startup wizard" >&2
  local deadline=$((SECONDS + 120))
  until curl -fsS -X POST "$BASE_URL/Startup/Configuration" \
    -H 'Content-Type: application/json' \
    -d '{"ServerName":"smoke-test","UICulture":"en-US","MetadataCountryCode":"US","PreferredMetadataLanguage":"en"}' \
    >/dev/null 2>&1; do
    if (( SECONDS > deadline )); then
      echo "bootstrap: Startup/Configuration timeout" >&2
      exit 1
    fi
    sleep 2
  done
  # Initialize placeholder user (UpdateStartupUser does not call InitializeAsync).
  curl -fsS "$BASE_URL/Startup/User" >/dev/null
  until curl -fsS -X POST "$BASE_URL/Startup/User" \
    -H 'Content-Type: application/json' \
    -d "{\"Name\":\"$USER\",\"Password\":\"$PASS\"}" >/dev/null 2>&1; do
    if (( SECONDS > deadline )); then
      echo "bootstrap: Startup/User timeout" >&2
      exit 1
    fi
    sleep 2
  done
  curl -fsS -X POST "$BASE_URL/Startup/Complete" \
    -H 'Content-Type: application/json' \
    -d '{}' >/dev/null
}

if ! wizard_done; then
  complete_wizard
fi

TOKEN="$(
  curl -fsS -X POST "$BASE_URL/Users/AuthenticateByName" \
    -H 'Content-Type: application/json' \
    -H "X-Emby-Authorization: $AUTH_HEADER" \
    -d "{\"Username\":\"$USER\",\"Pw\":\"$PASS\"}" \
    | python3 -c "import sys,json; print(json.load(sys.stdin)['AccessToken'])"
)"

echo "bootstrap OK" >&2
echo "$TOKEN"
