# Spirit Hatchers - Animation Generation Prompts

Use these prompts to generate creature sprites, enemy sprites, battle backgrounds, and animation sprite sheets that match the project style.

## Static Creature Or Enemy Sprite

Use this for single-frame creature sprites, enemy sprites, and ScriptableObject form sprites.

Replace the bracketed values before generating:

- `[CREATURE_NAME]`
- `[CREATURE_ROLE]` such as player creature, enemy creature, expedition enemy
- `[ELEMENT]`
- `[CREATURE_DESCRIPTION]`
- `[COLOR_PALETTE]`
- `[POSE]`
- `[CHROMA_KEY_COLOR]`

Use `#ff00ff` for green/nature creatures. Use `#00ff00` for non-green creatures. The output must be post-processed into a true alpha PNG before importing as a Unity sprite.

```txt
Create a 2D [CREATURE_ROLE] sprite for a cute fantasy mobile creature collection battle game.

Creature: [CREATURE_NAME]. [CREATURE_DESCRIPTION]
Element: [ELEMENT].
Color palette: [COLOR_PALETTE].

Pose: [POSE], full body visible, facing slightly forward, readable silhouette, centered with generous padding.

Art style: cute fantasy, chibi creature collection, cozy magical forest feeling, soft shading, vibrant element-based colors, warm lighting, soft magical glow, polished casual mobile game asset, readable at small size, suitable for Unity 2D.

Sprite requirements:
- 1024x1024
- full body visible
- centered with generous padding
- clean silhouette
- no cropping
- no text
- no UI
- no watermark
- no extra characters
- no cast shadow
- no background scenery

Background: perfectly flat solid [CHROMA_KEY_COLOR] chroma-key background for later transparency removal. The background must be one uniform color with no shadows, gradients, texture, reflections, floor plane, checkerboard pattern, or lighting variation. Do not use [CHROMA_KEY_COLOR] anywhere in the creature.

Avoid: transparent checkerboard background, fake transparency, realistic monster design, horror style, angry scary face unless requested, human-like anatomy unless requested, overly complex details, dark fantasy colors.
```

## Wild Sprout Enemy Example

```txt
Create a 2D enemy creature sprite for a cute fantasy mobile creature collection battle game.

Creature: Wild Sprout enemy. A tiny magical sprout creature shaped like a rounded seed bulb with a fresh green sprout growing from the top. It has simple glossy bead-like eyes only, no mouth or only a tiny simple dot mouth, tiny root nubs at the base instead of legs, and two small leaf growths on the sides instead of arms. Soft moss patches and subtle seed texture. It should feel like a living magical plant, early-game forest enemy, cute and collectible, not a humanoid character.
Element: Nature.
Color palette: leaf green, emerald green, warm brown seed shell, soft cream highlights, small golden magical glow accents.

Pose: full body, facing slightly forward, stationary plant battle pose, rooted on the ground with a slight lean, readable silhouette, centered with generous padding.

Art style: cute fantasy, chibi creature collection, cozy magical forest feeling, soft shading, vibrant element-based colors, warm lighting, soft magical glow, polished casual mobile game asset, readable at small size, suitable for Unity 2D.

Sprite requirements:
- 1024x1024
- full body visible
- centered with generous padding
- clean silhouette
- no cropping
- no text
- no UI
- no watermark
- no extra characters
- no cast shadow
- no background scenery

Background: perfectly flat solid #ff00ff chroma-key background for later transparency removal. The background must be one uniform color with no shadows, gradients, texture, reflections, floor plane, checkerboard pattern, or lighting variation. Do not use #ff00ff anywhere in the creature.

Avoid: transparent checkerboard background, fake transparency, humanoid plant person, human-like arms, human-like legs, realistic plant monster, horror style, angry scary face, excessive thorns, overly complex details, dark fantasy colors.
```

## Expedition Battle Background

Use this for battle scenes that place enemies in the upper half and the player's future 3-creature team in the lower half.

Replace the bracketed values before generating:

- `[EXPEDITION_NAME]`
- `[SCENE_DESCRIPTION]`
- `[MOOD]`
- `[COLOR_PALETTE]`

