# Spirit Hatchers - Creature Asset And Animation Prompts

Use this file as the practical checklist for generating creature display sprites and animation sheets. The art style can evolve per creature, but the Unity-facing technical rules must stay consistent.

## Core Rules

- Creature animation direction: 3/4 view facing right.
- Unity mirrors enemy-side creatures horizontally; do not generate separate left-facing sheets.
- Static display sprite size: `1024x1024`.
- Creature animation sheet size: `3072x512`.
- Animation layout: `8` frames, `8 columns x 1 row`.
- Animation cell size: `384x512`.
- Current project standing baseline: the lowest visible feet/root/contact pixel should land around `y = 376` from the top of each `384x512` cell.
- Keep the same feet/root/contact point across all frames in a sheet.
- Do not center by visual mass if it moves the feet/root point away from the baseline.
- Use a flat chroma-key background first, then remove it to real PNG alpha.
- Chroma key: use `#00ff00` for non-green creatures and `#ff00ff` for green/nature creatures.
- Final Unity sprites must have real alpha, no checkerboard/fake transparency.

Why `y = 376`: the current Ember Fox and Flame Fox idle sheets use this visual baseline. Older notes mentioned `y = 472-474`, but that places new sheets visibly lower than the existing battle sprites.

## Skill Progression Data

Skills are now explicit data, not just loose visual overrides.

- `CreatureStaticData.skillName`, `skillRange`, `skillPowerMultiplier`, `attackFrames`, `projectileFrames`, and `impactFrames` are legacy fallback fields.
- Each `CreatureFormData` has `unlockedSkills`.
- Treat `unlockedSkills` as newly unlocked skills for that form, not the full repeated list.
- Runtime skill access is cumulative by `formIndex`: a later form has all skills unlocked by earlier forms plus its own new skill.
- The active battle attack is the last non-empty unlocked skill available up to the current form.
- Passive/ambient orbit visuals use the latest unlocked skill with orbit effect frames, even when the active attack is a newer bite or projectile skill.
- For orbit skills that need real per-orb hit checks, use `orbitOrbFrames` instead of a precomposed multi-orb orbit sheet. Runtime spawns `orbitOrbCount` separate orbs, rotates them around the creature, and plays `orbitHitEffectFrames` when each orb touches an enemy.
- If a creature has no `unlockedSkills`, battle falls back to the legacy fields.
- Use one `CreatureSkillData` entry per newly unlocked skill, with its own name, range, multiplier, attack frames, projectile frames, impact frames, orbit effect frames, and orbit hit frames.

Ember Fox current progression:

- Baby Ember Fox unlocks `Ember Bite`.
- Flame Fox unlocks `Flame Orbit`, and still keeps `Ember Bite`.
- Inferno Fox unlocks `Inferno Bite`, and still keeps `Ember Bite` plus `Flame Orbit`.
- The next Inferno-specific skill should be added as a new `unlockedSkills` entry on the Inferno form.

## Style Rule

Keep the Spirit Hatchers identity as a loose art direction, not a hard style lock:

- fantasy creature collection
- readable silhouette
- vibrant element colors
- expressive face/body language
- polished 2D game asset
- readable at small mobile sizes

Do not force every creature into the old cute/chibi look. If a creature form has a stronger reference image, match that reference while preserving the project-facing technical rules above.

## Static Creature Sprite Prompt

Replace bracketed values before generating.

```txt
Create a 1024x1024 2D fantasy creature sprite for Unity.

Use the provided reference image as the main visual target if one is provided. Match its character design, proportions, pose language, colors, markings, silhouette, and rendering detail. Keep the Spirit Hatchers creature-collection readability, but do not redesign the creature into a different style.

Creature: [CREATURE_NAME], [FORM_NAME]. [CREATURE_DESCRIPTION]
Element: [ELEMENT].
Color palette: [COLOR_PALETTE].

Pose:
- full body visible
- centered with generous padding
- clean readable silhouette
- [POSE_DESCRIPTION]

Sprite requirements:
- 1024x1024
- full body visible
- no cropping
- no text
- no UI
- no watermark
- no extra characters
- no cast shadow
- no background scenery

Background:
- perfectly flat solid [CHROMA_KEY_COLOR] chroma-key background for later transparency removal
- one uniform color only
- no shadows, gradients, textures, reflections, floor plane, or checkerboard pattern
- do not use [CHROMA_KEY_COLOR] anywhere in the creature

Avoid:
fake transparency, transparent checkerboard, cropped parts, inconsistent anatomy, muddy colors, horror style unless requested, extra characters, text, UI, watermark.
```

## Idle Animation Prompt

Use this for a new idle sheet. If a static creature sprite already exists, provide it as the strict visual reference.

