#!/usr/bin/env bash
set -euo pipefail

echo "Pause: updating git status and checkpointing"
git status

echo "Pause: staging all changes"
git add -A

msg="wip: checkpoint (pause)"
echo "Pause: committing: $msg"
git commit -m "$msg" || echo "Nothing to commit"

echo "Pause: pushing branch"
git push

echo "Pause complete"
echo "Next: update docs/context.md if needed, then exit copilot"
