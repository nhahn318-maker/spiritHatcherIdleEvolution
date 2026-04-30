# Spirit Hatchers - Animation Generation Prompts

Use these prompts to generate creature animation sprite sheets that match the project style.

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
