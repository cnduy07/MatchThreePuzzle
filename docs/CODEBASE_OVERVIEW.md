# Codebase Overview

## Project Type

Unity 2D match-3 puzzle game. The project now has a small menu-to-game scene flow around the original compact prototype gameplay.

## How To Read Progress

Use this file for the current state of the project:

- `Implemented Features` lists behavior and systems that exist in the project now.
- `Missing / Not Release-Ready Features` lists work that is absent, incomplete, placeholder-only, or still needs target-device verification.
- Script sections document how the implemented systems work and what risks future agents should keep in mind.

Use `docs/TECHNICAL_PLAN.md` for the roadmap and next engineering sequence. Use `docs/SCENE_UI_ARCHITECTURE.md` for scene/UI structure and overlay conventions.

## Important Paths

- `Assets/Scenes/Boot.unity` - first enabled scene; routes into the menu.
- `Assets/Scenes/Menu.unity` - runtime-built home screen.
- `Assets/Scenes/Level Select.unity` - runtime-built level grid.
- `Assets/Scenes/Game.unity` - reusable gameplay scene driven by `LevelData`.
- `Assets/Scenes/Level 1.unity` - legacy reference gameplay scene, disabled in build settings.
- `Assets/Concept/` - visual/UX references for menu, level map, and gameplay scene direction.
- `Assets/Scripts/` - all custom C# gameplay and UI flow scripts.
- `Assets/Scripts/Flow/` - scene names, scene navigation, scene bootstrap routing, and menu/level-map UI builders.
- `Assets/Scripts/UI/` - reusable runtime UI shell, safe-area root, runtime settings state, and UI element factory.
- `Assets/Scripts/Flow/MenuSceneUiBuilder.cs` - runtime-built Menu screen layout.
- `Assets/Scripts/Flow/LevelMapSceneUiBuilder.cs` - runtime-built Level Select map layout.
- `Assets/Scripts/UI/GameSceneHudBuilder.cs` - runtime-built gameplay HUD, threat preview, and booster bar layout.
- `Assets/Scripts/Editor/Phase45UiPrefabBuilder.cs` - editor-only helper for building placeholder shared UI prefabs.
- `Assets/Scripts/ParticleManager.cs`, `ParticlePlayer.cs` - particle effect helpers used by board clears, tile breaks, and bomb feedback.
- `Assets/Scripts/PlayerProgress.cs` - local `PlayerPrefs` JSON save data for progression, best scores, stars, and settings.
- `Assets/Scripts/LevelData.cs`, `LevelDatabase.cs`, `LevelLoader.cs` - first ScriptableObject level-data foundation.
- `Assets/Scripts/ThemeData.cs`, `PieceSetData.cs`, `GameResourceLibrary.cs` - shared theme, piece-set, and resource registry foundation.
- `Assets/Data/Levels/Level_001.asset` through `Level_005.asset` - first data-driven playable level set.
- `Assets/Data/PieceSets/ClassicPieceSet.asset` - shared default references for current normal pieces, bombs, collectibles, and blockers.
- `Assets/Data/Themes/ClassicTheme.asset` - shared default placeholder theme for tile, gameplay background, UI color, UI prefab, sprite, gameplay HUD sprite, and palette references.
- `Assets/Sprites/menu_*_btn.png` - user-provided pixel-art menu action button cutouts wired through `ClassicTheme`.
- `Assets/Resources/GameResourceLibrary.asset` - runtime-loaded default theme and piece-set registry.
- `Assets/Resources/LevelDatabase.asset` - runtime-loaded list of playable levels used by level select, loading, and next-level flow.
- `Assets/Prefabs/UI/RuntimeUiShell.prefab` - reusable shell prefab marker for shared modal/pause UI.
- `Assets/Prefabs/UI/SafeAreaRoot.prefab` - shared safe-area root prefab.
- `Assets/Prefabs/UI/Shared/` - generated placeholder shared UI prefab library for buttons, panels, level cards, rows, counters, and loading overlay.
- `Assets/Prefabs/UI/Shared/LoseThreatStage.prefab` - placeholder shared stage prefab for the lose-threat presentation.
- `Assets/Prefabs/Dots/` - normal match-piece prefabs.
- `Assets/Prefabs/Bombs/` - row, column, adjacent, and color bomb prefabs.
- `Assets/Prefabs/Tiles/` - normal, breakable, double-breakable, obstacle tile prefabs.
- `Assets/Prefabs/Collectibles/` - collectible and blocker prefabs.
- `Assets/Sprites/GameAssets/` - current piece and bomb sprites.
- `Assets/Sprites/PixelArt/` - generated/test pixel-art assets.
- `Assets/Sprites/PixelArt/Generated/` - generated gameplay sprite candidates wired into the current dot, bomb, collectible, blocker, breakable/obstacle tile, booster, and pause HUD references for review.
- `Assets/Particles/Prefab/` - existing clear/break/bomb visual effects.
- `Assets/Sounds/` - music, win/lose sounds, and FX.
- `Packages/manifest.json` - Unity packages.
- `ProjectSettings/` - Unity project settings.

