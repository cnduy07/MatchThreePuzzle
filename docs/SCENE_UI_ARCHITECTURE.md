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
- `Assets/Data/Levels/Level_001.asset` through `Level_005.asset` are registered in `LevelDatabase`.
- `PieceSetData`, `ThemeData`, and `GameResourceLibrary` provide the first shared resource/theme layer for current pieces, tiles, UI colors, and future UI prefab slots.
- `Assets/Prefabs/UI/Shared/` contains placeholder shared UI prefabs already referenced by `ClassicTheme`.
- `Assets/Concept/` contains menu, level-map, and gameplay visual references; gameplay includes a monster-door threat scene above the board.
- `PlayerProgress` stores local progression and settings data through `PlayerPrefs`.
- Level Select shows locked/unlocked state, completion stars, and best score.
- Winning a level records the result and unlocks the next registered level.
- Settings persist audio and haptics toggle state locally.
- Menu, Level Select, and Game runtime UI now use concept-like placeholder layouts based on `Assets/Concept/`.
- Game HUD includes placeholder top counters/objective, monster-door threat area, score, boosters, and pause control.
- Runtime concept HUD owns gameplay counters while legacy scene HUD panels, text, and old score decoration are hidden to avoid duplicate UI.
- Game board camera framing reserves more top presentation space so the board sits lower under the monster-door concept area.
- Placeholder UI uses small runtime motion effects for hierarchy and feedback.

Still planned for scene/UI:

- final pixel-art replacement for shared UI prefab sprites and sliced panels
- final concept-accurate sprite replacement for menu, level-map, gameplay HUD, resource counters, boosters, and bottom navigation
- phone/iPad visual verification and final responsive HUD tuning
- additional data-driven levels beyond the first five

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
- present levels as a concept-style map path with themed region placeholders

The current implementation uses a placeholder world-map layout rather than the earlier simple grid. Final map art and node sprites still need approved assets.

### Game Scene

Reusable gameplay scene. Responsibilities:

- load selected `LevelData`
- load selected `ThemeData`
- configure board, moves, goals, pieces, tiles, blockers, collectibles
- show gameplay HUD
- show concept-driven threat presentation above the board
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

Defines reusable visual/audio style. The first implementation exists and currently includes:

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
- UI prefab slots for primary, secondary, icon button, panel frame, modal, top bar, currency counter, level card, objective item, toggle row, slider row, and loading overlay

ThemeData allows worlds or seasons later without duplicating gameplay code.

## Resource Reuse Strategy

Use one prefab structure with swappable data wherever possible.

Recommended:

- one normal piece prefab pattern, configured by sprite and match value
- one row bomb prefab pattern per match value only if current code requires separate prefabs
- shared UI button prefab with different labels/icons through semantic `RuntimeButtonStyle`
- shared modal prefab for pause/win/lose variants through semantic `RuntimePanelStyle`
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
11. (done) Add save/progression.
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
