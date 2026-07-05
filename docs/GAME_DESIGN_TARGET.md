# Game Design Target

## Product Goal

Create a polished, approachable pixel-art match-3 puzzle game for iPhone and iPad. The game should feel simple to start, readable on small screens, and satisfying through cascading clears, special pieces, collectibles, and level goals.

## Audience

Primary audience:

- casual puzzle players
- mobile users playing short sessions
- players who understand Candy Crush-like swap matching
- users who value clarity and satisfying feedback over complex systems

Design for one-handed phone play first, then scale comfortably to iPad.

## Core Gameplay Loop

1. Player enters a level.
2. Player sees a clear goal and move count.
3. Player swaps adjacent pieces to make matches of 3 or more.
4. Matched pieces clear, score increments, and pieces cascade.
5. Larger matches create special pieces.
6. Special pieces clear rows, columns, nearby pieces, or colors.
7. Player wins by reaching the level goal before moves run out.
8. Player advances, replays, or returns to level select.

## Current Match Rules

- Match 3: clear pieces.
- Match 4: create row or column bomb based on swap direction.
- Match 5 or more: create color bomb, unless the match shape is a corner pattern.
- Corner/L style: create adjacent bomb.
- Color bomb swap: clears all pieces with the swapped piece's match value.
- Collectibles use `MatchValue.None`.
- Some collectibles clear at the bottom row.

Do not change these rules casually. If rules change, update tests and player-facing tutorial language.

## Level Goals

The current playable rules use score goal and moves left. `LevelData` already stores objective type and target count for future levels, but non-score objective rules are not implemented in win/loss resolution yet.

The finished game should support multiple level objective types:

- reach score
- collect target items
- clear blockers
- break all breakable tiles
- bring collectibles to bottom
- mixed objectives

Start with score and collectible goals because the codebase already supports them.

## Level Progression

Current implementation:

- Levels 1-5 exist as data-driven score-goal levels.
- Winning a level unlocks the next registered level.
- Level Select shows locked/unlocked state, stars, and best score.
- Audio and haptics toggle state persist locally.

Recommended progression model:

- 30-50 initial levels for first public test build.
- First 5 levels teach core mechanics.
- Levels 6-15 introduce bombs and blockers.
- Levels 16-30 mix objectives and board shapes.
- Later levels introduce harder layouts and tuned move limits.

Each level should define:

- board width and height
- move limit
- score goal
- piece colors enabled
- tile layout
- starting pieces
- blockers
- collectibles
- collectible spawn chance and maximum
- objective type and target count

## Tutorial Plan

Tutorial should be interactive and brief:

1. Swap two pieces to make a 3-match.
2. Make a 4-match to create a row or column bomb.
3. Trigger a special piece.
4. Make or use a color bomb.
5. Clear or collect the objective item.

Avoid long text blocks. Use simple highlights, arrows, and locked input when teaching one move.

## Economy Scope

For the initial version, keep economy simple:

- coins earned from level completion
- optional boosters unlocked later
- no ads or IAP until core gameplay is stable

If ads, analytics, IAP, or randomized paid rewards are added later, update privacy, App Store metadata, and review documentation before release.

## UX Principles

- Board readability is more important than decoration.
- Player should always understand the goal, moves left, and outcome.
- Pieces must be distinguishable by shape as well as color.
- Swaps should feel responsive.
- Invalid moves should be clear but not punishing.
- Cascades should be exciting but not too slow.
- End screens should quickly offer replay or next level.

## Screens Needed

Minimum finished game:

- boot/loading
- home
- level select
- gameplay
- pause
- settings
- level win
- level lose
- credits/licenses

## Concept Reference Direction

Use `Assets/Concept/` as the current UI/UX visual target:

- `menuscene.png` targets a full-screen illustrated home scene with resource counters, large logo, stacked primary actions, settings, and bottom navigation.
- `levelmapscene.png` targets a world-map level select with themed regions, path-based level nodes, star ratings, locks, and bottom navigation.
- `gamescene.png` targets a gameplay scene with top counters/objectives, a visible monster-and-door threat scene above the board, boosters below the board, and a pause button.

The gameplay threat scene is part of the product fantasy: when the player runs out of moves, the monster should rush the door and attack before the lose overlay appears. Until approved monster/door sprites exist, implementation should use placeholder theme slots and shared prefabs rather than generated art.

Current implementation status:

- Menu uses a concept-style placeholder layout with top resources, large logo, stacked actions, settings, progress sign, and bottom nav.
- Level Select uses a concept-style placeholder map with region labels, path segments, level nodes, locks, and bottom nav.
- Gameplay uses a concept-style placeholder HUD with level/goal/moves counters, monster-door threat area, score, boosters, and pause.
- Gameplay background currently renders behind the world board; HUD and overlays render on the safe-area UI canvas.
- Bottom booster controls are visual placeholders only. They show candidate icons and counts, but do not spend inventory or trigger board effects.
- Final approved art is still required for logo, icons, buttons, boosters, map regions, nodes, monster, door, and release-quality backgrounds.

Optional later:

- shop
- daily reward
- events
- achievements

## Scene And Screen Model

Use a small scene set with reusable overlays:

- Boot scene for persistent managers and routing.
- Menu scene for home navigation.
- Level Select scene for progression.
- Game scene for all playable levels.
- Overlay panels for pause, settings, tutorial, win, lose, rewards, and confirmations.

Do not make settings, win, lose, or tutorial separate scenes at first. They need to overlay gameplay and reuse the same UI style.

## Gameplay Scene Layout

Recommended portrait layout:

- top area: level, goal, moves, score
- center: board
- bottom area: boosters or contextual controls
- safe-area padding for notches and home indicator

iPad layout should not simply stretch the board too large. Keep board readable and use extra space for cleaner margins or side UI.

## Integrated Feature Direction

Future feature additions should connect to the same player loop and UI shell.

High-value additions:

- world/level progression
- star rating per level
- richer objective types
- functional boosters connected to inventory, board effects, and economy
- daily challenge with fixed seed
- combo meter for cascades
- coins earned from wins
- pixel-art collection album
- haptics
- interactive tutorial

Avoid isolated features that do not connect to save data, progression, UI, art style, or resource reuse.
