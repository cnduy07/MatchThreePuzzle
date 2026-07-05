# Technical Plan

## Guiding Approach

Finish the game by stabilizing the existing prototype first, then add product systems around it. Avoid rewriting the whole board unless a specific bug or feature proves the current design cannot support release.

## How To Use This Plan

This file is the roadmap and sequencing guide. For current implemented-versus-missing status, start with `docs/CODEBASE_OVERVIEW.md`. For scene/UI implementation rules, use `docs/SCENE_UI_ARCHITECTURE.md`.

When a phase changes, update the relevant current-status bullets here and keep the overview's implemented/missing feature lists in sync.

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
- `Assets/Data/Levels/Level_001.asset` is the first tuned data-driven version of the old `Level 1` board setup, with a reachable `3000` score target.
- `Assets/Data/Levels/Level_001.asset` through `Level_005.asset` are registered in `Assets/Resources/LevelDatabase.asset`.
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
- Level Select shows locked/unlocked state, completion stars, and best score from local progression data.
- `SafeAreaRoot` constrains runtime menu, level select, and gameplay overlay content to `Screen.safeArea`.
- Settings open from both Menu and Pause through the shared `RuntimeUiShell` settings overlay.
- Settings persist audio on/off through `AudioListener.volume` and persist a haptics toggle stub for future haptics service work.
- `Board.SettupCamera()` now uses safe-area aspect ratio, reserves extra top world-space room for the concept HUD/threat area, keeps a smaller bottom margin, and uses a 9-world-unit minimum board fit height baseline.
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

- verify Game scene screenshots on iPhone portrait, tall/notched iPhone, and iPad before further art work
- tune board/HUD spacing only from those screenshots, with board visibility taking priority over decoration
- replace runtime-only placeholder controls with shared UI prefabs after the target layout is stable

## Camera And Board Fit Open Item

Current code has an improved first-pass board-fit implementation in `Board.SettupCamera()`: it centers the camera on the board with a concept-HUD offset that leaves more visible space above the board, computes vertical and horizontal orthographic sizes from `width`, `height`, `borderSize`, reserved top/bottom world-space margins, and `Screen.safeArea` aspect ratio, then chooses the larger size. Smaller boards use a 9-world-unit minimum board fit height baseline so iPad-style layouts do not zoom too aggressively.

Target approach to design during Phase 3:

- Continue tuning safe-area-driven scaling, not letterboxing, for normal iPhone and iPad play.
- Validate the reserved top/bottom UI world-space margins on real target aspect ratios.
- Keep the full board visible on small, notched, large iPhone, and iPad layouts.
- Tune the 9-world-unit board fit baseline after device screenshots.
- Keep the board centered in the available safe gameplay area, not necessarily the full physical screen.
- Keep gameplay background rendering behind world-space board sprites. Do not move gameplay backgrounds into overlay canvas layers.

This remains an open tuning item. Do not claim the game has final responsive board fitting until the camera/layout system is verified on target iPhone and iPad aspect ratios.

## Phase 4: Save Data

Current status:

- `PlayerProgress` stores local JSON save data through `PlayerPrefs`.
- Save data tracks highest unlocked level, completed levels, best score, best stars, coin count placeholder, audio setting, and haptics setting.
- Winning a level records the result and unlocks the next registered level.
- Level Select reads save data to lock/unlock level buttons and show stars/best score.
- Menu Continue starts the highest unlocked level.

Implemented first-pass fields:

- unlocked level
- completed levels
- best score per level
- coin count
- audio/haptics settings

Remaining Phase 4 tuning:

- tune star thresholds after level balancing
- add explicit save reset/debug tooling for development builds if needed
- add save migration/versioning if the format changes after more systems are added
- keep local save only; do not add cloud save until the base product is stable

## Phase 4.5: Resource And Theme Reuse

Current status:

