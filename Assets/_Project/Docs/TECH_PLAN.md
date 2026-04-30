# Spirit Hatchers - Technical Plan

## 1. Engine

- Unity 2D
- C#
- Unity UI
- TextMeshPro
- Portrait mobile layout
- Android first

## 2. Project Structure

Assets/_Project/
- Docs/
- References/
- Scripts/
  - Core/
  - Data/
  - Save/
  - Resources/
  - Creature/
  - Hatch/
  - Expedition/
  - DailyReward/
  - Ads/
  - UI/
- ScriptableObjects/
  - Creatures/
  - Expeditions/
- Sprites/
  - Creatures/
  - Backgrounds/
  - Icons/
  - UI/
- Prefabs/
  - UI/
  - Cards/
  - Popups/
- Scenes/

## 3. Main Scene

Use one main scene for the MVP:

- MainScene

Inside MainScene, use different UI panels:

- HomeScreen
- HatchScreen
- CollectionScreen
- CreatureDetailScreen
- ExpeditionScreen
- DailyRewardPopup
- HatchResultPopup

## 4. Data Architecture

Use ScriptableObjects for static data:

- CreatureStaticData
- CreatureDatabase
- ExpeditionData

Use serializable classes for player data:

- PlayerSaveData
- PlayerCreatureData
- PlayerExpeditionData

Use local JSON save:

- Save file path: Application.persistentDataPath
- File name: save_data.json

## 5. Main Managers

- GameManager
- ResourceManager
- SaveManager
- HatchManager
- CreatureProgressionManager
- ExpeditionManager
- DailyRewardManager
- UIManager
- MockRewardedAdService

## 6. Coding Rules

- Keep code beginner-friendly.
- Use clear file names.
- Use serialized fields for Unity references.
- Add null checks.
- Use Debug.Log for important events.
- Do not add real ads in MVP.
- Do not add server code.
- Do not add multiplayer.
- Do not use complex architecture.