# Pixel Art Direction

## Art Goal

The game should look like a cohesive premium pixel-art mobile puzzle game, not a mixture of generated placeholders. Artwork generation is handled outside ordinary implementation tasks; agents should keep the project ready for approved assets without invoking image-generation tools.

## Visual Pillars

- chunky, readable silhouettes
- bright but controlled palette
- high contrast on gameplay pieces
- clean transparent-background sprites
- satisfying special-piece visuals
- UI that looks pixel-crafted but remains touch-friendly
- backgrounds that support the board instead of competing with it

## Camera And Sprite Style

Recommended style for match pieces:

- front/side presentation, not deep perspective
- centered on transparent 128x128 canvas
- readable at 64px and gameplay scale
- selective outline
- medium shading
- limited highlight details
- unique silhouette per color

Recommended style for board tiles:

- square tile, top-down or very shallow perspective
- subtle border
- enough contrast behind pieces
- normal, breakable, double-breakable, obstacle, and goal-related states

Recommended style for UI:

- pixel-art panels and buttons
- simple iconography
- no tiny decorative text
- large touch targets
- consistent bevel/outline language
- reusable visual components across menu, level select, gameplay HUD, settings, pause, win, and lose overlays

## Asset Sizes

Initial practical sizes:

- match pieces: 128x128 PNG
- bombs: 128x128 PNG
- blockers/collectibles: 128x128 PNG
- board tiles: 128x128 PNG
- UI icons: 128x128 or 256x256 PNG
- buttons/panels: generated at intended UI aspect, then sliced or used as sprites
- backgrounds: phone portrait friendly, ideally 1536x2048 or larger source if generated outside PixelLab

Use a consistent Pixels Per Unit plan. The current existing game assets use mixed PPU values. A clean replacement pass should standardize gameplay pieces and tiles.

Recommended starting point:

- gameplay cell = 1 Unity unit
- piece sprite = 128 pixels wide
- import PPU = 128
- filter mode = Point
- compression = None or high-quality lossless choice after profiling
- mipmaps = off

## PixelLab Usage

PixelLab is disabled for normal agent workflow. Agents must not call PixelLab tools or any image-generation tools from this thread.

When artwork is needed:

- use existing placeholder assets
- prepare implementation around replaceable asset references
- explain which assets are required
- prepare TODOs or prompt drafts if useful

Do not generate sprites, UI assets, icons, backgrounds, or any other artwork. Artwork generation belongs in a separate dedicated workflow.

For single match-piece prompt drafts:

- transparent background
- 128x128
- medium detail
- selective outline
- medium shading
- side view or top-down depending on the target sprite

Example prompt pattern:

`single match-3 puzzle piece sprite: [object/color], chunky pixel art, clear silhouette, centered, transparent background, mobile game asset, readable at small size`

Suggested normal piece set:

- red ruby gem
- blue water drop
- yellow star coin
- green leaf crystal
- purple moon candy
- orange sun shell

Suggested special pieces:

- row bomb: same color piece with horizontal stripe/arrow
- column bomb: same color piece with vertical stripe/arrow
- adjacent bomb: same color piece with burst ring
- color bomb: rainbow prism or sparkling orb, distinct from normal pieces

Suggested blockers and collectibles:

- blocker: dark stone crate or locked block
- breakable tile: cracked stone/glass states
- collectible: golden key, fruit, or charm
- coin: small gold coin for economy

## Style Consistency Rules

- Prepare related asset prompt drafts with the same descriptive style language.
- Keep outlines consistent in thickness and color.
- Keep lighting direction consistent.
- Keep canvas size and padding consistent.
- Do not mix realistic, vector, painterly, and pixel-art assets.
- Avoid overly detailed sprites that blur at board size.
- Avoid sprites identifiable only by color.
- Treat UI as a shared kit. Do not create unrelated panel/button styles for different scenes.
- Use ThemeData-style thinking: backgrounds, tiles, pieces, UI, sounds, and palette should feel like one world.

## UI Asset Set

Plan UI art as a reusable kit, not isolated images:

- primary button
- secondary button
- icon button
- panel frame
- modal window
- level card
- objective badge
- currency counter
- settings toggle background
- progress/star frame
- pause/settings/close/retry/next icons

The same kit should support menu, level select, gameplay HUD, settings, pause, win, and lose overlays.

