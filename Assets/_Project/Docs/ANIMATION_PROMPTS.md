# Spirit Hatchers - Animation Generation Prompts

Use these prompts to generate creature sprites, enemy sprites, battle backgrounds, and animation sprite sheets that match the project style.

## Project Animation Direction Convention

All creature animation sprite sheets must be authored in the same canonical direction:

- Canonical direction: 3/4 view facing right.
- This applies to idle, attack, skill, hurt, victory, and any other creature animation sheet.
- Player-side creatures use the original facing-right sprites.
- Enemy-side creatures are mirrored horizontally by Unity code to face left.
- Do not author separate left-facing creature sheets unless a specific creature has non-mirroring art requirements.
- Do not mix idle facing left with attack facing right. Idle frame 1, idle frame 8, and all attack frames must read as the same right-facing creature.

Add this direction block to every creature animation prompt:

```txt
Facing direction:
- The creature must face right in every frame.
- Use a consistent 3/4 view facing right.
- This is the canonical project direction for all creature animation sheets.
- Enemy-facing direction will be handled in Unity by horizontal mirroring, not by separate art.
- Do not create left-facing, front-facing, back-facing, or mixed-direction frames.

Consistency check:
- Before finalizing, verify that frame 1 and frame 8 face the same direction as all attack frames.
- The creature's eyes, snout/mouth, body angle, tail placement, and action direction must all read as facing right.
```

## Attack-To-Idle Fit Convention

After an idle sheet exists, every attack or skill sheet must be generated as an in-place fit pass against that idle sheet. Unity handles battle movement and enemy mirroring. The art should not fake combat movement by sliding the creature around inside each frame.

Add this alignment block to every attack or skill animation prompt:

```txt
Idle-to-attack alignment:
- Use the provided [CREATURE_NAME] idle facing-right sprite sheet as the strict reference for design, scale, canvas padding, vertical placement, and feet/root pivot.
- This is a Unity animation fit pass, not a redesign.
- Keep the same 8 columns x 1 row layout and same 384x512 cell size as the idle sheet.
- Frame 1 must match idle frame 1 as closely as possible in scale, position, feet/root pivot, and silhouette.
- Frame 8 must return to idle frame 1 scale, position, feet/root pivot, and silhouette so the loop does not pop.
- The feet/root contact point must stay on the same pixel position in every frame as much as possible.
- Do not recenter the creature differently per frame.
- Do not shift the whole creature up, down, left, or right inside the cell.
- Do not move the whole creature forward to fake a lunge; Unity moves melee attackers toward the target.
- Attack motion should come from pose changes, body squash/stretch, head/ear/leaf/tail/mouth movement, and attached effects only.
- Detached projectiles, detached trails, impact effects, or hit flashes should be separate assets unless the prompt explicitly asks for them.
```

## Battle Ground Alignment Convention

Creature sprite sheets do not need to share the same visible height or width. A large creature may be taller or wider than a small creature. The important shared rule is that every creature must stand on the same battle ground baseline.

Use the current Ember Fox and Wild Sprout idle sheets as the baseline references.

- Standard cell size: 384x512.
- Standard sheet size: 3072x512 for 8-frame creature sheets.
- Standard standing baseline: the lowest feet/root/contact point should land around `y = 472-474` from the top of each 384x512 cell.
- The creature may be taller, shorter, wider, or narrower depending on its design.
- Do not force every creature to have the same visible height.
- Do not center by visual mass if that moves the feet/root contact point off the shared ground line.
- The feet/root contact point must stay fixed on the same baseline across every frame of the same sheet.
- Frame 1 and frame 8 must return to the same baseline as the idle sheet.
- Keep the body grounded; do not let the creature float, sink, or slide between frames.

Add this block to every creature idle, attack, skill, hurt, and victory prompt:

```txt
Battle ground alignment:
- The creature may be taller, shorter, wider, or narrower than other creatures if its design requires it.
- Do not force all creatures to have the same visible height or width.
- What must be consistent is the standing baseline.
- In each 384x512 cell, place the lowest feet/root/contact point on the shared battle ground line, approximately y = 472-474 from the top of the cell.
- Keep the same feet/root/contact point across every frame.
- Do not center the creature by visual mass if that moves the feet/root up or down.
- Do not let the creature float, sink, or slide between frames.
```

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

## Wild Sprout - Seed Spit Attack Prompt

Use this to recreate `attack_seed_spit.png`.
Provide `idle.png` from the Wild Sprout folder as the image reference.

```txt
Create a 2D sprite sheet for a cute chibi fantasy creature ranged attack wind-up animation, designed for Unity 2D frame-by-frame animation.

Use the provided Wild Sprout idle facing-right sprite sheet as the strict reference.
The new attack sheet must fit perfectly with that idle sheet in Unity.
This is an in-place ranged wind-up and release-pose sheet only. The projectile and hit effect will be created as separate assets.

Creature: Wild Sprout, Wild Sprout form. A tiny magical sprout creature shaped like a rounded seed bulb with a fresh green sprout growing from the top. It has simple glossy bead-like eyes, no mouth or only a tiny simple dot mouth, tiny root nubs at the base instead of legs, and two small leaf growths on the sides instead of arms. Soft moss patches and subtle seed texture. It should feel like a living magical plant, early-game forest enemy, cute and collectible, not a humanoid character.
Element: Nature.
Skill: Seed Spit, a quick nature ranged attack where the sprout charges a tiny seed in its cheeks/body and releases it forward. Do not include the seed projectile in this sheet.
Color palette: leaf green, emerald green, warm brown seed shell, soft cream highlights, small golden magical glow accents.

Facing direction:
- The creature must face right in every frame.
- Use a consistent 3/4 view facing right.
- This is the canonical project direction for all creature animation sheets.
- Enemy-facing direction will be handled in Unity by horizontal mirroring, not by separate art.
- Do not create left-facing, front-facing, back-facing, or mixed-direction frames.

Idle-to-attack alignment:
- Use the provided Wild Sprout idle facing-right sprite sheet as the strict reference for design, scale, canvas padding, vertical placement, and feet/root pivot.
- This is a Unity animation fit pass, not a redesign.
- Keep the same 8 columns x 1 row layout and same 384x512 cell size as the idle sheet.
- Frame 1 must match idle frame 1 as closely as possible in scale, position, feet/root pivot, and silhouette.
- Frame 8 must return to idle frame 1 scale, position, feet/root pivot, and silhouette so the loop does not pop.
- The root contact point must stay on the same pixel position in every frame as much as possible.
- Do not recenter the creature differently per frame.
- Do not shift the whole creature up, down, left, or right inside the cell.
- Attack motion should come from pose changes, cheek/body squash and stretch, eyes, leaves, sprout stem, root nubs, and a very subtle attached glow only.

Very important:
- Do not draw any projectile.
- Do not draw flying seeds, bullets, seed spit, spit trails, magic orbs, beams, detached sparkles, or impact effects.
- Only animate the creature body, face, eyes, tiny mouth, side leaves, top sprout leaves, root nubs, and a very subtle glow attached directly to the creature.

Critical consistency requirements:
- The character must face the same direction in every frame: 3/4 view facing right.
- Same camera angle in every frame.
- Same character design in every frame.
- Same body proportions in every frame.
- Same scale in every frame.
- Same root pivot position as much as possible.
- Do not rotate the character to different views.
- Do not show front view, back view, or opposite direction.
- This must look like one continuous animation of the exact same creature.

Art style: cute fantasy, chibi creature collection, cozy magical forest feeling, soft shading, vibrant element-based colors, warm lighting, polished casual mobile game asset, readable at small size, suitable for Unity 2D.

Animation sequence:
Frame 1: idle battle stance matching idle frame 1 scale, position, and root pivot, facing right.
Frame 2: slight anticipation, body squashes subtly, side leaves tuck inward, eyes focus to the right, facing right.
Frame 3: stronger charge pose, cheeks/body puff slightly as if gathering seed energy, top sprout bends back a little, attached golden-green glow around the mouth/cheek area only, facing right.
Frame 4: release wind-up, body leans slightly into the action without shifting the whole sprite, side leaves open, tiny mouth begins to open, facing right.
Frame 5: attack release pose, tiny mouth open as if spitting a seed, leaves snapped forward, no projectile visible, no trail visible, facing right.
Frame 6: recoil slightly, body compresses back, top sprout bounces, attached glow fading while still touching the creature, facing right.
Frame 7: return toward idle pose, leaves settle, eyes relax, facing right.
Frame 8: idle battle stance matching idle frame 1 scale, position, and root pivot, facing right.

Sprite sheet requirements:
- 8 frames total
- arranged horizontally in one row
- 8 columns x 1 row
- each frame fits inside a 384x512 cell
- final normalized sheet should be 3072x512 after background removal
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

Consistency check:
- Before finalizing, verify that frame 1 and frame 8 face the same direction as all attack frames.
- Verify that frame 1 and frame 8 overlay cleanly with idle frame 1 without a visible pop in scale, vertical position, or root pivot.
- Verify there are no detached projectiles, trails, beams, or impact effects anywhere in the sheet.

Background: perfectly flat solid #ff00ff chroma-key background for later transparency removal. The background must be one uniform color with no shadows, gradients, texture, reflections, floor plane, checkerboard pattern, or lighting variation. Do not use #ff00ff anywhere in the creature.

Avoid: projectile, seed projectile, bullet, flying seed, spit trail, beam, detached sparkle, detached magic orb, detached attack effect, impact effect, humanoid plant person, human-like arms, human-like legs, changing viewing angle, rotating the creature, changing direction, different character designs between frames, different scale between frames, shifting the whole creature inside the frame, changing root pivot, front view, back view, left-facing frames, dramatic camera movement, action comic panels, separate pose concepts.
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

Facing direction:
- The creature must face right in every frame.
- Use a consistent 3/4 view facing right.
- This is the canonical project direction for all creature animation sheets.
- Enemy-facing direction will be handled in Unity by horizontal mirroring, not by separate art.
- Do not create left-facing, front-facing, back-facing, or mixed-direction frames.

Battle ground alignment:
- The creature may be taller, shorter, wider, or narrower than other creatures if its design requires it.
- Do not force all creatures to have the same visible height or width.
- What must be consistent is the standing baseline.
- In each 384x512 cell, place the lowest feet/root/contact point on the shared battle ground line, approximately y = 472-474 from the top of the cell.
- Keep the same feet/root/contact point across every frame.
- Do not center the creature by visual mass if that moves the feet/root up or down.
- Do not let the creature float, sink, or slide between frames.

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

Consistency check:
- Before finalizing, verify that frame 1 and frame 8 face the same direction as all attack frames.
- The creature's eyes, snout/mouth, body angle, tail placement, and action direction must all read as facing right.

Background: perfectly flat solid #00ff00 chroma-key background for later transparency removal. The background must be one uniform color with no shadows, gradients, texture, reflections, floor plane, or lighting variation. Do not use #00ff00 anywhere in the creature.

Avoid: left-facing frames, front view, back view, mixed-direction frames, no text, no UI, no watermark, no cast shadow, no contact shadow, no extra characters, no frame labels, no grid lines.
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
- `[IDLE_REFERENCE_DESCRIPTION]` such as `the provided idle facing-right sprite sheet`
- `[CHROMA_KEY_COLOR]`

```txt
Create a 2D sprite sheet for a cute chibi fantasy creature melee attack animation, designed for Unity 2D frame-by-frame animation.

