# Pixel Art Direction

## Art Goal

The game should look like a cohesive premium pixel-art mobile puzzle game, not a mixture of generated placeholders. PixelLab can create the first full asset pass, but every generated asset needs review for readability, consistency, and Unity import quality.

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

When `mcp__pixellab` tools are available, use PixelLab for draft generation.

For single match pieces, use `create_map_object`:

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

- Generate assets in batches with the same descriptive style language.
- Keep outlines consistent in thickness and color.
- Keep lighting direction consistent.
- Keep canvas size and padding consistent.
- Do not mix realistic, vector, painterly, and pixel-art assets.
- Avoid overly detailed sprites that blur at board size.
- Avoid sprites identifiable only by color.
- Treat UI as a shared kit. Do not generate unrelated panel/button styles for different scenes.
- Use ThemeData-style thinking: backgrounds, tiles, pieces, UI, sounds, and palette should feel like one world.

## UI Asset Set

Generate UI art as a reusable kit, not isolated images:

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

This proves the MCP integration works. It is a test asset, not final art direction by itself.

## Approval Workflow

1. Generate 2-3 candidate styles for one normal piece.
2. User selects preferred direction.
3. Generate the full normal piece set in that style.
4. Generate matching special pieces.
5. Generate tiles and blockers.
6. Generate a matching UI kit.
7. Generate backgrounds.
8. Import into Unity using consistent settings.
9. Review in scene at actual phone size.
10. Iterate only on weak/readability-breaking assets.