```txt
Create a 2D sprite sheet for a fantasy creature idle animation, designed for Unity 2D frame-by-frame animation.

Use the provided [CREATURE_NAME] image as the strict visual reference for character design, proportions, rendering detail, colors, markings, silhouette, and style. Match the reference closely. Do not redesign the creature into a different style.

Creature: [CREATURE_NAME], [FORM_NAME]. [CREATURE_DESCRIPTION]
Element: [ELEMENT].
Color palette: [COLOR_PALETTE].

Facing direction:
- The creature must face right in every frame.
- Use a consistent 3/4 view facing right.
- Do not create left-facing, front-facing, back-facing, or mixed-direction frames.

Idle motion:
- calm breathing loop
- [KEY_MOTION], such as ear flick, tail flicker, leaf sway, slime bounce, wing flutter, or glow pulse
- motion should be subtle and loop cleanly
- frame 1 and frame 8 should be nearly identical

Battle alignment:
- 8 frames total, arranged horizontally in one row
- each frame fits inside a 384x512 cell
- final normalized sheet should be 3072x512 after background removal
- place the lowest visible feet/root/contact pixel at about y = 376 from the top of each cell
- keep the same feet/root/contact point across every frame
- do not let the creature float, sink, slide, or recenter differently per frame

Consistency:
- same creature design in every frame
- same camera angle in every frame
- same body proportions in every frame
- same scale in every frame
- full body visible in every frame
- generous padding around ears, tail, wings, effects, and paws/roots

Background:
- perfectly flat solid [CHROMA_KEY_COLOR] chroma-key background for later transparency removal
- one uniform color only
- no shadows, gradients, textures, reflections, floor plane, or checkerboard pattern
- do not use [CHROMA_KEY_COLOR] anywhere in the creature

Avoid:
frame labels, grid lines, text, UI, watermark, extra characters, cast shadow, contact shadow, fake transparency, cropped parts, changing direction, changing scale, changing feet/root position, different designs between frames.
```

## Attack Or Skill Animation Prompt

Use this after an idle sheet exists. The attack sheet should fit the idle sheet; Unity handles battle movement.

```txt
Create a 2D sprite sheet for a fantasy creature [ATTACK_OR_SKILL_TYPE] animation, designed for Unity 2D frame-by-frame animation.

Use the provided [CREATURE_NAME] idle facing-right sprite sheet as the strict reference for design, scale, canvas padding, vertical placement, and feet/root pivot. This is an in-place animation fit pass, not a redesign.

Creature: [CREATURE_NAME], [FORM_NAME]. [CREATURE_DESCRIPTION]
Element: [ELEMENT].
Skill: [SKILL_NAME], [SKILL_DESCRIPTION].
Color palette: [COLOR_PALETTE].

Facing direction:
- The creature must face right in every frame.
- Use a consistent 3/4 view facing right.
- Do not create left-facing, front-facing, back-facing, or mixed-direction frames.

Idle-to-attack alignment:
- Keep the same 8 columns x 1 row layout and 384x512 cell size as the idle sheet.
- Final normalized sheet should be 3072x512 after background removal.
- Frame 1 must match idle frame 1 as closely as possible in scale, position, feet/root pivot, and silhouette.
- Frame 8 must return to idle frame 1 scale, position, feet/root pivot, and silhouette.
- Keep the lowest feet/root/contact pixel on the same baseline as the idle sheet, about y = 376.
- Do not shift the whole creature forward, up, down, left, or right inside the cell.
- Attack motion should come from pose changes, body/face/limb/tail/ear/wing movement, and attached effects.

Effect separation:
- Detached projectiles, trails, beams, hit flashes, and impact effects should be separate assets.
- Only include effects that remain attached to the creature unless this sheet explicitly needs otherwise.

Frame sequence:
Frame 1: idle stance matching idle frame 1.
Frame 2: anticipation.
Frame 3: stronger wind-up or charge.
Frame 4: release preparation.
Frame 5: attack or release pose.
Frame 6: recoil or recovery.
Frame 7: return toward idle.
Frame 8: idle stance matching idle frame 1.

Background:
- perfectly flat solid [CHROMA_KEY_COLOR] chroma-key background for later transparency removal
- one uniform color only
- no shadows, gradients, textures, reflections, floor plane, or checkerboard pattern
- do not use [CHROMA_KEY_COLOR] anywhere in the creature

Avoid:
detached projectile unless requested, impact effect unless requested, frame labels, grid lines, text, UI, watermark, extra characters, cast shadow, contact shadow, fake transparency, cropped parts, changing direction, changing scale, changing feet/root position, sliding the whole creature, different designs between frames.
```

## Effect Asset Prompt

Use separate effect sheets for projectiles, impacts, trails, orbit effects, or hit flashes.

```txt
Create a 2D [EFFECT_TYPE] sprite sheet for Unity.

Effect: [EFFECT_NAME]. [EFFECT_DESCRIPTION]
Element: [ELEMENT].
Color palette: [COLOR_PALETTE].

Requirements:
- [FRAME_COUNT] frames
- arranged horizontally in one row
- [CELL_WIDTH]x[CELL_HEIGHT] per frame
- full effect visible in every frame
- centered with padding
- no text, UI, watermark, frame labels, or grid lines
- perfectly flat solid [CHROMA_KEY_COLOR] background for later alpha removal
```

