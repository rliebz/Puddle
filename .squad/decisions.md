# Squad Decisions

## Active Decisions

No decisions recorded yet.

## Governance

- All meaningful changes require team consensus
- Document architectural decisions here
- Keep history focused on work, decisions focused on direction

## Model policy (Puddle)
**By:** Jake  
**What:**  
- Leader/orchestrator uses **Opus 4.7 (1M context)**.  
- Code reviewer, designer, and dev designer use **Opus 4.7 (Extra High)**.  
- Implementers use **Sonnet**.  
**Why:** Max context for orchestration, extra-high reasoning for critique/design, lower-cost implementers for throughput.

## Cap and stop policy (Puddle)
**By:** Jake  
**What:** If any agent hits rate limit, quota, token cap, or "premium requests disabled":  
1) Stop creating new subagents immediately  
2) Update `docs/context.md` with: what was in progress, files touched, next 3 tasks  
3) Instruct Jake to run `./scripts/pause.sh`  
4) Default action is stop and resume later (only continue if Jake explicitly opts in)  
**Why:** Prevent partial work and preserve a clean resume point.
