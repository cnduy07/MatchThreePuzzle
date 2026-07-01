# Technical Plan

## Guiding Approach

Finish the game by stabilizing the existing prototype first, then add product systems around it. Avoid rewriting the whole board unless a specific bug or feature proves the current design cannot support release.

## Phase 1: Stabilize Current Gameplay

Tasks:

- Keep the fixed single board initialization behavior from regressing.
- Keep the fixed singleton duplicate behavior from regressing.
- Validate all existing prefabs have required components.
- Add guardrails around null board cells.
- Verify normal matches, invalid swaps, cascades, bombs, blockers, collectibles, win, lose, and replay.
- Verify Board-owned mouse/touch grid-position input on iPhone, iPad, and desktop.

Expected result:

- Current level plays reliably on desktop and mobile input.
- No duplicate board pieces on start.
- No obvious null reference errors during cascades.

## Phase 2: Level Data

Current status:

- Initial `LevelData`, `LevelDatabase`, and `LevelLoader` scripts exist.
- `Board.ApplyLevelData()` can apply dimensions, prefab references, starting objects, and collectible settings before setup.
- `GameManager.ApplyLevelData()` can apply move limit, score goal, and display name before the game loop starts.
- `Assets/Data/Levels/Level_001.asset` mirrors the old `Level 1` board setup.
- `Assets/Resources/LevelDatabase.asset` contains the first playable level.
- `Assets/Scenes/Game.unity` is wired with `LevelLoader` and can prefer the selected level from `SceneFlow`.

Phase 2 is the level data model only. It should define the data structures that describe playable levels, then plug into the scene skeleton established in Phase 3. Do not duplicate Phase 3 scene-flow work here.

Add a level data layer. Recommended Unity-friendly approach:

- `ScriptableObject` level definitions.
- A level loader that configures `Board` and `GameManager` from level data.
- A simple list/database of levels for progression.

Level data should cover:

- board dimensions
- move count
- score goal
- enabled piece prefabs
- tile layout
- starting pieces
- starting blockers
- starting collectibles
- collectible spawn settings
- objective type
- objective target count

Keep the scene reusable. Do not create one scene per level unless there is a strong reason.

## Phase 3: Scene Flow And Mobile UI Shell

Build the complete user flow:

- boot scene
- home screen
- level select
- reusable gameplay scene
- gameplay HUD
- pause overlay
- reusable settings overlay
- win overlay
- lose overlay
- replay
- next level

Current status:

- First-pass `Boot`, `Menu`, `Level Select`, and reusable `Game` scenes exist.
- `SceneBootstrapper` builds simple menu and level-select screens at runtime.
- `RuntimeUiShell` handles gameplay start, pause, win, lose, retry, next, and level-select overlays.
- The legacy `Level 1` scene remains disabled in build settings as a reference.
- `GameManager` and `ScoreManager` are scene-local singleton subclasses so retry and level reloads create fresh gameplay state.

Technical requirements:

- safe-area aware layout
- phone portrait first
- iPad tested
- scalable UI anchors
- readable text
- touch targets large enough for mobile
- shared UI prefabs instead of one-off screen controls
- shared transitions and loading/fade behavior

Remaining Phase 3 work:

- safe-area-aware reusable roots
- final shared UI prefabs instead of runtime-only placeholder controls
- settings overlay reused from menu and pause
- phone and iPad visual verification

## Camera And Board Fit Open Item

Current code has only a basic board-fit implementation in `Board.SettupCamera()`: it centers the camera on the board, computes vertical and horizontal orthographic sizes from `width`, `height`, `borderSize`, and screen aspect ratio, then chooses the larger size. This fits the board in view, but it does not yet account for gameplay HUD, safe areas, bottom boosters, notches, or iPad max-size capping.

Target approach to design during Phase 3:

- Use safe-area-driven scaling, not letterboxing, for normal iPhone and iPad play.
- Calculate orthographic size from board world bounds plus reserved top/bottom UI world-space margins.
- Keep the full board visible on small, notched, large iPhone, and iPad layouts.
- Cap iPad board display size so it does not become comically large; use a starting cap of 9 world units tall for the board area, then tune after device screenshots.
- Keep the board centered in the available safe gameplay area, not necessarily the full physical screen.

