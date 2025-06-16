#!/usr/bin/env bash
set -e
REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$REPO_ROOT"

PREV_TAG=$(git describe --tags --abbrev=0 2>/dev/null || echo "")
if [ -n "$PREV_TAG" ]; then
  echo "Rolling back to $PREV_TAG"
  git checkout "$PREV_TAG"
else
  echo "No tag found for rollback. Manual intervention required." >&2
  exit 1
fi
