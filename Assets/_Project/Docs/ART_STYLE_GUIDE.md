# Spirit Hatchers - Art Style Guide

## 1. Main Style Reference

Use this image as the main visual reference:

Assets/_Project/References/style_reference_home_screen.png

Use it for the overall feeling, not as a strict creature rendering lock:

- Fantasy mobile creature collection
- Expressive collectible creatures
- Cozy magical forest setting
- Bright and colorful UI
- Large readable buttons
- Rounded fantasy UI panels
- Warm lighting
- Soft magical glow
- High contrast between creature, UI, and background
- Portrait mobile layout

## 2. Overall Visual Direction

The game should look like a polished casual mobile creature collection game.

Keywords:

- fantasy creature collection
- expressive creatures
- magical forest
- soft glow
- colorful mobile UI
- rounded buttons
- collectible creature game
- idle evolution game
- cozy fantasy atmosphere

## 3. Creature Style

Creature sprites should be:

- 2D fantasy game assets
- Collectible and expressive
- Clean silhouette
- Polished shading that matches the specific creature reference
- Vibrant element-based colors
- Centered full body
- Transparent background
- Readable at small size
- Suitable for Unity 2D
- Generated on a flat chroma-key background first, then converted to true PNG alpha

Creature sprites may vary by evolution form. A baby form can be cute and rounded, while an evolved form can be sharper, taller, more elegant, or more detailed. Preserve the creature's own reference image when one exists.

Creature battle animation sheets should also follow a shared ground baseline:

- Creatures may have different visible heights and widths.
- Do not force every creature to be the same size.
- For current Ember Fox family sheets, the visible project baseline is around `y = 376` from the top of each 384x512 cell.
- Keep that feet/root/contact point stable across every frame of the same animation.
- Do not visually center a creature in a way that makes it float above or sink below the shared battle ground line.
- If old docs, generated tools, or image prompts mention `y = 472-474`, do not use that for the current Ember Fox-style sheets; it will make the creature appear too low compared with existing `idle.png` and `flame_idle.png`.

Creature sprites should not be:

- Photorealistic unless explicitly requested
- Horror
- Needlessly noisy or unreadable at mobile size
- Dark fantasy
- Semi-transparent
- Cropped
- With text or UI
- With a fake transparent checkerboard baked into the image

Sprite generation note:

- Do not ask image generation tools for "transparent background" unless the tool supports true alpha output.
- Prefer a perfectly flat chroma-key background and remove it after generation.
- Use `#ff00ff` chroma-key for green or nature creatures.
- Use `#00ff00` chroma-key for non-green creatures.
- Always validate the final imported sprite has real alpha, not an RGB image with a checkerboard background.
- For 3072x512 animation sheets, set Texture Importer Max Size to at least `4096` so Unity does not downscale the sheet.
- Use short final filenames in creature folders, such as `form.png`, `idle.png`, `attack.png`, `attack_seed_spit.png`, `flame.png`, and `flame_idle.png`.
- Do not keep `raw`, `transparent`, `padded`, or generated hash filenames in final creature folders unless Unity references them.
- When renaming a Unity-referenced image, move the `.png` and `.png.meta` together so the GUID remains stable.

## 4. Background Style

Backgrounds should be:

- Vertical portrait mobile backgrounds
- 1080x1920
- Fantasy forest or magical hatchery setting
- Soft lighting
- Cozy and colorful
- Have clear empty space for UI and creature display
- No text
- No UI buttons baked into the image
- No main character baked into the image

## 5. UI Style

UI should follow the reference image:

- Large rounded buttons
- Fantasy wooden/golden frames
- High readability
- Big icons
- Soft shadows
- Warm highlights
- Mobile-friendly touch targets
- Bottom navigation with 3 main buttons
- Top resource bar

Main bottom buttons:

- Hatch
- Collection
- Expedition

Top resource bar:

- Coin
- Food
- Crystal
- Egg Ticket

## 6. Color Direction

Use a warm fantasy palette:

- Gold
- Orange
- Brown wood
- Emerald green
- Crystal blue
- Purple magic accent
- Soft dark forest background

Avoid:

- Flat grayscale UI
- Cyberpunk neon
- Realistic muddy colors
- Horror palette

## 7. Screen Layout Direction

The home screen should roughly follow this structure:

Top area:

- Resource bar

Upper middle:

- Game logo or screen title

Middle:

- Main featured creature on a magical nest or platform

Lower middle:

- Creature info card

Bottom:

- Daily reward banner
- Main navigation buttons

## 8. Asset Sizes

Creature sprite:

- 1024x1024
- PNG transparent

Background:

- 1080x1920
- PNG or JPG

Resource icon:

- 512x512
- PNG transparent

App icon:

- 1024x1024
- PNG

UI button/icon:

- 512x512 or scalable Unity UI
