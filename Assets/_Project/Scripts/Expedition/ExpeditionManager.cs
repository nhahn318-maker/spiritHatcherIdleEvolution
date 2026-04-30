using System;
using System.Collections.Generic;
using UnityEngine;
using SpiritHatchers.Core;
using SpiritHatchers.Creature;
using SpiritHatchers.Data;
using SpiritHatchers.Resources;
using SpiritHatchers.Save;

namespace SpiritHatchers.Expedition
{
    public class ExpeditionManager : MonoBehaviour
    {
        public static ExpeditionManager Instance { get; private set; }

        public event Action OnExpeditionsChanged;

        [Header("References")]
        [SerializeField] private GameManager gameManager;
        [SerializeField] private ResourceManager resourceManager;
        [SerializeField] private CreatureDatabase creatureDatabase;
        [SerializeField] private CreatureProgressionManager progressionManager;

        [Header("Expeditions")]
        [SerializeField] private List<ExpeditionData> expeditions = new List<ExpeditionData>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning("Duplicate ExpeditionManager found. Destroying the new one.");
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void Start()
        {
            FindReferencesIfNeeded();
            EnsureSaveList();
        }

        public List<ExpeditionData> GetAllExpeditions()
        {
            return new List<ExpeditionData>(expeditions);
        }

        public PlayerExpeditionData GetActiveExpeditionForExpedition(string expeditionId)
        {
            PlayerSaveData saveData = GetSaveData();
            if (saveData == null || string.IsNullOrEmpty(expeditionId))
            {
                return null;
            }

            for (int i = 0; i < saveData.activeExpeditions.Count; i++)
            {
                PlayerExpeditionData active = saveData.activeExpeditions[i];

                if (active != null && active.expeditionId == expeditionId)
                {
                    return active;
                }
            }

            return null;
        }

        public bool IsCreatureBusy(string creatureInstanceId)
        {
            PlayerSaveData saveData = GetSaveData();
            if (saveData == null || string.IsNullOrEmpty(creatureInstanceId))
            {
                return false;
            }

            for (int i = 0; i < saveData.activeExpeditions.Count; i++)
            {
                PlayerExpeditionData active = saveData.activeExpeditions[i];

                if (active != null && active.creatureInstanceId == creatureInstanceId)
                {
                    return true;
                }
            }

            return false;
        }

        public bool StartExpedition(ExpeditionData expedition, PlayerCreatureData creature)
        {
            FindReferencesIfNeeded();

            PlayerSaveData saveData = GetSaveData();
            if (saveData == null)
            {
                Debug.LogWarning("Cannot start expedition because save data is missing.");
                return false;
            }

            if (expedition == null || string.IsNullOrEmpty(expedition.expeditionId))
            {
                Debug.LogWarning("Cannot start expedition because expedition data is missing.");
                return false;
            }

            if (creature == null || string.IsNullOrEmpty(creature.uniqueInstanceId))
            {
                Debug.LogWarning("Cannot start expedition because creature data is missing.");
                return false;
            }

            if (GetActiveExpeditionForExpedition(expedition.expeditionId) != null)
            {
                Debug.Log($"Cannot start {expedition.expeditionName}; it already has an active run.");
                return false;
            }

            if (IsCreatureBusy(creature.uniqueInstanceId))
            {
                Debug.Log($"Cannot start expedition; creature {creature.creatureId} is already busy.");
                return false;
            }

            if (GetOwnedCreatureByInstanceId(creature.uniqueInstanceId) == null)
            {
                Debug.LogWarning("Cannot start expedition because the selected creature is not owned by the player.");
                return false;
            }

            DateTime startTime = DateTime.UtcNow;
            DateTime endTime = startTime.AddSeconds(Mathf.Max(1, expedition.durationSeconds));

            PlayerExpeditionData activeExpedition = new PlayerExpeditionData
            {
                expeditionId = expedition.expeditionId,
                creatureInstanceId = creature.uniqueInstanceId,
                startTime = startTime.ToString("o"),
                endTime = endTime.ToString("o")
            };

            saveData.activeExpeditions.Add(activeExpedition);
            SaveAndNotify();

            Debug.Log($"Started expedition {expedition.expeditionName} with {creature.creatureId}. Ends at {activeExpedition.endTime} UTC.");
            return true;
        }

