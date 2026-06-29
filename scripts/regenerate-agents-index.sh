#!/usr/bin/env bash
# Regenerate the catalog index table in AGENTS.md between marker comments.
#
# Marker contract — replace only content between these lines (markers stay):
#   <!-- catalog-index:start -->
#   <!-- catalog-index:end -->
#
# Usage:
#   ./scripts/regenerate-agents-index.sh
#   ./scripts/regenerate-agents-index.sh --check
set -euo pipefail

ROOT="$(cd "$(dirname "$0")/.." && pwd)"
cd "$ROOT"

VENV="$ROOT/.venv-catalog"
if [ ! -x "$VENV/bin/python" ]; then
  python3 -m venv "$VENV"
  "$VENV/bin/pip" install --quiet -r "$ROOT/scripts/catalog-requirements.txt"
fi

"$VENV/bin/python" - "$@" <<'PY'
import re
import sys
from pathlib import Path

import yaml

ROOT = Path(".").resolve()
CATALOG = ROOT / "plugins.catalog.yaml"
AGENTS = ROOT / "AGENTS.md"
START = "<!-- catalog-index:start -->"
END = "<!-- catalog-index:end -->"
MARKER_BLOCK = re.compile(
    re.escape(START) + r"\r?\n(?:.*?\r?\n)?" + re.escape(END),
    re.DOTALL,
)


def build_table(entries: list[dict]) -> str:
    lines = [
        "| Slug | Category | Tier | Doc |",
        "|------|----------|------|-----|",
    ]
    for entry in sorted(entries, key=lambda e: e["slug"]):
        doc_path = entry["doc_path"]
        lines.append(
            f"| `{entry['slug']}` | {entry['category']} | {entry['integration_tier']} "
            f"| [{doc_path}]({doc_path}) |"
        )
    return "\n".join(lines)


def render_agents(content: str, table: str) -> str:
    matches = list(MARKER_BLOCK.finditer(content))
    if len(matches) != 1:
        raise SystemExit(
            f"regenerate-agents-index.sh: expected exactly one marker block, found {len(matches)}"
        )
    replacement = f"{START}\n{table}\n{END}"
    return MARKER_BLOCK.sub(replacement, content, count=1)


def main() -> int:
    args = sys.argv[1:]
    if not args:
        check = False
    elif args == ["--check"]:
        check = True
    else:
        print(f"regenerate-agents-index.sh: unknown args: {' '.join(args)}", file=sys.stderr)
        return 1

    data = yaml.safe_load(CATALOG.read_text())
    table = build_table(data["entries"])
    original = AGENTS.read_text()
    updated = render_agents(original, table)

    if check:
        if updated != original:
            print("regenerate-agents-index.sh: AGENTS.md catalog index is stale", file=sys.stderr)
            print("Run ./scripts/regenerate-agents-index.sh and commit the result.", file=sys.stderr)
            return 1
        print("catalog index OK")
        return 0

    AGENTS.write_text(updated)
    print(f"updated {AGENTS}")
    return 0


if __name__ == "__main__":
    sys.exit(main())
PY
