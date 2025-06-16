#!/usr/bin/env bash
set -e
REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$REPO_ROOT"

echo "Generating CHANGELOG.md..."

git log --pretty=format:'* %h %ad %s' --date=short > CHANGELOG.md

echo "CHANGELOG.md updated"