        public bool IsComplete(PlayerExpeditionData activeExpedition)
        {
            if (activeExpedition == null)
            {
                return false;
            }

            if (!TryParseUtc(activeExpedition.endTime, out DateTime endTime))
            {
                return false;
            }

            return DateTime.UtcNow >= endTime;
        }

        public TimeSpan GetRemainingTime(PlayerExpeditionData activeExpedition)
        {
            if (activeExpedition == null || !TryParseUtc(activeExpedition.endTime, out DateTime endTime))
            {
                return TimeSpan.Zero;
            }

            TimeSpan remaining = endTime - DateTime.UtcNow;
            return remaining.TotalSeconds > 0 ? remaining : TimeSpan.Zero;
        }

        public bool ClaimReward(PlayerExpeditionData activeExpedition, out ExpeditionReward reward)
        {
            return ClaimReward(activeExpedition, out reward, out ExpeditionBattleResult battleResult);
        }

        public bool ClaimReward(PlayerExpeditionData activeExpedition, out ExpeditionReward reward, out ExpeditionBattleResult battleResult)
        {
            FindReferencesIfNeeded();
            reward = new ExpeditionReward();
            battleResult = new ExpeditionBattleResult();

            PlayerSaveData saveData = GetSaveData();
            if (saveData == null || activeExpedition == null)
            {
                Debug.LogWarning("Cannot claim expedition because save data or active expedition is missing.");
                return false;
            }

            if (!IsComplete(activeExpedition))
            {
                Debug.Log("Cannot claim expedition reward yet; expedition is still running.");
                return false;
            }

            ExpeditionData expedition = GetExpeditionById(activeExpedition.expeditionId);
            PlayerCreatureData creature = GetOwnedCreatureByInstanceId(activeExpedition.creatureInstanceId);

            if (expedition == null || creature == null)
            {
                Debug.LogWarning("Cannot claim expedition because expedition or creature data is missing.");
                return false;
            }

            if (resourceManager == null)
            {
                Debug.LogWarning("Cannot claim expedition because ResourceManager is missing.");
                return false;
            }

            battleResult = ResolveBattle(expedition, creature);
            reward = CalculateReward(expedition, creature, battleResult);

            saveData.activeExpeditions.Remove(activeExpedition);
            resourceManager.AddResources(reward.coin, reward.food, reward.crystal, 0);
            SaveAndNotify();

            Debug.Log($"Claimed {expedition.expeditionName}: {battleResult.resultLabel}. {reward.coin} Coin, {reward.food} Food, {reward.crystal} Crystal.");
            return true;
        }

        public ExpeditionReward CalculateReward(ExpeditionData expedition, PlayerCreatureData creature)
        {
            return CalculateReward(expedition, creature, null);
        }

        public ExpeditionReward CalculateReward(ExpeditionData expedition, PlayerCreatureData creature, ExpeditionBattleResult battleResult)
        {
            ExpeditionReward reward = new ExpeditionReward();

            if (expedition == null || creature == null)
            {
                return reward;
            }

            CreatureStaticData creatureStaticData = creatureDatabase != null
                ? creatureDatabase.GetCreatureById(creature.creatureId)
                : null;

            int creaturePower = progressionManager != null
                ? progressionManager.CalculatePower(creatureStaticData, creature)
                : CalculatePowerFallback(creatureStaticData, creature);

            int creatureLevel = Mathf.Max(1, creature.level);
            float clearMultiplier = battleResult != null ? battleResult.rewardMultiplier : 1f;

            reward.coin = Mathf.RoundToInt((expedition.baseCoin + creaturePower * 2) * clearMultiplier);
            reward.food = Mathf.RoundToInt((expedition.baseFood + creatureLevel * 5) * clearMultiplier);
            reward.crystal = Mathf.RoundToInt(expedition.baseCrystal * clearMultiplier);

            return reward;
        }

