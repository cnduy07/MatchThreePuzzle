# Worklog

Before starting any task, read this file and compare active entries against the domains in `docs/OWNERSHIP.md`. When starting substantive work, append a new entry that claims the domains and files expected to change. When the task is finished, merged, or abandoned, mark the entry as `DONE` or `ABANDONED` instead of deleting it, so the repository keeps a short coordination history.

Entry template:

```text
[STATUS] <short task title>

Branch: <branch name>
Domains touched (see docs/OWNERSHIP.md): <domain names>
Agent role: <Architecture Agent or Feature Agent>
Files/scenes expected to change: <list>
Shared API changes: <yes/no; Architecture Agent only>
Started: <date>
Closed: <date or "in progress">
Notes: <anything another session should know, e.g. "will modify LevelDatabase.asset, please rebase after merge">

- STATUS values: ACTIVE, DONE, ABANDONED.
```

## Entries

[DONE] (example) Add Level_002 data asset

Branch: codex/example-level-002
Domains touched (see docs/OWNERSHIP.md): Level Data
Agent role: Feature Agent
Files/scenes expected to change: Assets/Data/Levels/Level_002.asset, Assets/Resources/LevelDatabase.asset
Shared API changes: no
Started: 2026-01-01
Closed: 2026-01-01
Notes: Example only; not a real completed task.
