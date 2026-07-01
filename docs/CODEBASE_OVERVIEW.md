# Codebase Overview

## Project Type

Unity 2D match-3 puzzle game. The project now has a small menu-to-game scene flow around the original compact prototype gameplay.

## Important Paths

- `Assets/Scenes/Boot.unity` - first enabled scene; routes into the menu.
- `Assets/Scenes/Menu.unity` - runtime-built home screen.
- `Assets/Scenes/Level Select.unity` - runtime-built level grid.
- `Assets/Scenes/Game.unity` - reusable gameplay scene driven by `LevelData`.
- `Assets/Scenes/Level 1.unity` - legacy reference gameplay scene, disabled in build settings.
- `Assets/Scripts/` - all custom C# gameplay and UI flow scripts.
- `Assets/Scripts/Flow/` - scene names, scene navigation, and scene bootstrap UI.
- `Assets/Scripts/UI/` - reusable runtime UI shell and UI element factory.
- `Assets/Scripts/LevelData.cs`, `LevelDatabase.cs`, `LevelLoader.cs` - first ScriptableObject level-data foundation.
- `Assets/Data/Levels/Level_001.asset` - first data-driven copy of the old level setup.
- `Assets/Resources/LevelDatabase.asset` - runtime-loaded list of playable levels.
- `Assets/Prefabs/UI/RuntimeUiShell.prefab` - reusable shell prefab marker for shared modal/pause UI.
- `Assets/Prefabs/Dots/` - normal match-piece prefabs.
- `Assets/Prefabs/Bombs/` - row, column, adjacent, and color bomb prefabs.
- `Assets/Prefabs/Tiles/` - normal, breakable, double-breakable, obstacle tile prefabs.
- `Assets/Prefabs/Collectibles/` - collectible and blocker prefabs.
- `Assets/Sprites/GameAssets/` - current piece and bomb sprites.
- `Assets/Sprites/PixelArt/` - generated/test pixel-art assets.
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

Important risks:

- Calls `m_board.SetupBoard()` after the start prompt; board setup is now guarded so repeated calls return without duplicating contents.
- `ApplyLevelData()` can configure the move limit, score goal, and display name before the game loop starts.
- Runtime `RuntimeUiShell` now handles start, pause, win, and lose overlays in the reusable `Game` scene flow.
- Runtime UI disables the legacy scene `MessageWindow` to avoid duplicate goal/end popups.
- Lose is decided only after the board has finished refilling, so final-move cascades can still satisfy the score goal and win.
- Uses string-based coroutine calls.
- Retry reloads the reusable `Game` scene through `SceneFlow`; win can advance to the next level if the database has one, otherwise it returns to level select.

### `LevelData.cs`, `LevelDatabase.cs`, `LevelLoader.cs`

First pass of the level-data model. Responsibilities include:

- storing level id, display name, board size, move limit, score goal, objective type, prefab references, starting tiles, starting pieces, and collectible spawn settings
- grouping levels in a simple `LevelDatabase`
- applying one selected `LevelData` to the current `Board` and `GameManager` before board setup

Important risks:

- `Assets/Scenes/Game.unity` has a `LevelLoader` assigned to `Level_001`. `LevelLoader` also prefers the level selected through `SceneFlow`.
- Objective type and target count are stored but not yet resolved by game rules beyond the existing score goal.

### `SceneFlow.cs`, `SceneBootstrapper.cs`, `RuntimeUiShell.cs`

First pass of the reusable scene/UI foundation. Responsibilities include:

- loading Menu, Level Select, and Game scenes
- storing the selected level id for the current run
- loading `LevelDatabase` from `Resources`
- runtime-building the current simple menu and level-select UI
- runtime-building reusable modal and pause overlays for gameplay

Important risks:

- Current screens are functional runtime UI, not final pixel-art prefabs.
- Safe-area layout and final iPad/phone polish still need a dedicated pass.
- Settings, save/progression, stars, locks, and level results are not implemented yet.

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

## Existing Gameplay Features

- Normal swap match-3 interaction.
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

## Missing Product Features

- Additional level data assets beyond `Level_001`.
- Tutorial.
- Settings menu.
- Save data.
- Mobile safe-area handling.
- Touch-first input system.
- Pixel-art replacement pass.
- App icon and launch screen.
- App Store metadata and screenshots.
- Release QA checklist.