Use [IDLE_REFERENCE_DESCRIPTION] as the strict reference.
The new attack sheet must fit perfectly with that idle sheet in Unity.
This is an in-place attack animation sheet. Unity will move the melee attacker toward the target during battle.

Creature: [CREATURE_NAME], [FORM_NAME]. [CREATURE_DESCRIPTION]
Element: [ELEMENT].
Skill: [SKILL_NAME], [SKILL_DESCRIPTION].
Color palette: [COLOR_PALETTE].

Facing direction:
- The creature must face right in every frame.
- Use a consistent 3/4 view facing right.
- This is the canonical project direction for all creature animation sheets.
- Enemy-facing direction will be handled in Unity by horizontal mirroring, not by separate art.
- Do not create left-facing, front-facing, back-facing, or mixed-direction frames.

Critical alignment requirements:
- This attack sheet must align perfectly with the provided idle facing-right sheet.
- Use the same 8 columns x 1 row layout.
- Each frame must fit inside a 384x512 cell.
- Use the same creature scale as the idle sheet.
- Use the same vertical position as the idle sheet.
- Use the same feet/root contact point as idle frame 1.
- The creature may become visually taller or wider during the attack pose, but the feet/root contact point must remain on the idle baseline.
- Frame 1 must match idle frame 1 as closely as possible in scale, position, feet/root pivot, and silhouette.
- Frame 8 must return to the same scale, position, and feet/root pivot as idle frame 1.
- The feet/root contact point should stay on the same pixel position in every frame as much as possible.
- Do not shift the entire creature up, down, left, or right inside the cell.
- Do not move the whole creature forward to fake the melee approach.
- Do not change canvas padding compared with the idle sheet.
- Attack motion should come from pose changes, head/ear/body movement, mouth movement, tail movement, and attached effects, not from moving the whole creature around inside the frame.

