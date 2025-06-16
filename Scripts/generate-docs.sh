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

# Copy to internal developer portal if directory exists
PORTAL_DIR="$REPO_ROOT/InternalPortal/api-docs"
if [ -d "$PORTAL_DIR" ]; then
  rm -rf "$PORTAL_DIR"/*
  cp -r "$DOCS_DIR"/* "$PORTAL_DIR"/
fi

echo "Documentation generated in $DOCS_DIR"
