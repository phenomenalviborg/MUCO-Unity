#!/usr/bin/env bash
# Updates the "description" field in package.json to include the package
# version and current git commit.
#
# Usage:  bash update-package-description.sh
#         (run from the MUCO-Unity package root, or pass the root as $1)

set -euo pipefail

cd "${1:-$(dirname "$0")}"

version=$(jq -r '.version' package.json)
commit=$(git rev-parse --short HEAD)

jq --arg desc "MUCO Core Package v${version} (commit ${commit})" \
  '.description = $desc' \
  package.json > package.json.tmp

mv package.json.tmp package.json

echo "✨ package.json description → MUCO Core Package v${version} (commit ${commit})"