```txt
Create a vertical 2D mobile game expedition battle background for [EXPEDITION_NAME].

Scene: [SCENE_DESCRIPTION]
Mood: [MOOD]

Battle layout composition:
- Portrait mobile layout, 1080x1920.
- The upper third to middle area must stay visually open for enemy creatures.
- The lower third must have a wide clear battle area where 3 friendly chibi creatures can stand side by side in a horizontal row.
- Keep the very bottom area clean enough for future battle UI buttons, skill icons, HP bars, and action panels.
- Use subtle ground markings, soft moss patches, lighting pools, or small stone positions to imply 3 player creature slots at the bottom, but do not draw actual creatures or UI.
- Background depth should guide the eye from the bottom team area toward the enemy area near the top-middle.

Camera and perspective:
- Slight side-view / three-quarter mobile RPG battle perspective.
- Clear foreground battle ground at the lower-middle.
- Soft depth in the background.
- Keep important details away from the very bottom UI zone.

Art style: cute fantasy, chibi creature collection, cozy magical forest, soft shading, warm lighting, soft magical glow, polished casual mobile game background, high contrast, readable on mobile, suitable for Unity 2D.

Color palette: [COLOR_PALETTE]

Requirements:
- vertical portrait background
- 1080x1920
- background only
- no characters
- no monsters
- no UI buttons
- no text
- no logo
- no watermark
- no baked-in health bars
- no slot circles
- no arrows
- no realistic muddy colors
- no horror or dark fantasy
```

## Forest Walk Battle Background Example

```txt
Create a vertical 2D mobile game expedition battle background for Forest Walk.

Scene: a cozy magical forest battle path with soft green moss, round stones, tiny glowing flowers, warm fireflies, old tree roots, leafy bushes, and a circular forest clearing.
Mood: safe, cute, colorful, and beginner-friendly.

Battle layout composition:
- Portrait mobile layout, 1080x1920.
- The upper third to middle area must stay visually open for enemy creatures.
- The lower third must have a wide clear battle area where 3 friendly chibi creatures can stand side by side in a horizontal row.
- Keep the very bottom area clean enough for future battle UI buttons, skill icons, HP bars, and action panels.
- Use subtle ground markings, soft moss patches, lighting pools, or small stone positions to imply 3 player creature slots at the bottom, but do not draw actual creatures or UI.
- Background depth should guide the eye from the bottom team area toward the enemy area near the top-middle.

Camera and perspective:
- Slight side-view / three-quarter mobile RPG battle perspective.
- Clear foreground battle ground at the lower-middle.
- Soft depth in the magical forest background.
- Keep important details away from the very bottom UI zone.

Art style: cute fantasy, chibi creature collection, cozy magical forest, soft shading, warm lighting, soft magical glow, polished casual mobile game background, high contrast, readable on mobile, suitable for Unity 2D.

Color palette: emerald green, leaf green, warm gold light, soft brown wood, small blue and purple magic accents.

Requirements:
- vertical portrait background
- 1080x1920
- background only
- no characters
- no monsters
- no UI buttons
- no text
- no logo
- no watermark
- no baked-in health bars
- no slot circles
- no arrows
- no realistic muddy colors
- no horror or dark fantasy
```

## Idle Creature Sprite Sheet

Replace the bracketed values before generating:

- `[CREATURE_NAME]`
- `[FORM_NAME]`
- `[ELEMENT]`
- `[CREATURE_DESCRIPTION]`
- `[COLOR_PALETTE]`
- `[KEY_MOTION]`

```txt
Create a 2D sprite sheet for a cute chibi fantasy creature idle animation, matching a cozy mobile creature collection game style.

Creature: [CREATURE_NAME], [FORM_NAME]. [CREATURE_DESCRIPTION]
Element: [ELEMENT].
Color palette: [COLOR_PALETTE].

Art style: cute fantasy, chibi creature collection, cozy magical forest feeling, soft shading, vibrant element-based colors, warm lighting, soft magical glow, polished casual mobile game asset, readable at small size, suitable for Unity 2D.

Animation: idle breathing loop with very subtle body bounce, [KEY_MOTION]. Keep the pose calm and cute, not action-based.

Sprite sheet requirements:
- 8 frames total
- arranged horizontally in one row
- 8 columns x 1 row
- each frame should fit inside a 384x512 cell
- wide padding around the full creature in every frame
- no part of the ears, tail, wings, horns, flame, aura, or body may touch the frame edges
- same canvas size, same scale, same centered position in every frame
- full body visible in every frame
- consistent character design across all frames
- no cropping

Background: perfectly flat solid #00ff00 chroma-key background for later transparency removal. The background must be one uniform color with no shadows, gradients, texture, reflections, floor plane, or lighting variation. Do not use #00ff00 anywhere in the creature.

Avoid: no text, no UI, no watermark, no cast shadow, no contact shadow, no extra characters, no frame labels, no grid lines.
```