## Current Unity Packages

The project already has the main packages needed for a 2D pixel-art mobile puzzle game:

- Unity UI / UGUI
- TextMesh Pro
- 2D Sprite
- 2D Pixel Perfect
- 2D PSD Importer
- 2D Tilemap
- Unity Test Framework

Avoid adding third-party runtime dependencies unless there is a clear need.

## Core Gameplay Scripts

### `Board.cs`

Main board controller. Responsibilities include:

- board dimensions and camera setup
- tile creation
- starting piece placement
- random piece fill
- collectible spawning
- swap handling
- match finding
- cascade/refill loop
- bomb creation
- bomb activation
- tile breaking
- collectible clearing
- score triggering
- particle triggering

Important existing behavior:

- `width`, `height`, and `borderSize` are serialized.
- The current scene uses a 7x9 board.
- `gamePiecePrefabs` contains normal match pieces.
- `rowBombPrefabs`, `columnBombPrefabs`, `adjacentBombPrefabs`, and `colorBombPrefab` define special pieces.
- `collectiblePrefabs`, `collectibleMax`, and `changeForCollectible` control collectible spawning.
- Matches are detected by comparing `GamePiece.matchValue`.
- Bombs are `GamePiece` objects with an extra `Bomb` component.
- `SetupBoard()` is guarded so accidental repeated calls do not duplicate board contents.
- Board-owned mouse/touch input maps screen position to board grid coordinates and calls `ClickedTile`, `DragToTile`, and `ReleaseTile`.
- `ApplyLevelData()` can configure board dimensions, piece/tile prefab references, starting layout, and collectible settings before setup.
- `SettupCamera()` reserves top/bottom world-space room for HUD/home-area UI, uses `Screen.safeArea` aspect ratio, and keeps smaller boards from exceeding a 9-world-unit fit height baseline.

Important risks:

- Many methods assume board arrays and cells are non-null.
- The class is large and mixes board rules, spawning, scoring, particles, and level concerns.
- `changeForCollectible` appears to be a code typo for collectible spawn chance. Do not rename it casually; fix the field name together with the future `LevelData` collectible spawn chance model to avoid serialized-data mismatches.
- Input ignores only interactable Unity UI `Selectable` controls on pointer begin. Decorative raycast targets such as transparent faders and background images should not block board swipes.

### `GamePiece.cs`

Base component for pieces, bombs, and collectibles. Responsibilities include:

- board coordinates
- movement coroutine
- interpolation style
- score value
- match value
- clear sound
- color-copy behavior for generated bombs

Important risks:

- Movement state blocks new moves while `isMoving` is true.
- `MoveRoutine` updates board placement only after reaching destination.
- `AddScore` also plays clear audio, so score and audio concerns are coupled.

### `Tile.cs`

Board tile component. Responsibilities include:

- tile coordinates
- tile type: normal, obstacle, breakable
- breakable sprite state
- tile initialization from `Board`

Important risks:

- Tile no longer owns pointer input; `Board` maps mouse and touch positions to board grid coordinates for selection.
- `BreakTileRoutine` uses `breakableValue` as an array index. Guard sprite bounds when changing breakable states.

### `Bomb.cs`

Special piece subtype with `BombType`:

- none
- row
- column
- adjacent
- color

Bomb effect resolution lives in `Board.cs`.

### `Collectibles.cs`

Special `GamePiece` subtype for collectibles. It sets `matchValue` to `None` and supports:

- `clearedByBomb`
- `clearedAtBottom`

## Game Flow Scripts

### `GameManager.cs`

Controls the loop:

