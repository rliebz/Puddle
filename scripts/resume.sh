#!/usr/bin/env bash
set -euo pipefail

branch="squad/web-port-spike"
echo "Resume: switching to $branch"
git checkout "$branch"

echo "Resume: pulling latest"
git pull

echo "Resume: open docs/context.md and follow Next 3 tasks"  # Launch Copilot with the Squad agent, auto-resume the last session, and enable YOLO mode copilot --agent squad --continue --yolo 