        public ExpeditionBattleResult PreviewBattle(ExpeditionData expedition, PlayerCreatureData creature)
        {
            return ResolveBattle(expedition, creature);
        }

        public int GetCreaturePower(PlayerCreatureData creature)
        {
            FindReferencesIfNeeded();

            if (creature == null)
            {
                return 0;
            }

            CreatureStaticData creatureStaticData = creatureDatabase != null
                ? creatureDatabase.GetCreatureById(creature.creatureId)
                : null;

            return progressionManager != null
                ? progressionManager.CalculatePower(creatureStaticData, creature)
                : CalculatePowerFallback(creatureStaticData, creature);
        }

        public ExpeditionData GetExpeditionById(string expeditionId)
        {
            if (string.IsNullOrEmpty(expeditionId))
            {
                return null;
            }

            for (int i = 0; i < expeditions.Count; i++)
            {
                ExpeditionData expedition = expeditions[i];

                if (expedition != null && expedition.expeditionId == expeditionId)
                {
                    return expedition;
                }
            }

            return null;
        }

        public PlayerCreatureData GetOwnedCreatureByInstanceId(string creatureInstanceId)
        {
            PlayerSaveData saveData = GetSaveData();
            if (saveData == null || string.IsNullOrEmpty(creatureInstanceId))
            {
                return null;
            }

            for (int i = 0; i < saveData.ownedCreatures.Count; i++)
            {
                PlayerCreatureData creature = saveData.ownedCreatures[i];

                if (creature != null && creature.uniqueInstanceId == creatureInstanceId)
                {
                    return creature;
                }
            }

            return null;
        }

        public List<PlayerCreatureData> GetAvailableOwnedCreatures()
        {
            List<PlayerCreatureData> availableCreatures = new List<PlayerCreatureData>();
            PlayerSaveData saveData = GetSaveData();

            if (saveData == null)
            {
                return availableCreatures;
            }

            for (int i = 0; i < saveData.ownedCreatures.Count; i++)
            {
                PlayerCreatureData creature = saveData.ownedCreatures[i];

                if (creature != null && !IsCreatureBusy(creature.uniqueInstanceId))
                {
                    availableCreatures.Add(creature);
                }
            }

            return availableCreatures;
        }

        [ContextMenu("Debug/Complete All Active Expeditions")]
        public void DebugCompleteAllActiveExpeditions()
        {
            PlayerSaveData saveData = GetSaveData();
            if (saveData == null)
            {
                Debug.LogWarning("Cannot complete expeditions because save data is missing.");
                return;
            }

            saveData.EnsureListsAreValid();
            string completedTime = DateTime.UtcNow.AddSeconds(-1).ToString("o");

            for (int i = 0; i < saveData.activeExpeditions.Count; i++)
            {
                PlayerExpeditionData active = saveData.activeExpeditions[i];
                if (active != null)
                {
                    active.endTime = completedTime;
                }
            }

            SaveAndNotify();
            Debug.Log($"Debug completed {saveData.activeExpeditions.Count} active expedition(s).");
        }

        [ContextMenu("Debug/Clear Active Expeditions")]
        public void DebugClearActiveExpeditions()
        {
            PlayerSaveData saveData = GetSaveData();
            if (saveData == null)
            {
                Debug.LogWarning("Cannot clear expeditions because save data is missing.");
                return;
            }

            saveData.EnsureListsAreValid();
            int clearedCount = saveData.activeExpeditions.Count;
            saveData.activeExpeditions.Clear();

            SaveAndNotify();
            Debug.Log($"Debug cleared {clearedCount} active expedition(s).");
        }

        private PlayerSaveData GetSaveData()
        {
            FindReferencesIfNeeded();

            if (gameManager == null || gameManager.CurrentSaveData == null)
            {
                return null;
            }

            gameManager.CurrentSaveData.EnsureListsAreValid();
            return gameManager.CurrentSaveData;
        }

