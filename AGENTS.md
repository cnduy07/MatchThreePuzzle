# AGENTS.md

## Purpose

This repository is a Unity 2D match-3 puzzle game. The goal is to finish it as a polished pixel-art mobile game for iPhone and iPad, with App Store release quality.

Use this file as the durable project guide for Codex agents. Read the focused docs in `docs/` before making substantive changes:

- `docs/CODEBASE_OVERVIEW.md`
- `docs/GAME_DESIGN_TARGET.md`
- `docs/SCENE_UI_ARCHITECTURE.md`
- `docs/PIXEL_ART_DIRECTION.md`
- `docs/TECHNICAL_PLAN.md`
- `docs/IOS_APPSTORE_PLAN.md`

## Documentation Map

- `docs/CODEBASE_OVERVIEW.md` is the fastest progress dashboard. Use its implemented and missing/not-release-ready sections to answer "what works now?" and "what is still open?"
- `docs/TECHNICAL_PLAN.md` is the roadmap. Use it to decide the next engineering phase and to track remaining work inside each phase.
- `docs/SCENE_UI_ARCHITECTURE.md` is the scene/UI source of truth. Use it for scene flow, overlays, safe areas, shared prefabs, and resource reuse.
- `docs/GAME_DESIGN_TARGET.md` is the product design target. Use it for gameplay goals, intended screens, progression, tutorial, and player-facing UX.
- `docs/PIXEL_ART_DIRECTION.md` is the art workflow and style guide. It is planning-only unless a separate approved artwork workflow provides assets.
- `docs/IOS_APPSTORE_PLAN.md` is the release checklist. Use it for iPhone/iPad QA, privacy, build settings, metadata, and App Store readiness.
- `docs/OWNERSHIP.md` and `WORKLOG.md` coordinate parallel Codex sessions by domain, branch, and expected file changes.

## Current Project Snapshot

- Unity version: `6000.5.0f1`.
- Build scene flow: `Assets/Scenes/Boot.unity` -> `Menu.unity` -> `Level Select.unity` -> `Game.unity`.
- Reference legacy scene: `Assets/Scenes/Level 1.unity`.
- Core gameplay scripts: `Assets/Scripts/Board.cs`, `GamePiece.cs`, `Tile.cs`, `Bomb.cs`, `Collectibles.cs`, `LevelData.cs`, `LevelDatabase.cs`.
- Shared resource scripts: `ThemeData.cs`, `PieceSetData.cs`, `GameResourceLibrary.cs`.
- Game flow scripts: `GameManager.cs`, `ScoreManager.cs`, `SoundManager.cs`, `LevelLoader.cs`, `SceneFlow.cs`, `SceneBootstrapper.cs`, `RuntimeUiShell.cs`, `PlayerProgress.cs`, `MessageWindow.cs`, `ScreenFader.cs`, `RectXformMove.cs`, `Singleton.cs`.
- Existing gameplay: board fill, swap, match detection, cascades, score, move limit, row bombs, column bombs, adjacent bombs, color bombs, breakable tiles, obstacles, collectibles, basic particles, basic sounds, start/win/lose dialogs, first-pass local progression save.
- Visual/UX concept references: `Assets/Concept/menuscene.png`, `levelmapscene.png`, and `gamescene.png`.
- Target platforms: iPhone and iPad.
- Target store: Apple App Store.
- Target art direction: cohesive pixel-art match-3 puzzle game.

## Non-Negotiables

- Do not edit `Library/`, `Temp/`, `Logs/`, or generated IDE files.
- Preserve Unity `.meta` files. When adding assets, keep their `.meta` files once Unity creates them.
- Do not commit secrets, API tokens, MCP bearer tokens, provisioning profiles, or private signing data.
- Do not put PixelLab tokens in docs, prompts, scene files, or source files.
- Keep changes scoped. Do not refactor unrelated systems while fixing one feature.
- Treat the existing dirty worktree as user-owned unless the user explicitly asks to clean it.
- Prefer Unity built-ins already in the project before adding runtime packages.
- Keep mobile performance and touch UX in mind for every gameplay, UI, and art decision.
- Follow the scene/UI architecture in `docs/SCENE_UI_ARCHITECTURE.md`: small scene set, reusable overlay panels, data-driven levels, shared UI prefabs, and theme/resource reuse.
- For UI/UX layout and product feel, refer to `Assets/Concept/` before changing menu, level select, gameplay HUD, boosters, win/lose presentation, or visual hierarchy.
- Check `docs/OWNERSHIP.md` and `WORKLOG.md` before editing files; do not work in a domain another session has marked `ACTIVE`.

## Parallel Session Work Protocol

This repository may be worked on by multiple Codex sessions in parallel, each typically on its own git branch or git worktree.