Battle ground alignment:
- The creature may be taller, shorter, wider, or narrower than other creatures if its design requires it.
- Do not force all creatures to have the same visible height or width.
- What must be consistent is the standing baseline.
- In each 384x512 cell, place the lowest feet/root/contact point on the shared battle ground line, approximately y = 472-474 from the top of the cell.
- Keep the same feet/root/contact point across every frame.
- Do not center the creature by visual mass if that moves the feet/root up or down.
- Do not let the creature float, sink, or slide between frames.

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

Art style: cute fantasy, chibi creature collection, cozy magical forest feeling, soft shading, vibrant element-based colors, warm lighting, soft magical glow, polished casual mobile game asset, readable at small size, suitable for Unity 2D.

Animation sequence:
Frame 1: idle battle stance matching idle frame 1 scale, position, and feet/root pivot, facing right.
Frame 2: slight crouch anticipation, facing right.
Frame 3: lean forward, attack ready, facing right.
Frame 4: short in-place melee strike pose, small element spark near attack point, feet/root pivot unchanged, facing right.
Frame 5: impact pose, tiny element spark, facing right.
Frame 6: recoil backward slightly, facing right.
Frame 7: return toward idle pose, facing right.
Frame 8: idle battle stance matching idle frame 1 scale, position, and feet/root pivot, facing right.

Sprite sheet requirements:
- 8 frames total
- arranged horizontally in one row
- 8 columns x 1 row
- each frame fits inside a 384x512 cell
- final normalized sheet should be 3072x512 after background removal
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

Consistency check:
- Before finalizing, verify that frame 1 and frame 8 face the same direction as all attack frames.
- The creature's eyes, snout/mouth, body angle, tail placement, and action direction must all read as facing right.
- Verify that frame 1 and frame 8 overlay cleanly with idle frame 1 without a visible pop in scale, vertical position, or feet/root pivot.

Background: perfectly flat solid [CHROMA_KEY_COLOR] chroma-key background for later transparency removal. The background must be one uniform color with no shadows, gradients, texture, reflections, floor plane, checkerboard pattern, or lighting variation. Do not use [CHROMA_KEY_COLOR] anywhere in the creature.

