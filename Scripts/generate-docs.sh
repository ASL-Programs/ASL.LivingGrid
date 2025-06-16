#!/usr/bin/env bash
set -e
REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
DOCS_DIR="$REPO_ROOT/docs_output"

if ! command -v docfx >/dev/null 2>&1; then
  echo "docfx command not found. Please install docfx."
  exit 1
fi

cd "$REPO_ROOT"
docfx Docs/docfx.json -o "$DOCS_DIR"

echo "Documentation generated in $DOCS_DIR"