        private int CalculatePowerFallback(CreatureStaticData creatureData, PlayerCreatureData creature)
        {
            if (creatureData == null || creature == null)
            {
                return 0;
            }

            CreatureFormData form = creatureData.GetFormByIndex(creature.currentFormIndex);
            float multiplier = form != null ? Mathf.Max(0f, form.powerMultiplier) : 1f;
            return Mathf.RoundToInt(creatureData.basePower * multiplier + Mathf.Max(1, creature.level) * 5);
        }

        private ExpeditionBattleResult ResolveBattle(ExpeditionData expedition, PlayerCreatureData creature)
        {
            ExpeditionBattleResult result = new ExpeditionBattleResult();

            if (expedition == null || creature == null)
            {
                result.resultLabel = "No Battle";
                result.rewardMultiplier = 0f;
                return result;
            }

            CreatureStaticData creatureStaticData = creatureDatabase != null
                ? creatureDatabase.GetCreatureById(creature.creatureId)
                : null;

            int creaturePower = progressionManager != null
                ? progressionManager.CalculatePower(creatureStaticData, creature)
                : CalculatePowerFallback(creatureStaticData, creature);

            CreatureElement creatureElement = creatureStaticData != null ? creatureStaticData.element : CreatureElement.Fire;
            string creatureName = creatureStaticData != null ? creatureStaticData.creatureName : creature.creatureId;
            string skillName = creatureStaticData != null && !string.IsNullOrEmpty(creatureStaticData.skillName)
                ? creatureStaticData.skillName
                : "Spirit Strike";
            float skillMultiplier = creatureStaticData != null ? Mathf.Max(0.1f, creatureStaticData.skillPowerMultiplier) : 1f;

            int creatureHealth = Mathf.Max(30, creaturePower * 3);
            int maxCreatureHealth = creatureHealth;
            result.totalEnemies = expedition.enemies != null ? expedition.enemies.Count : 0;

            if (result.totalEnemies == 0)
            {
                result.enemiesDefeated = 0;
                result.resultLabel = "Clear";
                result.rewardMultiplier = 1f;
                result.logLines.Add($"{creatureName} explored safely. No enemies appeared.");
                return result;
            }

            for (int i = 0; i < expedition.enemies.Count; i++)
            {
                ExpeditionEnemyData enemy = expedition.enemies[i];
                if (enemy == null)
                {
                    continue;
                }

                string enemyName = string.IsNullOrEmpty(enemy.enemyName) ? "Unknown Enemy" : enemy.enemyName;
                int enemyHealth = Mathf.Max(1, enemy.maxHealth);
                int round = 0;

                result.logLines.Add($"Encounter {i + 1}: {enemyName} appeared.");

                while (creatureHealth > 0 && enemyHealth > 0 && round < 8)
                {
                    round++;
                    int damage = CalculateSkillDamage(creaturePower, skillMultiplier, creatureElement, enemy.element, enemy.defense);
                    enemyHealth -= damage;
                    result.logLines.Add($"{creatureName} used {skillName} for {damage} damage.");

                    if (enemyHealth <= 0)
                    {
                        break;
                    }

                    int counterDamage = Mathf.Max(1, enemy.attack + enemy.maxHealth / 8 - creaturePower / 12);
                    creatureHealth -= counterDamage;
                    result.logLines.Add($"{enemyName} countered for {counterDamage} damage.");
                }

                if (enemyHealth > 0)
                {
                    result.logLines.Add($"{creatureName} retreated from {enemyName}.");
                    break;
                }

                result.enemiesDefeated++;
                result.logLines.Add($"{enemyName} defeated.");
            }

            result.creatureRemainingHealthPercent = maxCreatureHealth > 0
                ? Mathf.Clamp01((float)creatureHealth / maxCreatureHealth)
                : 0f;
            result.rewardMultiplier = CalculateBattleRewardMultiplier(result.enemiesDefeated, result.totalEnemies);
            result.resultLabel = BuildBattleResultLabel(result.enemiesDefeated, result.totalEnemies, result.creatureRemainingHealthPercent);
            return result;
        }