This is an open implementation item. Do not claim the game has final responsive board fitting until this camera/layout system is implemented and verified on target aspect ratios.

## Phase 4: Save Data

Start simple:

- unlocked level
- completed levels
- best score per level
- coin count
- audio/haptics settings

Use local save data first. Do not add cloud save until the base product is stable.

## Phase 4.5: Resource And Theme Reuse

Add reusable data and prefab structure before generating many levels or art variants:

- `ThemeData` for background, tiles, pieces, UI sprites, music, and palette notes.
- `PieceSetData` for normal pieces and special piece variants.
- shared UI prefabs for buttons, panels, level cards, objective rows, toggles, and modals.
- sprite atlas grouping for gameplay, UI, and background assets.

Use direct Unity references first. Consider Addressables only when asset count, downloadable content, or memory pressure makes it useful.

## Phase 5: Pixel-Art Asset Replacement

Replace assets in controlled groups:

1. normal pieces
2. special pieces
3. tiles and blockers
4. collectibles and coin
5. HUD icons
6. panels and buttons
7. background
8. app icon and store draft art

Do not mix old and new art in a final review build. Temporary mixed art is acceptable only during implementation.

## Phase 6: Polish

Polish areas:

- swap animation timing
- cascade timing
- score feedback
- special piece creation feedback
- bomb activation feedback
- particle style
- audio and haptics
- end-screen transitions
- tutorial clarity

Keep effects readable and performant.

## Phase 7: Tests

Recommended edit-mode tests:

- match detection horizontal and vertical
- invalid swap rollback
- match 4 creates correct special piece
- match 5 creates color bomb
- adjacent bomb clears correct area
- row bomb clears row
- column bomb clears column
- collectibles clear at bottom
- breakable tiles decrement state
- level data loads expected board

Recommended play-mode smoke tests:

- scene boots without errors
- start dialog works
- valid swap consumes move
- invalid swap does not consume move
- win flow appears
- lose flow appears
- replay reloads cleanly

## Phase 8: Performance

Profile on target-like hardware before release.

Watch for:

- allocations during cascades
- repeated temporary audio GameObjects
- particle spikes
- overdraw from UI/background
- large uncompressed textures
- slow scene reloads

Potential optimizations:

- pool particles
- pool audio sources
- use sprite atlases
- compress backgrounds separately from gameplay pieces
- cap particle counts
- avoid unnecessary `FindObjectOfType` during play

## Known Technical Risks

- `Board.cs` is doing too much. Keep early fixes small, but consider splitting later into board model, match resolver, level loader, and board view.
- Current input is Board-owned screen-to-grid mapping with selective UI blocking for interactable `Selectable` controls; validate drag behavior, edge-cell selection, and future overlay controls as panels are added.
- LevelData stores objective type and target count before all objective rules exist, so avoid treating non-score objectives as complete until the resolver/game rules are added.
- Scene and prefab references can break if YAML is edited carelessly.
- Existing sprite import settings are mixed.
- Current sound manager does not manage looping music robustly.
- Score counting coroutine can overlap.
- App Store readiness is not just a build setting; privacy and metadata must match actual SDK usage.
- Scene flow can become messy if settings, end screens, and tutorials are implemented as separate one-off scenes instead of reusable overlays.
- Asset duplication can grow quickly if levels, themes, buttons, and pieces do not use shared data/prefab patterns.

## Definition Of Done For A Feature

A feature is done only when:

- it works in the gameplay scene
- it handles normal failure paths
- it does not regress existing match/cascade behavior
- it is usable on phone-sized screens
- it has tests or a documented manual verification path
- Unity console does not show relevant errors
- docs are updated if it changes design, art pipeline, or release assumptions

## Documentation Updates

Every meaningful implementation pass should consider documentation impact.

Update Markdown files when:

- a new feature changes player-facing behavior
- a major bug fix changes known behavior or risk
- architecture changes
- level-data format changes
- asset import rules change
- PixelLab workflow changes
- scene flow, UI panel conventions, or resource reuse strategy changes
- release, privacy, or App Store assumptions change

If the implementation is tiny and does not affect durable project knowledge, no docs change is required, but the final response should say that no documentation update was needed.
