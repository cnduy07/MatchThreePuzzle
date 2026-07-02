# Ownership Map

This document maps the repository into ownership domains so multiple Codex sessions can work in parallel without editing the same files or assets at the same time. Use it with `WORKLOG.md` to claim domains, spot conflicts early, and identify domains that require exclusive access.

## Domains

| Domain | Key files/folders | Conflict risk | Notes |
| --- | --- | --- | --- |
| Shared APIs | Public interfaces and method signatures, ScriptableObject schemas, save data models, core manager contracts, event contracts | Very High | Only an Architecture Agent may change these contracts. Feature agents may consume them but must not modify them, even when the files also belong to another domain. |
| Gameplay Core | `Assets/Scripts/Board.cs`, `GamePiece.cs`, `Tile.cs`, `Bomb.cs`, `Collectibles.cs` | Very High | `Board.cs` is a large shared file called out in `docs/TECHNICAL_PLAN.md` as a known risk. It mixes board rules, spawning, scoring hooks, particles, input, and level setup, so it should generally be owned by only one session at a time. |
| Game Loop & Scoring | `Assets/Scripts/GameManager.cs`, `ScoreManager.cs`, `SoundManager.cs` | High | These scripts coordinate start/win/lose flow, score display, move limits, audio, and gameplay state. Changes can overlap with UI shell, level data, and gameplay-core tasks. |
| Scene Flow & UI Shell | `Assets/Scripts/Flow/SceneFlow.cs`, `SceneBootstrapper.cs`, `SceneNames.cs`, `Assets/Scripts/UI/RuntimeUiShell.cs`, `RuntimeUiFactory.cs`, `RuntimeSettingsState.cs`, `SafeAreaRoot.cs`, `Assets/Scripts/MessageWindow.cs`, `ScreenFader.cs`, `RectXformMove.cs`, `Singleton.cs` | Very High | Used by every scene and every major UI flow. Runtime UI shell changes can affect menu, level select, gameplay HUD, overlays, save/progression display, and reusable scene loading. |
| Level Data | `Assets/Scripts/LevelData.cs`, `LevelDatabase.cs`, `LevelLoader.cs`, `Assets/Data/Levels/*`, `Assets/Resources/LevelDatabase.asset` | High | Exclusive when adding, removing, reordering, or retuning levels. Two sessions editing level assets or the database concurrently can silently overwrite level ordering or references. |
| Unity Scenes | `Assets/Scenes/*.unity` | Very High | Scene YAML is easy to merge incorrectly. Never let two branches modify the same scene file at the same time, even for apparently unrelated UI or object changes. |
| Prefabs | `Assets/Prefabs/**` | High | Prefab YAML has the same merge risk as scenes. Shared UI, piece, tile, bomb, and collectible prefab work should be claimed narrowly and verified in Unity after merge. |
| Art, Sprites & Theme Resources | `Assets/Sprites/**`, `Assets/Scripts/ThemeData.cs`, `PieceSetData.cs`, `GameResourceLibrary.cs`, `Assets/Data/Themes/*`, `Assets/Data/PieceSets/*`, `Assets/Resources/GameResourceLibrary.asset` | Low-to-Medium | Low risk when scoped to one sprite folder or one theme asset. Risk rises when changing shared theme/resource registries because every scene and level can consume them. |
| Progression & Settings | `Assets/Scripts/PlayerProgress.cs` and save/progression display usage in runtime UI | Medium | Save schema, unlock logic, stars, settings, and display hooks should be coordinated with level-select and game-flow work. |
| Effects & Audio | `Assets/Scripts/ParticleManager.cs`, `ParticlePlayer.cs`, `Assets/Particles/**`, `Assets/Sounds/**` | Medium | Usually separable from gameplay rules, but effect/audio references can be wired through board, prefabs, theme assets, or sound manager. |
| Project-Wide Config | `ProjectSettings/`, `Packages/manifest.json`, `Packages/packages-lock.json`, `.gitignore`, build settings | Very High | Changes affect every session and should be treated as exclusive-lock-only. Avoid package, Unity version, build setting, or ignore-rule changes while other sessions have active work. |
| Documentation & Process | `AGENTS.md`, `WORKLOG.md`, `docs/**` | Low-to-Medium | Documentation is generally mergeable, but ownership/process changes affect all sessions. Coordinate when changing this ownership map or workflow rules. |

## Domain Ownership Rules

- A session may only edit files inside domains it has explicitly claimed in `WORKLOG.md`.
- `Very High` and `High` risk domains must be claimed exclusively. No two sessions may claim the same domain at the same time.
- If a task requires touching a file outside the session's claimed domain, the session must stop, note the conflict, and ask the user rather than editing it silently.
- Prefer assigning each parallel session a task that maps to exactly one domain, or to a small combination of low-risk domains.
- Avoid feature labels that hide file overlap. For example, "Level Select scene" and "Game scene" can look separate, but both may touch Scene Flow & UI Shell and Level Data. Treat that as a conflict unless the exact file lists are separate and logged.

## Shared API Rules

- Only an Architecture Agent may change Shared APIs.
- Shared APIs include public interfaces, public method signatures used across domains, ScriptableObject field schemas, save data models, core manager contracts, and event contracts.
- Feature agents may consume Shared APIs but must not modify them.
- If a feature task appears to require a Shared API change, the feature agent must stop, add a note in `WORKLOG.md`, and ask the user to route the API change through an Architecture Agent.
- Shared API changes must be claimed explicitly in `WORKLOG.md` as the `Shared APIs` domain, in addition to any file domain touched by the implementation.

## Git Workflow For Parallel Sessions

- Each session should work on its own git branch, ideally through `git worktree add`, so sessions run in separate working directories without needing to stash or switch branches.
- Merge branches back to `main` one at a time, not in bulk, especially for any branch that touched `Assets/Scenes` or `Assets/Prefabs`.
- Enable Unity's Smart Merge (`UnityYAMLMerge`) as the git mergetool for `.unity` and `.prefab` files.
- A clean Smart Merge result still must be manually opened and verified in the Unity Editor before trusting it. Auto-merged scene or prefab YAML can be syntactically valid but semantically broken, such as duplicated GameObjects or broken component references, without a merge conflict being reported.
- Never let two open branches modify the same scene or prefab file at the same time, even if the changes seem unrelated, such as two different UI elements in the same scene.
