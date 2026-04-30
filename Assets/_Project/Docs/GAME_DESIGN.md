# Spirit Hatchers: Idle Evolution - Game Design Document

## 1. Game Overview

Spirit Hatchers: Idle Evolution is a 2D portrait mobile idle creature collection game.

The player hatches magical creatures from eggs, collects different elemental spirits, sends them on expeditions to earn resources, upgrades them, and evolves them through multiple visual forms.

## 2. Target Platform

- Android first
- Unity 2D
- Portrait screen
- Offline single-player
- Local save only
- No server
- No multiplayer

## 3. Core Gameplay Loop

Hatch egg  
→ Get creature  
→ Send creature on expedition  
→ Earn resources  
→ Upgrade creature  
→ Evolve creature  
→ Unlock more creatures  
→ Repeat

## 4. MVP Features

The first playable version should include:

- Home screen
- Hatch screen
- Collection screen
- Creature detail screen
- Expedition screen
- Resource system
- Hatch/gacha system
- Creature upgrade system
- Creature evolution system
- Local JSON save/load
- Daily reward
- Mock rewarded ads

## 5. Not Included in MVP

Do not include these features in the first version:

- Multiplayer
- PvP
- Server login
- Real-money gacha
- Online leaderboard
- Complex combat
- Battle pass
- Guild system
- 3D model system

## 6. Resources

The game has four resources:

### Coin

Used for upgrading creatures.

### Food

Used for evolving creatures.

### Crystal

Used for hatching and higher evolution requirements.

### Egg Ticket

Used for free hatching.

## 7. Creature System

Each creature has:

- Creature ID
- Creature name
- Element
- Rarity
- Base power
- Current level
- Current form
- Sprite for each evolution form

## 8. Elements

The MVP has four elements:

- Fire
- Water
- Nature
- Shadow

## 9. Rarity

The MVP has three rarity levels:

- Common
- Rare
- Epic

## 10. Hatch System

The player can hatch a creature by spending:

- 1 Egg Ticket if available
- Otherwise, 10 Crystal

Rarity rates:

- Common: 70%
- Rare: 25%
- Epic: 5%

Duplicate rewards:

- Common duplicate: 50 Food
- Rare duplicate: 100 Food + 5 Crystal
- Epic duplicate: 200 Food + 15 Crystal

## 11. Evolution System

Each creature has three forms:

- Form 1: Baby form
- Form 2: Advanced form
- Form 3: Final form

Evolution requirements:

Form 1 to Form 2:

- Level 10
- 100 Food

Form 2 to Form 3:

- Level 25
- 300 Food
- 20 Crystal

## 12. Upgrade System

Creature upgrade cost:

Upgrade Cost = 20 + Level * 10

Each upgrade increases creature level by 1.

Power formula:

Power = Base Power * Form Multiplier + Level * 5

## 13. Expedition System

The player can send one owned creature to an expedition.

Expeditions:

### Forest Walk

- Duration: 5 minutes
- Reward: Coin + Food

### Crystal Cave

- Duration: 30 minutes
- Reward: Coin + Food + Crystal

### Shadow Ruins

- Duration: 2 hours
- Reward: Coin + Food + Crystal

Expeditions continue while the app is closed.

## 14. Daily Reward

The player can claim once per day:

- 1 Egg Ticket
- 100 Coin
- 50 Food

## 15. Monetization Plan

MVP only uses mock rewarded ads.

Future monetization:

- Rewarded ad for extra Egg Ticket
- Rewarded ad for double expedition reward
- Remove Ads purchase
- Cosmetic skin packs

No paid random gacha in the MVP.