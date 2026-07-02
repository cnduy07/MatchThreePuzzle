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

[DONE] Menu scene polish worktree

Branch: codex/menuscene
Domains touched (see docs/OWNERSHIP.md): Menu Scene Builder
Agent role: Feature Agent
Files/scenes expected to change: Assets/Scripts/Flow/MenuSceneUiBuilder.cs
Shared API changes: no
Started: 2026-07-02
Closed: 2026-07-02
Notes: Menu-only worktree. Do not edit SceneFlow.cs, SceneBootstrapper.cs, RuntimeUiFactory.cs, RuntimeUiShell.cs, scenes, prefabs, Shared APIs, or level data from this branch.

[DONE] Level map scene polish worktree

Branch: codex/levelmapscene
Domains touched (see docs/OWNERSHIP.md): Level Map Scene Builder
Agent role: Feature Agent
Files/scenes expected to change: Assets/Scripts/Flow/LevelMapSceneUiBuilder.cs
Shared API changes: no
Started: 2026-07-02
Closed: 2026-07-02
Notes: Level-map-only worktree. Runtime Level Select polish completed in LevelMapSceneUiBuilder.cs without editing shared scene flow, RuntimeUiFactory.cs, RuntimeUiShell.cs, scenes, prefabs, Shared APIs, or LevelData assets.

[ACTIVE] Game scene HUD polish worktree

Branch: codex/gamescene
Domains touched (see docs/OWNERSHIP.md): Game Scene HUD Builder
Agent role: Feature Agent
Files/scenes expected to change: Assets/Scripts/UI/GameSceneHudBuilder.cs
Shared API changes: no
Started: 2026-07-02
Closed: in progress
Notes: Gameplay-HUD-only worktree. Do not edit RuntimeUiShell.cs, SceneFlow.cs, RuntimeUiFactory.cs, scenes, prefabs, Shared APIs, or gameplay rules from this branch.

[DONE] Split runtime UI builders for parallel scene work

Branch: codex/architecture-ui-split
Domains touched (see docs/OWNERSHIP.md): Shared Scene Flow & UI Shell, Documentation & Process
Agent role: Architecture Agent
Files/scenes expected to change: Assets/Scripts/Flow/SceneBootstrapper.cs, Assets/Scripts/Flow/MenuSceneUiBuilder.cs, Assets/Scripts/Flow/LevelMapSceneUiBuilder.cs, Assets/Scripts/UI/RuntimeUiShell.cs, Assets/Scripts/UI/GameSceneHudBuilder.cs, docs/CODEBASE_OVERVIEW.md, docs/SCENE_UI_ARCHITECTURE.md, docs/OWNERSHIP.md, WORKLOG.md
Shared API changes: no
Started: 2026-07-02
Closed: 2026-07-02
Notes: Behavior-preserving split so later menuscene, levelmapscene, and gamescene branches can work in separate files.

[DONE] (example) Add Level_002 data asset

Branch: codex/example-level-002
Domains touched (see docs/OWNERSHIP.md): Level Data
Agent role: Feature Agent
Files/scenes expected to change: Assets/Data/Levels/Level_002.asset, Assets/Resources/LevelDatabase.asset
Shared API changes: no
Started: 2026-01-01
Closed: 2026-01-01
Notes: Example only; not a real completed task.
