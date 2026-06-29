#!/usr/bin/env bash
# Validate plugins.catalog.yaml against JSON Schema and repo rules.
#
# AGENTS.md marker contract (regenerate-agents-index.sh):
#   <!-- catalog-index:start -->
#   <!-- catalog-index:end -->
#
# Usage:
#   ./scripts/validate-catalog.sh [--file PATH]
#   ./scripts/validate-catalog.sh --self-check
set -euo pipefail

ROOT="$(cd "$(dirname "$0")/.." && pwd)"
cd "$ROOT"

VENV="$ROOT/.venv-catalog"
if [ ! -x "$VENV/bin/python" ]; then
  python3 -m venv "$VENV"
  "$VENV/bin/pip" install --quiet -r "$ROOT/scripts/catalog-requirements.txt"
fi

"$VENV/bin/python" - "$@" <<'PY'
import json
import re
import sys
from pathlib import Path

import jsonschema
import yaml

ROOT = Path(".").resolve()
SCHEMA_PATH = ROOT / "schemas" / "plugins.catalog.schema.json"
DEFAULT_CATALOG = ROOT / "plugins.catalog.yaml"
FIXTURES = ROOT / "scripts" / "fixtures" / "catalog"
REPO_URL = re.compile(r"^(https?://|git@)")


def validate_file(catalog_path: Path) -> list[str]:
    errors: list[str] = []
    schema = json.loads(SCHEMA_PATH.read_text())

    try:
        data = yaml.safe_load(catalog_path.read_text())
    except yaml.YAMLError as exc:
        return [f"{catalog_path}: invalid YAML: {exc}"]

    if data is None:
        return [f"{catalog_path}: empty catalog"]

    try:
        jsonschema.validate(instance=data, schema=schema)
    except jsonschema.ValidationError as exc:
        path = ".".join(str(p) for p in exc.absolute_path) or "root"
        errors.append(f"{catalog_path}: schema: {path}: {exc.message}")
        return errors

    slugs: set[str] = set()
    for index, entry in enumerate(data.get("entries", [])):
        slug = entry.get("slug", f"entry[{index}]")
        if slug in slugs:
            errors.append(f"{catalog_path}: duplicate slug: {slug}")
        slugs.add(slug)

        doc_path = entry.get("doc_path")
        if doc_path and not (ROOT / doc_path).is_file():
            errors.append(f"{catalog_path}: doc_path missing: {doc_path}")

        if entry.get("entry_type") == "plugin":
            repo_url = entry.get("repo_url")
            if not repo_url:
                errors.append(f"{catalog_path}: entry '{slug}': plugin requires repo_url")
            elif not REPO_URL.match(repo_url):
                errors.append(f"{catalog_path}: entry '{slug}': invalid repo_url: {repo_url}")

        tool_path = entry.get("optional_tool_path")
        if tool_path and not (ROOT / tool_path).exists():
            errors.append(f"{catalog_path}: optional_tool_path missing: {tool_path}")

    return errors


def run_self_check() -> int:
    cases = [
        (FIXTURES / "valid-minimal.yaml", True),
        (FIXTURES / "missing-required-field.yaml", False),
        (FIXTURES / "plugin-missing-repo-url.yaml", False),
        (FIXTURES / "pattern-no-repo-url.yaml", True),
        (FIXTURES / "broken-doc-path.yaml", False),
    ]
    failed = False
    for path, should_pass in cases:
        errors = validate_file(path)
        ok = not errors
        if ok != should_pass:
            failed = True
            expected = "pass" if should_pass else "fail"
            got = "pass" if ok else "fail"
            print(f"self-check: {path.name}: expected {expected}, got {got}", file=sys.stderr)
            for err in errors:
                print(f"  {err}", file=sys.stderr)
    if failed:
        return 1
    print("self-check: all fixture cases passed")
    return 0


def main() -> int:
    args = sys.argv[1:]
    if args == ["--self-check"]:
        return run_self_check()

    catalog = DEFAULT_CATALOG
    if len(args) == 2 and args[0] == "--file":
        catalog = Path(args[1])
    elif args:
        print(f"validate-catalog.sh: unknown args: {' '.join(args)}", file=sys.stderr)
        return 1

    errors = validate_file(catalog)
    if errors:
        for err in errors:
            print(err, file=sys.stderr)
        return 1

    print(f"catalog OK: {catalog}")
    return 0


if __name__ == "__main__":
    sys.exit(main())
PY
