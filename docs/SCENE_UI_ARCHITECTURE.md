# Scene UI Architecture

## Purpose

This document defines the planned scene flow, UI/UX structure, shared resources, and reuse strategy for the finished mobile game. Future agents should follow this direction unless the user explicitly changes product direction.

The main principle is simple: keep scenes few, keep UI panels reusable, and make levels data-driven.

## Architecture Decision

Do not create a separate Unity scene for every menu, settings screen, end screen, or level. That approach creates duplicated UI, repeated setup, inconsistent transitions, and hard-to-maintain scene references.

Use this structure instead:

```text
Boot Scene
  Persistent managers, save data, audio, settings, scene flow, loading/fade.

Menu Scene
  Home screen, main navigation, player entry point.

Level Select Scene
  Progression map or level grid, locked/unlocked state, level details.

Game Scene
  Reusable gameplay scene. Loads the selected LevelData and ThemeData.

Overlay Panels
  Pause, settings, tutorial, win, lose, rewards, credits/privacy, confirmation dialogs.
```

## Current Implementation Status

This section tracks scene/UI architecture only. For whole-project implemented and missing feature status, use `docs/CODEBASE_OVERVIEW.md`.

Implemented first pass:

- `Assets/Scenes/Boot.unity`, `Menu.unity`, `Level Select.unity`, and `Game.unity` are in build settings.
- `Assets/Scenes/Level 1.unity` remains as a disabled legacy reference scene.
- `SceneBootstrapper` runtime-builds the simple Menu and Level Select screens.
- `SceneFlow` owns selected level id and scene navigation.
- `RuntimeUiShell` runtime-builds reusable start, pause, win, and lose overlays.
- `SafeAreaRoot` constrains menu, level select, and runtime overlay content to `Screen.safeArea`.
- `RuntimeUiShell` runtime-builds a shared settings overlay opened from both Menu and Pause.
- `Assets/Data/Levels/Level_001.asset` is the first tuned data-driven version of the old scene's board setup.
- `Assets/Resources/LevelDatabase.asset` exposes the level list to menu and gameplay flow.

Still planned for scene/UI:

- final shared pixel-art UI prefabs for buttons, panels, level cards, and settings rows
- phone/iPad visual verification and final responsive HUD tuning
- save/progression, locks, stars, and result recording
- additional data-driven levels

Settings, pause, win, lose, and tutorial should be overlay panels, not separate scenes. They need to open over gameplay or menu without destroying the current context.

## Scene Responsibilities

### Boot Scene

Owns persistent app-level systems:

- scene flow
- save data
- player profile
- settings data
- audio service
- haptics service
- asset/theme registry
- loading/fade transition

Boot scene should load once and persist. It should route the player to menu or directly to the last known location if that behavior is added later.

### Menu Scene

Home entry screen. Responsibilities:

- play button
- continue button if progress exists
- settings entry
- credits/privacy entry
- optional shop/events entry later

Menu scene should not contain gameplay state.

### Level Select Scene

Progression screen. Responsibilities:

- show unlocked/completed levels
- show stars or best score
- show level objective preview
- select a level and start game
- return to menu

Start with a simple scrollable level grid. A world map can be added later after the core flow is stable.

### Game Scene

Reusable gameplay scene. Responsibilities:

- load selected `LevelData`
- load selected `ThemeData`
- configure board, moves, goals, pieces, tiles, blockers, collectibles
- show gameplay HUD
- open pause/settings/tutorial/win/lose overlays
- report result to save/progression system

Do not create one gameplay scene per level. Store level differences in data.

### Overlay Panels

Reusable panel types:

- pause panel
- settings panel
- win panel
- lose panel
- tutorial panel
- reward panel
- credits/privacy panel
- confirmation dialog
- loading/fade panel

Overlays should be controlled by a UI navigation stack or panel manager so back/close behavior is consistent.

## UI/UX Flow

Recommended first complete flow:

```text
Launch
  -> Boot
  -> Menu
  -> Level Select
  -> Game
    -> Pause Overlay
      -> Settings Overlay
    -> Win Overlay
      -> Next Level or Level Select
    -> Lose Overlay
      -> Retry or Level Select
```

Settings can be opened from menu or pause. The same settings panel should be reused in both places.

## UI Prefab Library

Build shared prefabs before making many screens:

- `PrimaryButton`
- `SecondaryButton`
- `IconButton`
- `PanelFrame`
- `ModalWindow`
- `CreditsPrivacyPanel`
- `TopBar`
- `CurrencyCounter`
- `LevelCard`
- `ObjectiveItem`
- `ToggleRow`
- `SliderRow`
- `SafeAreaRoot`
- `ScreenFader`
- `LoadingOverlay`

Every screen should compose these shared prefabs instead of creating unique one-off controls.

## HUD Structure

Gameplay HUD should be stable across levels:

- top: level number/name, objective, moves, score
- center: board
- bottom: boosters or future powerups
- edge: pause button and optional settings shortcut

Board readability is the priority. Decorative UI must not compete with pieces.

## Data Assets

Use ScriptableObjects for durable project data.

Recommended data types:

- `LevelData`
- `LevelDatabase`
- `ObjectiveData`
- `PieceSetData`
- `ThemeData`
- `AudioLibrary`
- `BoosterData`
- `TutorialStepData`

### LevelData

Should eventually define:

- level id
- display name
- board width and height
- move limit
- score goal
- objective types and targets
- enabled piece set
- starting tile layout
- starting pieces
- blockers
- breakable tiles
- collectibles
- collectible spawn chance
- collectible max
- theme id
- tutorial steps if any

### ThemeData

Should define reusable visual/audio style:

- gameplay background
- board tile sprites
- UI panel sprites
- button sprites
- normal piece sprites
- special piece sprites
- blocker sprites
- collectible sprites
- coin sprite
- music clips
- UI sounds
- palette notes

ThemeData allows worlds or seasons later without duplicating gameplay code.

## Resource Reuse Strategy

Use one prefab structure with swappable data wherever possible.

Recommended:

- one normal piece prefab pattern, configured by sprite and match value
- one row bomb prefab pattern per match value only if current code requires separate prefabs
- shared UI button prefab with different labels/icons
- shared modal prefab for pause/win/lose variants
- shared background/theme references through ThemeData
- sprite atlases grouped by gameplay, UI, and backgrounds

Avoid:

- duplicate settings screens
- duplicate win/lose scenes
- one scene per level
- one-off buttons with unique styling
- hardcoded asset references spread across many scripts

## Asset Loading

Initial release can use direct ScriptableObject and prefab references. This is simpler and safer.

Consider Unity Addressables later only when:

- asset count grows substantially
- downloadable content is planned
- seasonal/event content needs separation
- memory pressure from always-referenced assets becomes a real problem

Do not add Addressables just because it is available. Add it when content scale demands it.

## Mobile Layout Rules

Phone portrait is the primary layout.

Rules:

- Respect safe areas.
- Keep buttons large enough for touch.
- Keep board centered and readable.
- Do not let modals cover critical buttons at small resolutions.
- Avoid tiny text in HUD.
- Support iPad with adaptive layout, not simple stretching.

iPad options:

- larger board with capped maximum size
- side panels for objective/progress
- wider level-select grid
- more breathing room around menu art

## Visual Integration

All scenes must feel like one game:

- same pixel-art panel style
- same button states
- same icon language
- same transition speed
- same sound/haptic style
- same typography approach
- same palette family per theme

When artwork is needed, use existing placeholders and prepare required asset lists, TODOs, or prompt drafts. Do not call PixelLab or any image-generation tool from ordinary scene/UI implementation work. Artwork generation belongs in a separate dedicated workflow. Do not mix unrelated generated styles.

## Feature Integration Order

Recommended order:

1. (done) Stabilize current `Level 1`.
2. (done) Add Boot scene and persistent managers.
3. (done) Add Menu scene.
4. (done) Add Level Select scene.
5. (done) Convert current gameplay to reusable Game scene.
6. (done) Add overlay panel manager.
7. (done) Add Win/Lose overlays with next/retry/level-select actions.
8. (done) Add LevelData and LevelDatabase.
9. (done) Convert current level into data.
10. (done) Add Settings overlay reused from menu and pause.
11. (remaining) Add save/progression.
12. (remaining) Add pixel-art theme replacement.
13. (remaining) Add boosters, richer objectives, and tutorial.

## Definition Of Done For Scene/UI Work

Scene/UI work is done only when:

- the flow works from boot to menu to level select to game
- back/close/retry/next-level buttons behave predictably
- settings can open from both menu and pause using the same panel
- phone safe area is respected
- iPad layout is acceptable
- UI style is consistent
- no duplicate screen-specific logic is introduced unnecessarily
- docs are updated if flow, data, resources, or UI conventions change
