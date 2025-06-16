#!/usr/bin/env bash
set -e

CONFIGURATION=${1:-Release}
PUBLISH_DIR="../publish"

echo "Publishing solution in $CONFIGURATION configuration..."
dotnet publish ../ASL.LivingGrid.sln -c $CONFIGURATION -o "$PUBLISH_DIR"

echo "Output located at $PUBLISH_DIR"