        private int CalculateSkillDamage(
            int creaturePower,
            float skillMultiplier,
            CreatureElement attackerElement,
            CreatureElement defenderElement,
            int enemyDefense)
        {
            float elementMultiplier = GetElementMultiplier(attackerElement, defenderElement);
            float rawDamage = Mathf.Max(1f, creaturePower * 0.35f * skillMultiplier * elementMultiplier);
            return Mathf.Max(1, Mathf.RoundToInt(rawDamage) - Mathf.Max(0, enemyDefense));
        }

        private float GetElementMultiplier(CreatureElement attacker, CreatureElement defender)
        {
            if ((attacker == CreatureElement.Fire && defender == CreatureElement.Nature) ||
                (attacker == CreatureElement.Nature && defender == CreatureElement.Water) ||
                (attacker == CreatureElement.Water && defender == CreatureElement.Fire) ||
                (attacker == CreatureElement.Shadow && defender == CreatureElement.Nature))
            {
                return 1.25f;
            }

            if ((defender == CreatureElement.Fire && attacker == CreatureElement.Nature) ||
                (defender == CreatureElement.Nature && attacker == CreatureElement.Water) ||
                (defender == CreatureElement.Water && attacker == CreatureElement.Fire) ||
                (defender == CreatureElement.Shadow && attacker == CreatureElement.Nature))
            {
                return 0.8f;
            }

            return 1f;
        }

        private float CalculateBattleRewardMultiplier(int enemiesDefeated, int totalEnemies)
        {
            if (totalEnemies <= 0)
            {
                return 1f;
            }

            if (enemiesDefeated >= totalEnemies)
            {
                return 1f;
            }

            return Mathf.Clamp(0.25f + (float)enemiesDefeated / totalEnemies * 0.6f, 0.25f, 0.85f);
        }

        private string BuildBattleResultLabel(int enemiesDefeated, int totalEnemies, float remainingHealthPercent)
        {
            if (totalEnemies <= 0 || enemiesDefeated >= totalEnemies)
            {
                return remainingHealthPercent >= 0.65f ? "Great Clear" : "Clear";
            }

            if (enemiesDefeated > 0)
            {
                return "Partial Clear";
            }

            return "Retreat";
        }

        private bool TryParseUtc(string value, out DateTime dateTime)
        {
            if (DateTimeOffset.TryParse(
                value,
                null,
                System.Globalization.DateTimeStyles.RoundtripKind,
                out DateTimeOffset dateTimeOffset))
            {
                dateTime = dateTimeOffset.UtcDateTime;
                return true;
            }

            dateTime = default(DateTime);
            return false;
        }

        private void EnsureSaveList()
        {
            PlayerSaveData saveData = GetSaveData();
            if (saveData != null)
            {
                saveData.EnsureListsAreValid();
            }
        }

        private void SaveAndNotify()
        {
            if (gameManager != null)
            {
                gameManager.SaveGame();
            }

            OnExpeditionsChanged?.Invoke();
        }

        private void FindReferencesIfNeeded()
        {
            if (gameManager == null)
            {
                gameManager = GameManager.Instance;
            }

            if (gameManager != null && !gameManager.IsInitialized)
            {
                gameManager.InitializeGame();
            }

            if (resourceManager == null)
            {
                resourceManager = ResourceManager.Instance;
            }

            if (resourceManager == null)
            {
                resourceManager = FindObjectOfType<ResourceManager>();
            }

            if (progressionManager == null)
            {
                progressionManager = CreatureProgressionManager.Instance;
            }

            if (progressionManager == null)
            {
                progressionManager = FindObjectOfType<CreatureProgressionManager>();
            }
        }
    }

    [Serializable]
    public struct ExpeditionReward
    {
        public int coin;
        public int food;
        public int crystal;
    }

    [Serializable]
    public class ExpeditionBattleResult
    {
        public string resultLabel;
        public int enemiesDefeated;
        public int totalEnemies;
        public float creatureRemainingHealthPercent;
        public float rewardMultiplier = 1f;
        public List<string> logLines = new List<string>();
    }
}
