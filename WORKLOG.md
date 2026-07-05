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

[DONE] Restore visible board tile layer

Branch: main
Domains touched (see docs/OWNERSHIP.md): Prefabs, Art, Sprites & Theme Resources, Documentation & Process
Agent role: Feature Agent
Files/scenes expected to change: Assets/Scripts/Editor/GeneratedSpritePrefabWireUp.cs, Assets/Prefabs/Tiles/*.prefab, docs/PIXEL_ART_DIRECTION.md, WORKLOG.md
Shared API changes: no
Started: 2026-07-03
Closed: 2026-07-03
Notes: Restored generated tile visibility by moving tile SpriteRenderers back to the visible board layer with full opacity while keeping pieces and bomb markers above them. Avoided scene edits.

[DONE] Fix generated gameplay sprite readability

Branch: main
Domains touched (see docs/OWNERSHIP.md): Prefabs, Game Scene HUD Builder, Art, Sprites & Theme Resources, Documentation & Process
Agent role: Feature Agent
Files/scenes expected to change: Assets/Scripts/Editor/GeneratedSpritePrefabWireUp.cs, Assets/Scripts/UI/GameSceneHudBuilder.cs, Assets/Prefabs/Dots/*.prefab, Assets/Prefabs/Bombs/**/*.prefab, Assets/Prefabs/Collectibles/*.prefab, Assets/Prefabs/Tiles/*.prefab, docs/PIXEL_ART_DIRECTION.md, WORKLOG.md
Shared API changes: no
Started: 2026-07-03
Closed: 2026-07-03
Notes: Fixed screenshot-reported Game scene readability problems by moving generated tile SpriteRenderers behind pieces, moving pieces/bomb markers above tiles, reducing tile opacity, and tightening the runtime gameplay HUD/threat/booster layout. Avoided scene edits.

[DONE] Wire generated gameplay sprites into prefabs

Branch: main
Domains touched (see docs/OWNERSHIP.md): Prefabs, Art, Sprites & Theme Resources, Documentation & Process
Agent role: Feature Agent
Files/scenes expected to change: Assets/Prefabs/Dots/*.prefab, Assets/Prefabs/Bombs/**/*.prefab, Assets/Prefabs/Collectibles/*.prefab, Assets/Prefabs/Tiles/*.prefab, Assets/Data/PieceSets/ClassicPieceSet.asset, Assets/Data/Themes/ClassicTheme.asset, docs/CODEBASE_OVERVIEW.md, docs/PIXEL_ART_DIRECTION.md, WORKLOG.md
Shared API changes: no
Started: 2026-07-03
Closed: 2026-07-03
Notes: Wired generated PixelArt/Generated sprites into existing dot, bomb, collectible, blocker, and tile prefab renderers. Added editor utility to normalize generated sprite imports and reapply prefab wiring. Avoided scene edits.

[DONE] Generate core pixel-art gameplay sprites

Branch: main
Domains touched (see docs/OWNERSHIP.md): Art, Sprites & Theme Resources, Documentation & Process
Agent role: Feature Agent
Files/scenes expected to change: Assets/Sprites/PixelArt/Generated/*, generated sprite .meta files if Unity creates them, docs/PIXEL_ART_DIRECTION.md, WORKLOG.md
Shared API changes: no
Started: 2026-07-03
Closed: 2026-07-03
Notes: User explicitly requested PixelLab MCP generation in this thread. Generated 16 candidate sprites under Assets/Sprites/PixelArt/Generated/ and normalized their import metas. Wiring into prefabs/theme should be a later resource pass after visual review.

[DONE] Wire menu button sprites

Branch: main
Domains touched (see docs/OWNERSHIP.md): Shared APIs, Menu Scene Builder, Art, Sprites & Theme Resources, Documentation & Process
Agent role: Architecture Agent
Files/scenes expected to change: Assets/Scripts/ThemeData.cs, Assets/Scripts/Flow/MenuSceneUiBuilder.cs, Assets/Data/Themes/ClassicTheme.asset, docs/CODEBASE_OVERVIEW.md, docs/SCENE_UI_ARCHITECTURE.md, WORKLOG.md
Shared API changes: yes; adds menu-specific sprite slots to ThemeData
Started: 2026-07-03
Closed: 2026-07-03
Notes: Wired user-provided Assets/Sprites/menu_*_btn.png sprites into the runtime-built Menu screen through ThemeData and ClassicTheme. PixelLab/image generation remains disabled for this thread per AGENTS.md and docs/PIXEL_ART_DIRECTION.md.

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

[DONE] Game scene HUD polish worktree

Branch: codex/gamescene
Domains touched (see docs/OWNERSHIP.md): Game Scene HUD Builder
Agent role: Feature Agent
Files/scenes expected to change: Assets/Scripts/UI/GameSceneHudBuilder.cs
Shared API changes: no
Started: 2026-07-02
Closed: 2026-07-02
Notes: Gameplay-HUD-only worktree. Completed runtime HUD polish in GameSceneHudBuilder.cs only. Unity batchmode verification is blocked by unrelated TextMesh Pro example compile errors in VertexZoom.cs and TMP_TextSelector_B.cs.

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
[DONE] Game scene sprite UI and backdrop polish

Branch: main
Domains touched (see docs/OWNERSHIP.md): Shared APIs, Shared Scene Flow & UI Shell, Game Scene HUD Builder, Prefabs, Art, Sprites & Theme Resources, Documentation & Process
Agent role: Architecture Agent
Files/scenes expected to change: Assets/Scripts/ThemeData.cs, Assets/Scripts/UI/RuntimeUiShell.cs, Assets/Scripts/UI/GameSceneHudBuilder.cs, Assets/Scripts/Editor/GeneratedSpritePrefabWireUp.cs, Assets/Prefabs/Tiles/*.prefab, Assets/Data/Themes/ClassicTheme.asset, Assets/Sprites/PixelArt/Generated/*, docs/CODEBASE_OVERVIEW.md, docs/PIXEL_ART_DIRECTION.md, docs/SCENE_UI_ARCHITECTURE.md, WORKLOG.md
Shared API changes: yes; adds gameplay HUD sprite slots to ThemeData
Started: 2026-07-03
Closed: 2026-07-03
Notes: Fixed screenshot-reported weird tile backing by using a clean full-cell board tile, added a full-canvas gameplay background layer, and wired generated booster/pause HUD sprites through ClassicTheme. Avoided scene YAML edits.
[DONE] Rebuild game scene visual hierarchy

Branch: main
Domains touched (see docs/OWNERSHIP.md): Shared Scene Flow & UI Shell, Game Scene HUD Builder, Prefabs, Art, Sprites & Theme Resources, Documentation & Process
Agent role: Architecture Agent
Files/scenes expected to change: Assets/Scripts/UI/RuntimeUiShell.cs, Assets/Scripts/UI/GameSceneHudBuilder.cs, Assets/Scripts/Editor/GeneratedSpritePrefabWireUp.cs, Assets/Prefabs/Tiles/*.prefab, Assets/Data/Themes/ClassicTheme.asset, docs/CODEBASE_OVERVIEW.md, docs/PIXEL_ART_DIRECTION.md, docs/SCENE_UI_ARCHITECTURE.md, WORKLOG.md
Shared API changes: no
Started: 2026-07-03
Closed: 2026-07-03
Notes: Removed overlay-canvas gameplay background that hid the world board, replaced it with a world-space background sprite behind the board, rebuilt bottom boosters as larger readable cards, cleared the bad cropped booster-frame sprite, and made normal tile backing visibly opaque. Avoided scene YAML edits. Batchmode verification was blocked because the Unity editor was open on this project.
[DONE] Review remaining issues and synchronize docs

Branch: main
Domains touched (see docs/OWNERSHIP.md): Documentation & Process
Agent role: Feature Agent
Files/scenes expected to change: docs/CODEBASE_OVERVIEW.md, docs/TECHNICAL_PLAN.md, docs/GAME_DESIGN_TARGET.md, docs/SCENE_UI_ARCHITECTURE.md, docs/PIXEL_ART_DIRECTION.md, WORKLOG.md
Shared API changes: no
Started: 2026-07-05
Closed: 2026-07-05
Notes: Synchronized docs around current remaining issues: Game scene visual QA, candidate art status, visual-only boosters, score-only objective resolution, and world-space background layering. Avoided scenes, prefabs, Shared APIs, and unrelated dirty project settings.