## 12-Frame Ultimate Effect Prompt Notes

Use this for large impact/ultimate VFX sheets such as `Inferno Nova`.

Unity import target:

- Total canvas size: `4608x512`.
- Frame count: `12`.
- Layout: `12 columns x 1 row`.
- Cell size: `384x512`.
- Unity slicing: `Grid By Cell Count`, `C = 12`, `R = 1`, offset `0`, padding `0`, pivot `Center`.
- Recommended `Pixels Per Unit`: `256`.
- Final asset should be a real alpha PNG. Generate on flat `#00ff00` first if needed, then remove chroma key.

Important prompt rules:

- Every frame must be an independent centered VFX pose.
- Do not create one continuous panoramic effect across the whole strip.
- Keep the full visible effect, including glow/sparks/particles, inside a centered `220x220` safe area per frame.
- Leave at least `72px` empty padding left/right and `110px` empty padding top/bottom inside every `384x512` frame.
- Nothing visible may cross the invisible Unity slice lines at every `384px`.
- Do not draw grid lines, frame labels, fake transparency checkerboard, shadows, or UI.

Copy-ready prompt block:

```txt
Create a SINGLE horizontal 12-frame 2D game VFX spritesheet for [SKILL_NAME].

Canvas:
- Exact total canvas size: 4608x512 pixels.
- Exactly 12 equal frames in one row.
- Each frame is exactly 384x512 pixels.
- Designed for Unity slicing with Grid By Cell Count: Columns 12, Rows 1.
- Every frame is an independent centered VFX pose.
- Do not create one continuous effect across the whole strip.
- Do not let any flame, glow, spark, blur, smoke, particle, or transparent glow cross from one frame into another.
- The center point of every effect must be exactly at the center of its own 384x512 frame.
- The entire visible VFX, including soft glow and particles, must fit inside a centered 220x220 pixel safe area.
- Leave at least 72 pixels empty padding left/right and 110 pixels empty padding top/bottom in every frame.
- No vertical lines, no dividers, no grid, no labels.

Background:
- Use a perfectly flat solid #00ff00 chroma-key background for background removal.
- One uniform color only.
- No shadows, gradients, textures, checkerboard, reflections, floor plane, or lighting variation.
- Do not use #00ff00 anywhere in the effect.

Style:
[STYLE_DESCRIPTION]

Animation:
[12_FRAME_SEQUENCE]

Quality control:
- Unity vertical slice lines at every 384 pixels must not cut through any visible effect.
- All 12 effects must appear separated with clear empty space between frames.
- Each frame should look clean when cropped alone at 384x512.
```

## Background Prompt

```txt
Create a vertical 2D mobile game battle background for [AREA_NAME].

Scene: [SCENE_DESCRIPTION]
Mood: [MOOD]
Color palette: [COLOR_PALETTE]

Composition:
- portrait mobile layout, 1080x1920
- upper/middle area stays open for enemies
- lower area has clear room for 3 friendly creatures
- very bottom stays clean for future battle UI
- no characters, monsters, UI, text, logo, health bars, slot circles, or arrows

Style:
- polished fantasy mobile game background
- readable on mobile
- supports creature visibility
```

## Unity Import Checklist

Creature sheets:

- Texture Type: Sprite (2D and UI)
- Sprite Mode: Multiple
- Slice Type: Grid By Cell Size
- Pixel Size: `384 x 512`
- Sheet size: `3072 x 512`
- Pivot: keep project default unless a specific controller needs otherwise
- Pixels Per Unit: `256`
- Max Size: at least `4096` for 3072-wide sheets
- Animation sample rate: `8`
- Loop Time: enabled for idle

Static creature sprites:

- Texture Type: Sprite (2D and UI)
- Sprite Mode: Single
- Size: `1024 x 1024`
- Pixels Per Unit: `100` or existing project setting for static display sprites
- Alpha Is Transparency: enabled

## Prep Workflow

After generation:

1. Copy the generated PNG into `tmp/imagegen/`.
2. Remove chroma-key background to real alpha PNG.
3. For sheets, split or extract 8 clean frames.
4. Remove frame labels, grid lines, and stray fragments.
5. Normalize to `3072x512`.
6. Align every frame to the project baseline, lowest visible contact pixel around `y = 376`.
7. Replace the final project PNG while keeping the existing `.png.meta` when Unity references already exist.
8. Verify the ScriptableObject idle/attack frame references still match the `.png.meta` sprite GUID and internal IDs.

Useful baseline check:

```powershell
@'
from PIL import Image
for path in ["Assets/_Project/Art/Creatures/Fire/EmberFox/idle.png"]:
    im = Image.open(path).convert("RGBA")
    for i in range(8):
        frame = im.crop((i*384, 0, (i+1)*384, 512))
        bbox = frame.getchannel("A").getbbox()
        print(i + 1, bbox, "lowest_y", bbox[3] - 1 if bbox else None)
'@ | python -
```
