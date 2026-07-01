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

## Current Project Snapshot

- Unity version: `6000.5.0f1`.
- Main scene: `Assets/Scenes/Level 1.unity`.
- Core gameplay scripts: `Assets/Scripts/Board.cs`, `GamePiece.cs`, `Tile.cs`, `Bomb.cs`, `Collectibles.cs`.
- Game flow scripts: `GameManager.cs`, `ScoreManager.cs`, `SoundManager.cs`, `MessageWindow.cs`, `ScreenFader.cs`, `RectXformMove.cs`.
- Existing gameplay: board fill, swap, match detection, cascades, score, move limit, row bombs, column bombs, adjacent bombs, color bombs, breakable tiles, obstacles, collectibles, basic particles, basic sounds, start/win/lose dialogs.
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

## First Fix Priorities

### Recently fixed foundation bugs

1. Board setup is guarded so `SetupBoard()` initializes a level only once.
2. `Singleton<T>.Awake()` destroys duplicate objects instead of destroying the existing singleton instance.
3. Board input now uses a Board-owned mouse/touch screen-to-grid path instead of `Tile.OnMouse*` callbacks.

### Next initiatives - see `docs/TECHNICAL_PLAN.md` phases 2-5

1. Scene flow and reusable UI shell.
2. Data-driven level model.
3. Save/progression plus shared resource/theme structure.
4. Pixel-art asset pipeline and replacement pass.

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
- For generated art, save source outputs under `Assets/Sprites/PixelArt/` or a clearly named subfolder. Do not overwrite existing sprites until the replacement set is approved.

## PixelLab MCP

PixelLab MCP is available for pixel-art generation when the `mcp__pixellab` tools are exposed in the session.

Recommended first asset generation pass:

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

Use PixelLab for fast consistent drafts, then let the user select the strongest direction. Keep human review in the loop for style taste.

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
- New art direction or generated asset workflow -> update `docs/PIXEL_ART_DIRECTION.md`.
- App Store, privacy, signing, SDK, analytics, ads, or IAP change -> update `docs/IOS_APPSTORE_PLAN.md`.
- New conventions for agents -> update this `AGENTS.md`.

Do not let docs drift behind the project.