Avoid: left-facing frames, changing viewing angle, rotating the creature, changing direction, different character designs between frames, different scale between frames, shifting the whole creature inside the frame, changing feet/root pivot, front view, back view, opposite-facing frames, dramatic camera movement, action comic panels, separate pose concepts, horror, blood.
```

## Ranged Attack Or Skill Wind-Up Sprite Sheet

Use this when the creature performs a ranged skill, but the projectile, beam, trail, hit flash, or impact effect will be created as a separate asset.

Replace the bracketed values before generating:

- `[CREATURE_NAME]`
- `[FORM_NAME]`
- `[CREATURE_DESCRIPTION]`
- `[ELEMENT]`
- `[SKILL_NAME]`
- `[SKILL_DESCRIPTION]`
- `[CHARGE_BODY_PARTS]` such as `mouth, cheeks, leaves, forehead`
- `[RELEASE_POSE]` such as `mouth open as if spitting`
- `[COLOR_PALETTE]`
- `[IDLE_REFERENCE_DESCRIPTION]` such as `the provided idle facing-right sprite sheet`
- `[CHROMA_KEY_COLOR]`

```txt
Create a 2D sprite sheet for a cute chibi fantasy creature ranged attack wind-up animation, designed for Unity 2D frame-by-frame animation.

Use [IDLE_REFERENCE_DESCRIPTION] as the strict reference.
The new attack sheet must fit perfectly with that idle sheet in Unity.
This is an in-place ranged wind-up and release-pose sheet only. The projectile and hit effect will be created as separate assets.

Creature: [CREATURE_NAME], [FORM_NAME]. [CREATURE_DESCRIPTION]
Element: [ELEMENT].
Skill: [SKILL_NAME], [SKILL_DESCRIPTION].
Color palette: [COLOR_PALETTE].

Facing direction:
- The creature must face right in every frame.
- Use a consistent 3/4 view facing right.
- This is the canonical project direction for all creature animation sheets.
- Enemy-facing direction will be handled in Unity by horizontal mirroring, not by separate art.
- Do not create left-facing, front-facing, back-facing, or mixed-direction frames.

Idle-to-attack alignment:
- Use the provided [CREATURE_NAME] idle facing-right sprite sheet as the strict reference for design, scale, canvas padding, vertical placement, and feet/root pivot.
- This is a Unity animation fit pass, not a redesign.
- Keep the same 8 columns x 1 row layout and same 384x512 cell size as the idle sheet.
- Frame 1 must match idle frame 1 as closely as possible in scale, position, feet/root pivot, and silhouette.
- Frame 8 must return to idle frame 1 scale, position, feet/root pivot, and silhouette so the loop does not pop.
- The feet/root contact point must stay on the same pixel position in every frame as much as possible.
- The creature may become visually taller or wider during the attack pose, but the feet/root contact point must remain on the idle baseline.
- Do not recenter the creature differently per frame.
- Do not shift the whole creature up, down, left, or right inside the cell.
- Attack motion should come from pose changes, [CHARGE_BODY_PARTS] movement, and very subtle attached glow only.

Battle ground alignment:
- The creature may be taller, shorter, wider, or narrower than other creatures if its design requires it.
- Do not force all creatures to have the same visible height or width.
- What must be consistent is the standing baseline.
- In each 384x512 cell, place the lowest feet/root/contact point on the shared battle ground line, approximately y = 472-474 from the top of the cell.
- Keep the same feet/root/contact point across every frame.
- Do not center the creature by visual mass if that moves the feet/root up or down.
- Do not let the creature float, sink, or slide between frames.

Very important:
- Do not draw any projectile.
- Do not draw flying bullets, seeds, fireballs, water drops, rocks, beams, or detached skill objects.
- Do not draw spit trails, motion trails, laser beams, detached sparkles, detached magic orbs, or impact effects.
- Only animate the creature body, face, mouth, eyes, limbs, leaves, ears, tail, and a very subtle glow attached directly to the creature.

Critical consistency requirements:
- The character must face the same direction in every frame: 3/4 view facing right.
- Same camera angle in every frame.
- Same character design in every frame.
- Same body proportions in every frame.
- Same scale in every frame.
- Same feet/root pivot position as much as possible.
- Do not rotate the character to different views.
- Do not show front view, back view, or opposite direction.
- This must look like one continuous animation of the exact same creature.