- Before making any changes, every agent session must read `docs/OWNERSHIP.md` and `WORKLOG.md`.
- Before starting substantive work, every agent session must append a `WORKLOG.md` entry that names its branch, claimed domains, and expected file or scene changes.
- When work is finished, merged, or abandoned, update that `WORKLOG.md` entry to `DONE` or `ABANDONED` rather than deleting it.
- Do not edit files in a domain currently claimed `ACTIVE` by another session in `WORKLOG.md`.
- Do not edit `Very High` risk domains from `docs/OWNERSHIP.md` without first confirming there is no conflicting `ACTIVE` entry.
- If a task's scope turns out to span multiple domains or conflicts with an `ACTIVE` entry, stop and flag the conflict to the user rather than proceeding silently.
- Shared APIs are architecture-owned. Only an Architecture Agent may change public interfaces, public method signatures used across domains, ScriptableObject schemas, save data models, core manager contracts, or event contracts.
- Feature agents may consume Shared APIs but must not modify them. If a feature needs a Shared API change, stop and ask the user to route that change through an Architecture Agent.

## First Fix Priorities

### Recently fixed foundation bugs

1. Board setup is guarded so `SetupBoard()` initializes a level only once.
2. `Singleton<T>.Awake()` destroys duplicate objects instead of destroying the existing singleton instance.
3. Board input now uses a Board-owned mouse/touch screen-to-grid path instead of `Tile.OnMouse*` callbacks.

### Next initiatives - see `docs/TECHNICAL_PLAN.md` Phase 3 (remaining) - Phase 5

1. Phone/iPad visual verification and final responsive HUD tuning.
2. Final shared UI prefabs replacing runtime placeholder controls.
3. Final pixel-art UI prefab replacement and sprite atlas grouping on top of the new shared UI prefab / `ThemeData` / `PieceSetData` / `GameResourceLibrary` foundation.
4. Additional LevelData tuning plus objective-rule expansion beyond score goals.
5. Pixel-art replacement pass using approved assets from the separate artwork workflow.

## How To Work

- Read the relevant docs before making changes.
- Inspect prefabs and scene YAML carefully before editing Unity assets by hand.
- Keep these Markdown files current. If a task adds a feature, fixes a major bug, changes architecture, changes art direction, changes the PixelLab workflow, changes release assumptions, or discovers an important project risk, update the relevant `.md` file in the same work session.
- If no documentation update is needed, mention why in the final response.
- Prefer small, verifiable steps:
  - gameplay correctness
  - mobile UX
  - art replacement
  - progression and persistence
  - release hardening
- For C# changes, keep public serialized fields stable unless a migration plan is clear.
- For Unity asset changes, prefer using Unity Editor when possible. If editing YAML, keep diffs minimal and verify scene/prefab references.
- For user-provided or separately approved art, save source outputs under `Assets/Sprites/PixelArt/` or a clearly named subfolder. Do not overwrite existing sprites until the replacement set is approved.

## PixelLab MCP

PixelLab MCP is disabled for normal agent workflow. Do not call PixelLab tools, image-generation tools, or other artwork-generation tools from this thread.

When artwork is needed, use existing placeholder assets, prepare implementation around replaceable asset references, and write TODOs or prompt drafts if useful. Artwork generation belongs in a separate dedicated workflow. Do not generate sprites, UI assets, icons, backgrounds, or any other artwork as part of ordinary implementation work.

Recommended first asset planning list:

- 6 normal match pieces
- row bomb variants
- column bomb variants
- adjacent bomb variants
- color bomb
- blocker
- collectible
- coin
- normal tile
- breakable tile states
- obstacle tile
- UI buttons and panels
- gameplay background
- App Store draft icon and screenshots later

Treat this list as planning scope, not permission to generate assets. Keep human review in the loop for style taste and asset approval.

## Verification Expectations

Before claiming a gameplay change is done:

- Confirm the project compiles.
- Run available Unity tests if tests exist.
- Manually verify scene boot, start dialog, swap, invalid swap, match clear, cascade, bombs, win, lose, and replay.
- Verify iPhone portrait, iPhone tall/notched, and iPad aspect ratios.
- Check Unity console for errors and warnings that affect play.

Before claiming an art/UI pass is done:

- Verify sprites are imported with pixel-friendly settings.
- Verify transparent backgrounds are truly transparent.
- Verify pieces remain readable at gameplay size.
- Verify UI touch targets are usable on phone screens.
- Verify no text overlaps safe areas or board content.

## Release Mindset

This is not just a prototype. Work toward a complete mobile game:

- polished first-run experience
- clear goals
- readable board
- responsive input
- satisfying feedback
- stable progression
- local save data
- privacy-safe App Store metadata
- no placeholder assets in release builds

## Documentation Maintenance

Future agents must treat project documentation as part of the product. Update Markdown guidance when work changes durable project knowledge.

Update examples:

- New feature added -> update `docs/GAME_DESIGN_TARGET.md` and/or `docs/TECHNICAL_PLAN.md`.
- Major gameplay bug fixed -> update `docs/CODEBASE_OVERVIEW.md` or `docs/TECHNICAL_PLAN.md` if the fix changes known risks or behavior.
- Scene flow, UI navigation, shared prefab, or resource reuse change -> update `docs/SCENE_UI_ARCHITECTURE.md`.
- Parallel work conflict discovered, or ownership boundaries change -> update `docs/OWNERSHIP.md`.
- New art direction or generated asset workflow -> update `docs/PIXEL_ART_DIRECTION.md`.
- App Store, privacy, signing, SDK, analytics, ads, or IAP change -> update `docs/IOS_APPSTORE_PLAN.md`.
- New conventions for agents -> update this `AGENTS.md`.

Do not let docs drift behind the project.
