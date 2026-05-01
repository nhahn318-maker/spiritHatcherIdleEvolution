# Spirit Hatchers - Asset List

## Naming Rules

Use short, stable filenames for generated art assets. Let the folder path carry the creature and element context, and let the filename describe the asset purpose.

Creature folder pattern:

- `Assets/_Project/Art/Creatures/[Element]/[CreatureFolder]/`

Creature image names:

- `form.png` - single-frame base form or enemy/form display sprite.
- `idle.png` - current primary idle animation sheet.
- `attack.png` - current primary melee/basic attack sheet.
- `attack_[skill].png` - named skill attack sheet, for example `attack_seed_spit.png`.
- `[form].png` - evolution form display sprite, for example `flame.png`, `inferno.png`.
- `[form]_idle.png` - evolution form idle sheet, for example `flame_idle.png`, `inferno_idle.png`.
- `idle_legacy.png` - only when an old scene still references an older idle sheet. Remove after references are migrated.

Skill/effect folder pattern:

- `Assets/_Project/Art/Skills/[Element]/`

Skill/effect image names:

- `[skill]_impact.png` - impact effect sheet, for example `ember_bite_impact.png`.
- `[skill]_projectile.png` - projectile sheet.
- `[skill]_trail.png` - trail sheet.

Temporary generation files:

- Do not keep `raw`, `transparent`, `padded`, generated hash names, or prompt-export filenames in final creature folders unless they are actively referenced by Unity.
- After chroma-key removal and normalization, keep only the final `.png` and matching `.png.meta`.
- If a file is referenced by Unity, rename by moving both `.png` and `.png.meta` together so the GUID stays stable.

## 1. Main Style Reference

- Assets/_Project/References/style_reference_home_screen.png

## 2. Creature Sprites

Each creature needs 3 evolution forms.

### Fire Creatures

1. Ember Fox
- Baby Ember Fox - Assets/_Project/Art/Creatures/Fire/EmberFox/baby.png
- Flame Fox - Assets/_Project/Art/Creatures/Fire/EmberFox/flame.png
- Inferno Fox - Assets/_Project/Art/Creatures/Fire/EmberFox/inferno.png

2. Solar Dragon
- Solar Whelp
- Solar Drake
- Solar Dragon

### Water Creatures

3. Aqua Slime
- Tiny Aqua Slime
- Bubble Slime
- Ocean Slime

4. Pearl Turtle
- Tiny Pearl Turtle
- River Pearl Turtle
- Ancient Pearl Turtle

### Nature Creatures

5. Leaf Deer
- Sprout Deer
- Forest Deer
- Ancient Deer

6. Moss Bunny
- Moss Bunny
- Bloom Bunny
- Elder Bloom Bunny

### Shadow Creatures

7. Shade Bat
- Tiny Shade Bat
- Night Bat
- Phantom Bat

8. Moon Cat
- Moon Kitten
- Mystic Moon Cat
- Lunar Spirit Cat

## 3. Resource Icons

- Coin - Assets/_Project/Art/Icons/Resources/gold_coin.png
- Food - Assets/_Project/Art/Icons/Resources/creature_food.png
- Crystal - Assets/_Project/Art/Icons/Resources/magic_crystal.png
- Egg Ticket - Assets/_Project/Art/Icons/Resources/egg_ticket.png

## 4. Element Icons

- Fire icon
- Water icon
- Nature icon
- Shadow icon

## 5. Backgrounds

- Home background - Assets/_Project/Art/Backgrounds/home_background.png
- Hatch background - Assets/_Project/Art/Backgrounds/hatch_background.png
- Expedition background
- Collection background

## 6. UI Assets

- Button background
- Popup panel
- Card frame
- Locked creature silhouette
- Rarity frame: Common
- Rarity frame: Rare
- Rarity frame: Epic

## 7. App Icon

- Baby Ember Fox hatching from magical egg - Assets/_Project/Art/AppIcon/app_icon_baby_ember_fox.png