## Melee Attack Sprite Sheet

Use this only when a real frame-by-frame creature attack sheet is needed. For early battle prototypes, prefer code-driven motion using the idle sprite plus a small impact effect because generated attack sheets can drift between frames.

Replace the bracketed values before generating:

- `[CREATURE_NAME]`
- `[FORM_NAME]`
- `[CREATURE_DESCRIPTION]`
- `[ELEMENT]`
- `[SKILL_NAME]`
- `[SKILL_DESCRIPTION]`
- `[COLOR_PALETTE]`
- `[FACING_DIRECTION]` such as `3/4 view facing right`
- `[CHROMA_KEY_COLOR]`

```txt
Create a 2D sprite sheet for a cute chibi fantasy creature melee attack animation, designed for Unity 2D frame-by-frame animation.

Creature: [CREATURE_NAME], [FORM_NAME]. [CREATURE_DESCRIPTION]
Element: [ELEMENT].
Skill: [SKILL_NAME], [SKILL_DESCRIPTION].
Color palette: [COLOR_PALETTE].

Critical consistency requirements:
- The character must face the same direction in every frame: [FACING_DIRECTION].
- Same camera angle in every frame.
- Same character design in every frame.
- Same body proportions in every frame.
- Same scale in every frame.
- Same feet/root pivot position as much as possible.
- Do not rotate the character to different views.
- Do not show front view, back view, or opposite direction.
- Do not create different poses as separate character concepts.
- This must look like one continuous animation of the exact same creature.

Art style: cute fantasy, chibi creature collection, cozy magical forest feeling, soft shading, vibrant element-based colors, warm lighting, soft magical glow, polished casual mobile game asset, readable at small size, suitable for Unity 2D.

Animation sequence:
Frame 1: idle battle stance, [FACING_DIRECTION].
Frame 2: slight crouch anticipation, [FACING_DIRECTION].
Frame 3: lean forward, attack ready, [FACING_DIRECTION].
Frame 4: short melee lunge forward, small element spark near attack point, [FACING_DIRECTION].
Frame 5: impact pose, tiny element spark, [FACING_DIRECTION].
Frame 6: recoil backward slightly, [FACING_DIRECTION].
Frame 7: return toward idle pose, [FACING_DIRECTION].
Frame 8: idle battle stance again, [FACING_DIRECTION].

Sprite sheet requirements:
- 8 frames total
- arranged horizontally in one row
- 8 columns x 1 row
- each frame fits inside a 384x512 cell
- full body visible in every frame
- wide padding around the creature in every frame
- no cropping
- no frame labels
- no grid lines
- no text
- no UI
- no watermark
- no extra characters
- no cast shadow
- no contact shadow

Background: perfectly flat solid [CHROMA_KEY_COLOR] chroma-key background for later transparency removal. The background must be one uniform color with no shadows, gradients, texture, reflections, floor plane, checkerboard pattern, or lighting variation. Do not use [CHROMA_KEY_COLOR] anywhere in the creature.

Avoid: changing viewing angle, rotating the creature, changing direction, different character designs between frames, different scale between frames, front view, back view, opposite-facing frames, dramatic camera movement, action comic panels, separate pose concepts, horror, blood.
```

## Ember Fox - Baby Melee Attack Example

