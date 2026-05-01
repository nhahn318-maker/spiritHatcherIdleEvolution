# Spirit Hatchers - Art Style Guide

## 1. Main Style Reference

Use this image as the main visual reference:

Assets/_Project/References/style_reference_home_screen.png

The game should follow the same overall feeling:

- Cute fantasy mobile game
- Chibi creature collection style
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

- cute fantasy
- chibi creature
- magical forest
- soft glow
- colorful mobile UI
- rounded buttons
- collectible creature game
- idle evolution game
- cozy fantasy atmosphere

## 3. Creature Style

Creature sprites should be:

- 2D chibi fantasy
- Cute and collectible
- Big expressive eyes
- Clean silhouette
- Soft shading
- Vibrant element-based colors
- Centered full body
- Transparent background
- Readable at small size
- Suitable for Unity 2D
- Generated on a flat chroma-key background first, then converted to true PNG alpha

Creature battle animation sheets should also follow a shared ground baseline:

- Creatures may have different visible heights and widths.
- Do not force every creature to be the same size.
- For 384x512 animation cells, the lowest feet/root/contact point should sit around `y = 472-474` from the top of the cell.
- Keep that feet/root/contact point stable across every frame of the same animation.
- Do not visually center a creature in a way that makes it float above or sink below the shared battle ground line.

Creature sprites should not be:

- Realistic
- Horror
- Too detailed
- Dark fantasy
- Overly complex
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