Art style: cute fantasy, chibi creature collection, cozy magical forest feeling, soft shading, vibrant element-based colors, warm lighting, polished casual mobile game asset, readable at small size, suitable for Unity 2D.

Animation sequence:
Frame 1: idle battle stance matching idle frame 1 scale, position, and feet/root pivot, facing right.
Frame 2: slight anticipation, [CHARGE_BODY_PARTS] begin charging, facing right.
Frame 3: stronger charge pose, eyes focused, [CHARGE_BODY_PARTS] fully charged, facing right.
Frame 4: release wind-up, body leans slightly into the action without shifting the whole sprite, facing right.
Frame 5: attack release pose, [RELEASE_POSE], no projectile visible, no trail visible, facing right.
Frame 6: recoil slightly, charge fading while still attached to the creature, facing right.
Frame 7: return toward idle pose, facing right.
Frame 8: idle battle stance matching idle frame 1 scale, position, and feet/root pivot, facing right.

Sprite sheet requirements:
- 8 frames total
- arranged horizontally in one row
- 8 columns x 1 row
- each frame fits inside a 384x512 cell
- final normalized sheet should be 3072x512 after background removal
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

Consistency check:
- Before finalizing, verify that frame 1 and frame 8 face the same direction as all attack frames.
- Verify that frame 1 and frame 8 overlay cleanly with idle frame 1 without a visible pop in scale, vertical position, or feet/root pivot.
- Verify there are no detached projectiles, trails, beams, or impact effects anywhere in the sheet.

Background: perfectly flat solid [CHROMA_KEY_COLOR] chroma-key background for later transparency removal. The background must be one uniform color with no shadows, gradients, texture, reflections, floor plane, checkerboard pattern, or lighting variation. Do not use [CHROMA_KEY_COLOR] anywhere in the creature.

Avoid: projectile, seed projectile, bullet, flying seed, fireball, spit trail, beam, detached sparkle, detached magic orb, detached attack effect, impact effect, changing viewing angle, rotating the creature, changing direction, different character designs between frames, different scale between frames, shifting the whole creature inside the frame, changing feet/root pivot, front view, back view, left-facing frames, dramatic camera movement, action comic panels, separate pose concepts.
```

## Ember Fox - Baby Melee Attack Example

```txt
Create a 2D sprite sheet for a cute chibi fantasy creature melee attack animation, designed for Unity 2D frame-by-frame animation.

Use the provided Ember Fox idle facing-right sprite sheet as the strict reference.
The new attack sheet must fit perfectly with that idle sheet in Unity.
This is an in-place attack animation sheet. Unity will move Ember Fox toward the target during battle.

Creature: Ember Fox, Baby Ember Fox form. A small orange fire fox with big expressive eyes, flame-like ears, glowing flame tail, warm orange and gold colors.
Element: Fire.
Skill: Ember Bite, a close-range bite attack.
Color palette: warm orange, golden yellow, cream fur, soft red flame accents.

Facing direction:
- The creature must face right in every frame.
- Use a consistent 3/4 view facing right.
- This is the canonical project direction for all creature animation sheets.
- Enemy-facing direction will be handled in Unity by horizontal mirroring, not by separate art.
- Do not create left-facing, front-facing, back-facing, or mixed-direction frames.

