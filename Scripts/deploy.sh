#!/usr/bin/env bash
set -e

# Determine the directory of this script so the script can be executed from
# any location. `dirname "$0"` returns the folder containing this script.
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

CONFIGURATION=${1:-Release}
PUBLISH_DIR="$SCRIPT_DIR/../publish"
SOLUTION="$SCRIPT_DIR/../ASL.LivingGrid.sln"

echo "Publishing solution in $CONFIGURATION configuration..."
dotnet publish "$SOLUTION" -c "$CONFIGURATION" -o "$PUBLISH_DIR"

echo "Output located at $PUBLISH_DIR"