## Import Checklist

For every imported gameplay sprite:

- texture type: Sprite
- sprite mode: Single unless atlas sheet
- filter mode: Point
- mipmaps: disabled
- alpha is transparency: enabled
- PPU: consistent with board cell size
- compression: None for source quality, then optimize later
- pivot: center for pieces, appropriate pivot for UI

## Current Test Asset

PixelLab test sprite:

- `Assets/Sprites/PixelArt/Test_RubyGem.png`
- PixelLab object ID: `8d614ae7-b860-4875-9ef0-fbde1ab9db11`

This historical test asset is not final art direction by itself. Do not use it as permission to call PixelLab or generate more artwork in ordinary implementation tasks.

## Current Generated Candidate Set

User-approved PixelLab MCP generation was run in this thread on 2026-07-03 for the first gameplay-readability pass. The generated sprites are candidates for review, not automatically final release art.

Generated candidate folder:

- `Assets/Sprites/PixelArt/Generated/`

Generated sprites wired into gameplay prefabs:

- `piece_red_ruby.png`
- `piece_blue_drop.png`
- `piece_yellow_star.png`
- `piece_green_leaf.png`
- `piece_purple_moon.png`
- `piece_orange_shell.png`
- `piece_color_bomb_prism.png`
- `special_row_bomb_arrow.png`
- `special_column_bomb_rocket.png`
- `special_adjacent_bomb.png`
- `blocker_dark_stone.png`
- `collectible_golden_key.png`
- `ui_coin.png`
- `tile_normal_stone.png`
- `tile_breakable_cracked.png`
- `tile_obstacle_dark_stone.png`
- `ui_booster_hammer.png`
- `ui_booster_bomb.png`
- `ui_booster_color.png`
- `ui_booster_hand.png`
- `ui_pause_icon.png`

Import settings applied to this folder:

- Texture Type: Sprite
- Sprite Mode: Single
- Filter Mode: Point
- Mipmaps: disabled
- PPU: 128
- Texture compression: uncompressed in the generated metas

Wiring status:

- `Dot 1` through `Dot 6` use the six normal generated piece sprites.
- Row, column, and adjacent bomb prefabs use generated normal piece roots plus generated special overlay sprites.
- `ColorBomb`, `Collectible`, `Blocker`, breakable tiles, and obstacle tiles use generated sprite candidates.
- Normal board cells intentionally use the existing full-cell `Assets/Sprites/squareFilled.png` sprite with a visible dark blue tint because the generated `tile_normal_stone.png` read as broken chunks behind gameplay pieces.
- Breakable tile state `0` resolves to the clean full-cell tile backing; cracked states resolve to `tile_breakable_cracked.png`.
- Board tile prefabs render on the board layer, while generated piece and bomb marker sprites render above the board tiles.
- `ClassicTheme` assigns `sky_night.png` as the gameplay background, displayed through a world-space background sprite behind the board.
- `ClassicTheme` assigns generated booster and pause icon sprites for the gameplay HUD. The generated `ui_booster_button_frame.png` is not assigned because it read poorly at gameplay size; booster cards currently use constructed runtime panels until better approved button art exists.
- `Assets/Scripts/Editor/GeneratedSpritePrefabWireUp.cs` can reapply the generated sprite import settings and prefab wiring.

Review notes:

- The six normal pieces are the most coherent part of the set and should be tested at actual board size first.
- The row, column, and adjacent bomb sprites are generic special-piece candidates. They may need color-specific variants or overlays before replacing all current bomb prefabs.
- The generated normal tile candidate should remain unused unless it is replaced by a full-cell tile that does not compete with pieces.
- The breakable, obstacle, and gameplay HUD icons are first-pass candidates and should be checked in the `Game` scene.
- These sprites are wired into gameplay prefabs, but they are still candidate art until reviewed in actual phone/iPad gameplay layouts.

## Approval Workflow

1. Identify the next needed asset or logical asset set.
2. Use placeholders and keep implementation references replaceable.
3. Prepare TODOs or prompt drafts and explain why the asset is needed.
4. Wait for the separate artwork workflow to provide approved assets.
5. Import approved assets into Unity using consistent settings.
6. Review in scene at actual phone size.
7. Iterate only on weak/readability-breaking assets after review.