Critical alignment requirements:
- This attack sheet must align perfectly with the provided idle facing-right sheet.
- Use the same 8 columns x 1 row layout.
- Each frame must fit inside a 384x512 cell.
- Use the same creature scale as the idle sheet.
- Use the same vertical position as the idle sheet.
- Use the same feet/root contact point as idle frame 1.
- Frame 1 must match idle frame 1 as closely as possible in scale, position, feet/root pivot, and silhouette.
- Frame 8 must return to the same scale, position, and feet/root pivot as idle frame 1.
- The feet/root contact point should stay on the same pixel position in every frame as much as possible.
- Do not shift the entire creature up, down, left, or right inside the cell.
- Do not move the whole creature forward to fake the melee approach.
- Do not change canvas padding compared with the idle sheet.
- Attack motion should come from pose changes, head/ear/body movement, mouth movement, tail flame motion, and attached fire sparks, not from moving the whole creature around inside the frame.

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
Frame 1: idle battle stance matching idle frame 1 scale, position, and feet/root pivot, facing right.
Frame 2: slight crouch anticipation, facing right.
Frame 3: lean forward, mouth slightly open, facing right.
Frame 4: short in-place bite pose, small fire spark near mouth, feet/root pivot unchanged, facing right.
Frame 5: bite impact pose, tiny fire spark, facing right.
Frame 6: recoil backward slightly, facing right.
Frame 7: return toward idle pose, facing right.
Frame 8: idle battle stance matching idle frame 1 scale, position, and feet/root pivot, facing right.

Sprite sheet requirements:
- 8 frames total
- arranged horizontally in one row
- 8 columns x 1 row
- each frame fits inside a 384x512 cell
- final normalized sheet should be 3072x512 after background removal
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

Consistency check:
- Before finalizing, verify that frame 1 and frame 8 face the same direction as all attack frames.
- The creature's eyes, snout/mouth, body angle, tail placement, and action direction must all read as facing right.
- Verify that frame 1 and frame 8 overlay cleanly with idle frame 1 without a visible pop in scale, vertical position, or feet/root pivot.

Background: perfectly flat solid #00ff00 chroma-key background for later transparency removal. The background must be one uniform color with no shadows, gradients, texture, reflections, floor plane, checkerboard pattern, or lighting variation. Do not use #00ff00 anywhere in the creature.

Avoid: changing viewing angle, rotating the creature, changing direction, different character designs between frames, different scale between frames, shifting the whole creature inside the frame, changing feet/root pivot, front view, back view, left-facing frames, dramatic camera movement, action comic panels, separate pose concepts, horror, blood.
```

## Ember Fox - Baby Idle Example

```txt
Create a 2D sprite sheet for a cute chibi fantasy creature idle animation, matching a cozy mobile creature collection game style.

Creature: Ember Fox, Baby Ember Fox form. A baby orange fire fox with big expressive eyes, tiny body, soft rounded shapes, flame-like ears, a glowing flame tail, warm orange and gold colors, and a clean readable silhouette.
Element: Fire.
Color palette: warm orange, golden yellow, cream fur, soft red flame accents.

Facing direction:
- The creature must face right in every frame.
- Use a consistent 3/4 view facing right.
- This is the canonical project direction for all creature animation sheets.
- Enemy-facing direction will be handled in Unity by horizontal mirroring, not by separate art.
- Do not create left-facing, front-facing, back-facing, or mixed-direction frames.

Art style: cute fantasy, chibi creature collection, cozy magical forest feeling, soft shading, vibrant element-based colors, warm lighting, soft magical glow, polished casual mobile game asset, readable at small size, suitable for Unity 2D.

Animation: idle breathing loop with very subtle body bounce, ears gently moving, and tail flame softly flickering. Keep the pose calm and cute, not action-based. The idle direction must match Ember Fox attack sheets: 3/4 view facing right.

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

Consistency check:
- Before finalizing, verify that frame 1 and frame 8 face the same direction as all attack frames.
- The creature's eyes, snout/mouth, body angle, tail placement, and action direction must all read as facing right.

Background: perfectly flat solid #00ff00 chroma-key background for later transparency removal. The background must be one uniform color with no shadows, gradients, texture, reflections, floor plane, or lighting variation. Do not use #00ff00 anywhere in the creature.

Avoid: left-facing frames, front view, back view, mixed-direction frames, no text, no UI, no watermark, no cast shadow, no contact shadow, no extra characters, no frame labels, no grid lines.
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
  --name "flame_idle"
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