```txt
Create a 2D sprite sheet for a cute chibi fantasy creature melee attack animation, designed for Unity 2D frame-by-frame animation.

Creature: Ember Fox, Baby Ember Fox form. A small orange fire fox with big expressive eyes, flame-like ears, glowing flame tail, warm orange and gold colors.
Element: Fire.
Skill: Ember Bite, a close-range bite attack.
Color palette: warm orange, golden yellow, cream fur, soft red flame accents.

Critical consistency requirements:
- The character must face the same direction in every frame: 3/4 view facing right.
- Same camera angle in every frame.
- Same character design in every frame.
- Same body proportions in every frame.
- Same scale in every frame.
- Same feet/root pivot position as much as possible.
- Do not rotate the character to different views.
- Do not show front view, back view, or opposite direction.
- Do not create different poses as separate character concepts.
- This must look like one continuous animation of the exact same creature.

Art style: cute fantasy, chibi creature collection, cozy magical forest feeling, soft shading, vibrant fire colors, warm lighting, soft magical glow, polished casual mobile game asset, readable at small size, suitable for Unity 2D.

Animation sequence:
Frame 1: idle battle stance, facing right.
Frame 2: slight crouch anticipation, facing right.
Frame 3: lean forward, mouth slightly open, facing right.
Frame 4: short bite lunge forward, small fire spark near mouth, facing right.
Frame 5: bite impact pose, tiny fire spark, facing right.
Frame 6: recoil backward slightly, facing right.
Frame 7: return toward idle pose, facing right.
Frame 8: idle battle stance again, facing right.

Sprite sheet requirements:
- 8 frames total
- arranged horizontally in one row
- 8 columns x 1 row
- each frame fits inside a 384x512 cell
- full body visible in every frame
- wide padding around the creature in every frame
- no cropping
- no frame labels
- no grid lines
- no text
- no UI
- no watermark
- no extra characters
- no cast shadow
- no contact shadow

Background: perfectly flat solid #00ff00 chroma-key background for later transparency removal. The background must be one uniform color with no shadows, gradients, texture, reflections, floor plane, checkerboard pattern, or lighting variation. Do not use #00ff00 anywhere in the creature.

Avoid: changing viewing angle, rotating the creature, changing direction, different character designs between frames, different scale between frames, front view, back view, left-facing frames, dramatic camera movement, action comic panels, separate pose concepts, horror, blood.
```

## Ember Fox - Baby Idle Example

```txt
Create a 2D sprite sheet for a cute chibi fantasy creature idle animation, matching a cozy mobile creature collection game style.

Creature: Ember Fox, Baby Ember Fox form. A baby orange fire fox with big expressive eyes, tiny body, soft rounded shapes, flame-like ears, a glowing flame tail, warm orange and gold colors, and a clean readable silhouette.
Element: Fire.
Color palette: warm orange, golden yellow, cream fur, soft red flame accents.

Art style: cute fantasy, chibi creature collection, cozy magical forest feeling, soft shading, vibrant element-based colors, warm lighting, soft magical glow, polished casual mobile game asset, readable at small size, suitable for Unity 2D.

Animation: idle breathing loop with very subtle body bounce, ears gently moving, and tail flame softly flickering. Keep the pose calm and cute, not action-based.

Sprite sheet requirements:
- 8 frames total
- arranged horizontally in one row
- 8 columns x 1 row
- each frame should fit inside a 384x512 cell
- wide padding around the full creature in every frame
- no part of the ears, tail, flame, or body may touch the frame edges
- same canvas size, same scale, same centered position in every frame
- full body visible in every frame
- consistent character design across all frames
- no cropping

Background: perfectly flat solid #00ff00 chroma-key background for later transparency removal. The background must be one uniform color with no shadows, gradients, texture, reflections, floor plane, or lighting variation. Do not use #00ff00 anywhere in the creature.

Avoid: no text, no UI, no watermark, no cast shadow, no contact shadow, no extra characters, no frame labels, no grid lines.
```

## Unity Import Target

Preferred normalized sheet after background removal:

- 8 frames
- 1 row
- 384x512 per frame
- 3072x512 total sheet size
- Sprite Mode: Multiple
- Slice: Grid By Cell Size, 384 x 512
- Animation sample rate: 8
- Loop Time: enabled

## Automated Prep Command

After generating an image, run this from the project root to copy the newest generated PNG into the project, remove the green background, and create a padded Unity-ready sheet:

```powershell
python "Assets/_Project/Tools/prepare_idle_sprite_sheet.py" `
  --latest-generated `
  --out-dir "Assets/_Project/Art/Creatures/Fire/EmberFox" `
  --name "flame_fox_idle_reference"
```

Output files:

- `[name]_raw.png`
- `[name]_transparent.png`
- `[name]_padded_384x512.png`

In Unity, slice the padded file with:

- Sprite Mode: Multiple
- Slice Type: Grid By Cell Size
- Pixel Size: 384 x 512
- Pivot: Center or Bottom