1. start dialog
2. play until score goal or no moves
3. end dialog
4. replay current scene

Important serialized fields:

- `movesLeft`
- `scoreGoal`
- `screenFader`
- `levelNameText`
- `movesLeftText`
- `messageWindow`
- `goalIcon`
- `winIcon`
- `loseIcon`

The old serialized UI references (`screenFader`, `levelNameText`, `movesLeftText`, `messageWindow`, `goalIcon`, `winIcon`, and `loseIcon`) are legacy scene wiring from `Assets/Scenes/Level 1.unity`. In the main build flow (`Boot` -> `Menu` -> `Level Select` -> `Game`), `RuntimeUiShell` handles start, pause, win, and lose overlays, and the copied `Game.unity` keeps the legacy `MessageWindow` disabled to avoid duplicate UI.

Important risks:

- Calls `m_board.SetupBoard()` after the start prompt; board setup is now guarded so repeated calls return without duplicating contents.
- `ApplyLevelData()` can configure the move limit, score goal, and display name before the game loop starts.
- Runtime `RuntimeUiShell` now handles start, pause, win, and lose overlays in the reusable `Game` scene flow.
- Runtime UI disables the legacy scene `MessageWindow` to avoid duplicate goal/end popups.
- Lose is decided only after the board has finished refilling, so final-move cascades can still satisfy the score goal and win.
- Uses string-based coroutine calls.
- Retry reloads the reusable `Game` scene through `SceneFlow`; win can advance to the next level if the database has one, otherwise it returns to level select.

### `PlayerProgress.cs`

First-pass local save/progression service. Responsibilities include:

- loading and saving one JSON blob in `PlayerPrefs`
- tracking highest unlocked level
- tracking completed levels
- tracking best score and best stars per level
- tracking local coin count placeholder
- storing audio and haptics settings
- calculating simple 1-3 star results for score-goal wins

Important risks:

- Save data is local only; no cloud save or migration/versioning beyond the current key.
- Stars are simple score/moves heuristics and need design tuning after level balancing.
- Haptics setting is persisted, but there is still no real haptics service.

### `LevelData.cs`, `LevelDatabase.cs`, `LevelLoader.cs`

First pass of the level-data model. Responsibilities include:

- storing level id, display name, board size, move limit, score goal, objective type, prefab references, starting tiles, starting pieces, and collectible spawn settings
- grouping levels in a simple `LevelDatabase`
- applying one selected `LevelData` to the current `Board` and `GameManager` before board setup
- finding the next level in database order for progression unlocks and next-level flow

Important risks:

- `Assets/Scenes/Game.unity` has a `LevelLoader` assigned to `Level_001`. `LevelLoader` also prefers the level selected through `SceneFlow`.
- Levels 1-5 currently use score goals and matching score objective targets. Keep those values aligned until score objectives are resolved through a dedicated objective system.
- Levels can now reference `ThemeData` and `PieceSetData`. Direct prefab arrays on `LevelData` still act as level-specific overrides and are intentionally preserved for compatibility.
- Objective type and target count are stored but not yet resolved by game rules beyond the existing score goal.

### `ThemeData.cs`, `PieceSetData.cs`, `GameResourceLibrary.cs`

First pass of the shared resource/theme layer. Responsibilities include:

- grouping normal pieces, bomb variants, collectibles, and blockers into reusable `PieceSetData`
- grouping default piece set, tile references, UI sprites, UI prefab slots, UI colors, background, music, and palette notes into `ThemeData`
- loading a default resource registry from `Assets/Resources/GameResourceLibrary.asset`
- letting `Board.ApplyLevelData()` resolve missing level-specific prefab references through the selected theme/piece set
- letting runtime menu/overlay UI use semantic button and panel styles that can later resolve to shared prefab art

Important risks:

- Current shared assets still point at placeholder/prototype prefabs and colors. They are a reuse foundation, not final art.
- The main Menu action buttons now use the first user-provided pixel-art cutouts through menu-specific `ThemeData` sprite slots. Other screen/UI art is still placeholder.
- Gameplay HUD booster and pause controls now have theme-specific sprite slots; `ClassicTheme` points them at the first generated icon/button candidates.
- `ClassicTheme.gameplayBackground` points at the current `sky_night.png` gameplay background, which the runtime shell displays as a world-space sprite behind the board.
- Existing levels retain direct prefab overrides, so later cleanup can remove duplicated arrays level by level after validation.
- Shared UI prefab slots are wired to placeholder prefabs under `Assets/Prefabs/UI/Shared/`; replace their art/sprites during the final pixel-art UI pass.