- Initial `PieceSetData`, `ThemeData`, and `GameResourceLibrary` ScriptableObject types exist.
- `Assets/Data/PieceSets/ClassicPieceSet.asset` centralizes the current prototype normal pieces, bombs, collectibles, and blocker references.
- `Assets/Data/Themes/ClassicTheme.asset` centralizes the current placeholder tile reference, UI colors, UI prefab slots, UI sprite slots, and palette notes.
- `Assets/Resources/GameResourceLibrary.asset` provides runtime defaults for theme and piece-set resolution.
- `Assets/Prefabs/UI/Shared/` contains placeholder shared prefabs for primary, secondary, and icon buttons, panel/modal surfaces, level card, objective item, toggle row, slider row, top bar, currency counter, and loading overlay.
- `LevelData` can reference a theme and piece set while preserving direct prefab arrays as level-specific overrides.
- `Board.ApplyLevelData()` resolves missing level prefab references from the selected shared theme/piece set before setup.
- `RuntimeUiFactory`, `SceneBootstrapper`, and `RuntimeUiShell` now use semantic button/panel styles so future pixel-art UI prefabs can be swapped through `ThemeData`.
- Runtime lose flow plays a themed monster-to-door attack animation before the lose modal, matching the `Assets/Concept/gamescene.png` direction with placeholder theme assets.
- Runtime Menu, Level Select, and Game HUD now use concept-style placeholder layouts for top resources, map nodes, action buttons, boosters, pause/settings, and monster-door presentation.
- Runtime concept HUD suppresses legacy scene HUD panels/text and old score decoration, and key placeholder UI elements have lightweight motion polish.
- `ClassicTheme` currently references generated gameplay sprite candidates for pieces, special pieces, collectibles, blockers, some HUD icons, and generated tile candidates where they remained readable.
- Normal board cells intentionally use a clean full-cell tile backing instead of the generated normal tile candidate.
- Gameplay background is a world-space sprite behind the board; HUD and overlays remain on the UI canvas.
- Bottom booster controls are visual placeholders only. Gameplay booster effects, inventory use, and economy integration are not implemented.

Add reusable data and prefab structure before generating many levels or art variants:

- `ThemeData` for background, tiles, pieces, UI sprites, music, and palette notes.
- `PieceSetData` for normal pieces and special piece variants.
- shared UI prefabs for buttons, panels, level cards, objective rows, toggles, and modals.
- sprite atlas grouping for gameplay, UI, and background assets.

Use direct Unity references first. Consider Addressables only when asset count, downloadable content, or memory pressure makes it useful.

Remaining Phase 4.5 work:

- verify the current generated sprite candidates in actual Game scene screenshots and reject weak assets before producing more art
- replace placeholder shared UI prefab visuals with approved pixel-art sprites/sliced panels
- replace the concept-style placeholders with approved sprites, sliced panels, background art, node art, booster icons, and monster/door sprites
- move level assets away from duplicated direct prefab arrays after shared references are verified in Unity
- add sprite atlas grouping once approved gameplay/UI art exists
- add additional theme assets only after one complete theme is validated

## Phase 5: Pixel-Art Asset Replacement

Current status:

- A first generated candidate set exists under `Assets/Sprites/PixelArt/Generated/`.
- The candidate set is wired for gameplay readability review, not approved for release.
- The generated normal tile and generated booster button frame are known weak assets and should not drive final visual direction.
- PixelLab generation remains outside normal implementation workflow unless the user explicitly requests a dedicated artwork pass.

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
- `Board.cs` is a poor candidate for simultaneous parallel edits because of its size and shared responsibilities. Treat the board model / match resolver / level loader / board view split above as a prerequisite, single-session task before multiple sessions attempt concurrent gameplay-core features.
- Current input is Board-owned screen-to-grid mapping with selective UI blocking for interactable `Selectable` controls; validate drag behavior, edge-cell selection, and future overlay controls as panels are added.
- LevelData stores objective type and target count before all objective rules exist, so avoid treating non-score objectives as complete until the resolver/game rules are added.
- Game HUD boosters are visual placeholders. Do not document them as functional until a booster data model, inventory/count consumption, input handlers, and board effects exist.
- Gameplay background must stay behind world-space board sprites; overlay canvas backgrounds can hide the board.
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