### `SceneFlow.cs`, `SceneBootstrapper.cs`, `MenuSceneUiBuilder.cs`, `LevelMapSceneUiBuilder.cs`, `RuntimeUiShell.cs`, `GameSceneHudBuilder.cs`

First pass of the reusable scene/UI foundation. Responsibilities include:

- loading Menu, Level Select, and Game scenes
- storing the selected level id for the current run
- loading `LevelDatabase` from `Resources`
- routing scene startup from `SceneBootstrapper` to screen-specific builders
- runtime-building the current simple menu and level-select UI through separate menu and level-map builder files
- showing locked/unlocked level-select state from `PlayerProgress`
- showing completed stars and best score on level buttons
- runtime-building the gameplay HUD, threat preview, and booster bar through `GameSceneHudBuilder`
- runtime-building reusable modal and pause overlays for gameplay
- runtime-building a shared settings overlay opened from both Menu and Pause
- applying `SafeAreaRoot` to menu, level select, pause button, modal, pause, and settings overlay content

Important risks:

- Current screens are functional runtime UI, not final pixel-art prefabs.
- Final iPad/phone polish still needs device/aspect-ratio verification.
- Save/progression is first-pass local data only, not cloud-backed.
- Level-select layout is still runtime placeholder UI and needs final visual treatment.
- Menu, level-map, and game-HUD runtime layout now live in separate builder files so parallel scene-focused sessions can work with fewer file conflicts.

Related helpers:

- `SceneNames.cs` centralizes the scene-name constants used by `SceneFlow` and scene bootstrapping.
- `RuntimeUiFactory.cs` builds theme-aware runtime UI elements and falls back to generated controls when a shared prefab slot is missing.
- `RuntimeSettingsState.cs` carries transient settings overlay state between runtime UI panels.
- `SafeAreaRoot.cs` applies `Screen.safeArea` padding to runtime and shared UI roots.

### `ScoreManager.cs`

Tracks current score and animates score text upward.

Important risk:

- `ScoreManager` is intentionally scene-local even though it inherits from `Singleton<T>`, so score and HUD references reset when the reusable `Game` scene reloads.
- Multiple score coroutines can overlap when cascades score quickly.

### `SoundManager.cs`

Plays random music, win, lose, and bonus clips.

Important risks:

- Music is played through `PlayClipAtPoint`, so it is not looped or managed as persistent music.
- Frequent FX create temporary GameObjects. Profile before release.

### `MessageWindow.cs`

Updates the modal message UI used by the game loop. Responsibilities include:

- assigning the message icon sprite
- assigning the message body text
- assigning the message button text
- requiring `RectXformMove` so the same UI object can animate on and off screen

Important risks:

- Uses legacy `UnityEngine.UI.Text` fields, so future UI work may need migration or consistent TextMesh Pro usage.
- `buttonText` is not null-checked before assignment when `buttonMsg` is non-null.
- It only updates display fields; button actions and panel movement are owned elsewhere, so keep event wiring clear when adding reusable overlays.

### `ScreenFader.cs`

Controls fade transitions on a `MaskableGraphic`. Responsibilities include:

- storing solid and clear alpha values
- applying an optional fade delay
- fading on to a solid overlay
- fading off to a clear overlay

Important risks:

- `m_graphic` is assigned in `Start()`, so calling `FadeOn()` or `FadeOff()` before `Start()` can fail.
- The fade uses `CrossFadeAlpha`; ensure the target graphic is configured correctly for raycasts and initial alpha.
- This is a simple screen fade helper, not a full scene loading or transition manager.

### `RectXformMove.cs`

Animates a UI `RectTransform` between configured anchored positions. Responsibilities include:

- storing start, onscreen, and end positions
- moving a panel on screen with `MoveOn()`
- moving a panel off screen with `MoveOff()`
- smoothing panel movement with a smoother-step interpolation

Important risks:

- The private `Move` method ignores its `timeTomove` parameter and uses the serialized `timeToMove` field.
- Movement is blocked while `m_isMoving` is true, so repeated show/hide commands during animation can be ignored.
- Positions are manually configured vectors, which may not adapt well to all safe-area and aspect-ratio layouts without a more robust UI panel system.

### `Singleton.cs`

Generic persistent singleton.

Important behavior:

- The first instance is retained.
- Singletons persist across scene loads by default, but subclasses can opt out through `ShouldPersistAcrossScenes`.
- `GameManager` and `ScoreManager` are scene-local because gameplay scene reloads must reset flow state, score, and scene references.
- Later duplicates destroy their own GameObject.

Important risk:

- The static instance can still hold stale references if a persistent singleton is destroyed outside the normal duplicate path.

## Implemented Features

Scene and level flow:

- Boot -> Menu -> Level Select -> reusable Game scene flow.
- Runtime-built first-pass Menu and Level Select screens.
- First-pass `LevelData`, `LevelDatabase`, and `LevelLoader` pipeline.
- `Level_001` through `Level_005` are the first playable data-driven level set.
- Retry, level select, and next-level navigation through `SceneFlow`.
- Local progression save unlocks the next level on win.
- Level Select shows locked/unlocked state, stars, and best score.

Gameplay:

- Normal swap match-3 interaction.
- Board-owned mouse/touch swipe input.
- Invalid swaps move back.
- Cascading matches.
- Move counter.
- Score target.
- Win and lose states.
- Row and column bombs from 4 matches.
- Adjacent bomb or color bomb from larger/corner matches.
- Breakable tiles.
- Obstacle tiles.
- Collectibles that can clear at the bottom.
- Basic particles and sounds.

Runtime UI:

- Runtime start, pause, win, and lose overlays in the reusable Game scene.
- Shared settings overlay opened from both Menu and Pause.
- Scene-specific runtime UI layout is split across `MenuSceneUiBuilder`, `LevelMapSceneUiBuilder`, and `GameSceneHudBuilder` to support parallel screen-focused work.
- Runtime menu and overlay controls now resolve semantic button/panel styles through `ThemeData`, using shared placeholder prefabs with generated-control fallback.
- Menu runtime UI now follows the `Assets/Concept/menuscene.png` structure with placeholder resource counters, large logo, stacked action buttons, settings button, progress sign, and bottom navigation.
- Menu action buttons can use theme-assigned pixel-art sprites for Play, Levels, Daily Reward, Events, and Shop.
- Game runtime UI uses a themed world-space gameplay background behind the board; HUD and overlays remain on the safe-area canvas.
- Level Select runtime UI now follows the `Assets/Concept/levelmapscene.png` structure with placeholder resource counters, region labels, path segments, level nodes, locked worlds, and bottom navigation.
- Game runtime UI now follows the `Assets/Concept/gamescene.png` structure with placeholder level/goal/moves panels, monster-door threat area, score, bottom boosters, and pause control.
- Game booster and pause controls now use theme-assigned sprite art with text fallbacks when sprites are missing.
- No-moves loss now plays a concept-inspired monster rush/door attack animation before the lose modal.
- Legacy scene HUD panels/text and old score decoration are hidden when the runtime concept HUD is active, preventing duplicate old/new UI.
- Major placeholder UI elements use lightweight runtime motion for slide-in, pulse, and floating feedback.
- Persistent audio toggle through `AudioListener.volume`.
- Persistent haptics toggle stub for a future haptics service.
- First-pass safe-area-aware layout root for menu, level select, HUD controls, and overlays.
- First-pass camera/board fit that uses safe-area aspect ratio and reserves extra top room so the board sits lower under the concept HUD/threat area.
- Generated gameplay sprites have first-pass render ordering: visible clean board tile backing on the board layer, piece roots above tiles, special-piece markers above their base piece, and world background behind all board sprites.

## Missing / Not Release-Ready Features

- More level data beyond the first five levels, with real tuning and varied layouts.
- Final shared pixel-art UI prefabs replacing runtime placeholder controls.
- Final phone/iPad visual verification and responsive HUD tuning.
- Tutorial.
- Real haptics service integration.
- Additional objective rules beyond score-goal flow.
- Final art-filled UI prefab pass, sprite atlas grouping, and art-filled theme assets.
- Final concept-accurate sprite/art replacement for menu, level map, gameplay HUD, boosters, and resource counters.
- Pixel-art replacement pass.
- App icon and launch screen.
- App Store metadata and screenshots.
- Release QA checklist.
