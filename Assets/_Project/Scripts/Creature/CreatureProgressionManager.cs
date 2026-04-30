using System;
using UnityEngine;
using SpiritHatchers.Core;
using SpiritHatchers.Data;
using SpiritHatchers.Resources;

namespace SpiritHatchers.Creature
{
    public class CreatureProgressionManager : MonoBehaviour
    {
        public static CreatureProgressionManager Instance { get; private set; }

        public event Action<PlayerCreatureData> OnCreatureProgressChanged;

        [Header("References")]
        [SerializeField] private GameManager gameManager;
        [SerializeField] private ResourceManager resourceManager;
        [SerializeField] private CreatureDatabase creatureDatabase;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning("Duplicate CreatureProgressionManager found. Destroying the new one.");
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void Start()
        {
            FindReferencesIfNeeded();
        }

        public int GetUpgradeCost(PlayerCreatureData playerCreature)
        {
            if (playerCreature == null)
            {
                return 0;
            }

            int level = Mathf.Max(1, playerCreature.level);
            return 20 + level * 10;
        }

        public int CalculatePower(CreatureStaticData creatureData, PlayerCreatureData playerCreature)
        {
            if (creatureData == null || playerCreature == null)
            {
                return 0;
            }

            CreatureFormData form = GetCurrentForm(creatureData, playerCreature);
            float multiplier = form != null ? Mathf.Max(0f, form.powerMultiplier) : 1f;
            int level = Mathf.Max(1, playerCreature.level);

            return Mathf.RoundToInt(creatureData.basePower * multiplier + level * 5);
        }

        public bool CanUpgrade(PlayerCreatureData playerCreature)
        {
            FindReferencesIfNeeded();

            if (resourceManager == null || playerCreature == null)
            {
                return false;
            }

            return resourceManager.CanAfford(ResourceType.Coin, GetUpgradeCost(playerCreature));
        }

        public bool TryUpgrade(PlayerCreatureData playerCreature)
        {
            FindReferencesIfNeeded();

            if (playerCreature == null)
            {
                Debug.LogWarning("Upgrade failed because player creature data is missing.");
                return false;
            }

            if (resourceManager == null)
            {
                Debug.LogWarning("Upgrade failed because ResourceManager is missing.");
                return false;
            }

            CreatureStaticData creatureData = GetStaticCreatureData(playerCreature);
            int cost = GetUpgradeCost(playerCreature);

            if (!resourceManager.SpendResource(ResourceType.Coin, cost))
            {
                Debug.Log($"Upgrade failed. Missing Coin. Need {cost}.");
                return false;
            }

            playerCreature.level = Mathf.Max(1, playerCreature.level) + 1;

            SaveAndNotify(playerCreature);
            Debug.Log($"Upgraded {GetCreatureLogName(creatureData, playerCreature)} to level {playerCreature.level}.");
            return true;
        }

        public bool IsFinalForm(CreatureStaticData creatureData, PlayerCreatureData playerCreature)
        {
            if (creatureData == null || playerCreature == null || creatureData.forms == null)
            {
                return true;
            }

            if (creatureData.forms.Count <= 1)
            {
                return true;
            }

            return creatureData.GetFormByIndex(playerCreature.currentFormIndex + 1) == null;
        }

        public bool CanEvolve(CreatureStaticData creatureData, PlayerCreatureData playerCreature, out string missingRequirements)
        {
            FindReferencesIfNeeded();

            missingRequirements = string.Empty;

            if (creatureData == null || playerCreature == null)
            {
                missingRequirements = "Creature data missing.";
                return false;
            }

            if (IsFinalForm(creatureData, playerCreature))
            {
                missingRequirements = "Already final form.";
                return false;
            }

            CreatureFormData nextForm = creatureData.GetFormByIndex(playerCreature.currentFormIndex + 1);
            if (nextForm == null)
            {
                missingRequirements = "Next form data missing.";
                return false;
            }

            GetEvolutionRequirements(playerCreature.currentFormIndex + 1, nextForm, out int requiredLevel, out int requiredFood, out int requiredCrystal);

            string missing = string.Empty;

            if (playerCreature.level < requiredLevel)
            {
                missing += $"Level {requiredLevel}";
            }

            if (resourceManager == null)
            {
                missing += AppendRequirement(missing, "Resource manager missing");
            }
            else
            {
                if (!resourceManager.CanAfford(ResourceType.Food, requiredFood))
                {
                    missing += AppendRequirement(missing, $"{requiredFood} Food");
                }

                if (!resourceManager.CanAfford(ResourceType.Crystal, requiredCrystal))
                {
                    missing += AppendRequirement(missing, $"{requiredCrystal} Crystal");
                }
            }

            missingRequirements = missing;
            return string.IsNullOrEmpty(missingRequirements);
        }

        public bool TryEvolve(CreatureStaticData creatureData, PlayerCreatureData playerCreature, out string message)
        {
            FindReferencesIfNeeded();

            if (!CanEvolve(creatureData, playerCreature, out string missingRequirements))
            {
                message = missingRequirements;
                Debug.Log($"Evolution failed for {GetCreatureLogName(creatureData, playerCreature)}. Missing: {missingRequirements}");
                return false;
            }

            CreatureFormData nextForm = creatureData.GetFormByIndex(playerCreature.currentFormIndex + 1);
            GetEvolutionRequirements(playerCreature.currentFormIndex + 1, nextForm, out int requiredLevel, out int requiredFood, out int requiredCrystal);

            if (requiredFood > 0 && !resourceManager.SpendResource(ResourceType.Food, requiredFood))
            {
                message = $"Missing {requiredFood} Food";
                Debug.Log($"Evolution failed. {message}");
                return false;
            }

            if (requiredCrystal > 0 && !resourceManager.SpendResource(ResourceType.Crystal, requiredCrystal))
            {
                if (requiredFood > 0)
                {
                    resourceManager.AddResource(ResourceType.Food, requiredFood);
                }

                message = $"Missing {requiredCrystal} Crystal";
                Debug.Log($"Evolution failed. {message}");
                return false;
            }

            playerCreature.currentFormIndex += 1;

            SaveAndNotify(playerCreature);
            message = $"Evolved to {nextForm.formName}.";
            Debug.Log($"Evolved {creatureData.creatureName} to form {playerCreature.currentFormIndex}: {nextForm.formName}.");
            return true;
        }

        public CreatureFormData GetCurrentForm(CreatureStaticData creatureData, PlayerCreatureData playerCreature)
        {
            if (creatureData == null || playerCreature == null)
            {
                return null;
            }

            CreatureFormData form = creatureData.GetFormByIndex(playerCreature.currentFormIndex);

            if (form == null && creatureData.forms != null && creatureData.forms.Count > 0)
            {
                form = creatureData.forms[0];
            }

            return form;
        }

        public CreatureFormData GetNextForm(CreatureStaticData creatureData, PlayerCreatureData playerCreature)
        {
            if (creatureData == null || playerCreature == null)
            {
                return null;
            }

            return creatureData.GetFormByIndex(playerCreature.currentFormIndex + 1);
        }

        [ContextMenu("Debug/Reset All Creatures To Baby Form")]
        public void DebugResetAllCreaturesToBabyForm()
        {
            FindReferencesIfNeeded();

            if (gameManager == null || gameManager.CurrentSaveData == null)
            {
                Debug.LogWarning("Cannot reset creatures because save data is missing.");
                return;
            }

            gameManager.CurrentSaveData.EnsureListsAreValid();
            int resetCount = 0;

            for (int i = 0; i < gameManager.CurrentSaveData.ownedCreatures.Count; i++)
            {
                PlayerCreatureData creature = gameManager.CurrentSaveData.ownedCreatures[i];
                if (creature == null)
                {
                    continue;
                }

                creature.currentFormIndex = 0;
                creature.level = 1;
                resetCount++;
                OnCreatureProgressChanged?.Invoke(creature);
            }

            if (gameManager != null)
            {
                gameManager.SaveGame();
            }

            Debug.Log($"Debug reset {resetCount} creature(s) to baby form and level 1.");
        }

        private void SaveAndNotify(PlayerCreatureData playerCreature)
        {
            if (gameManager != null)
            {
                gameManager.SaveGame();
            }
            else
            {
                Debug.LogWarning("Creature progress changed, but GameManager is missing so the game could not be saved.");
            }

            OnCreatureProgressChanged?.Invoke(playerCreature);
        }

        private string AppendRequirement(string existingText, string newRequirement)
        {
            if (string.IsNullOrEmpty(existingText))
            {
                return newRequirement;
            }

            return $", {newRequirement}";
        }

        private string GetCreatureLogName(CreatureStaticData creatureData, PlayerCreatureData playerCreature)
        {
            if (creatureData != null)
            {
                return creatureData.creatureName;
            }

            return playerCreature != null ? playerCreature.creatureId : "Unknown Creature";
        }

        private CreatureStaticData GetStaticCreatureData(PlayerCreatureData playerCreature)
        {
            if (creatureDatabase == null || playerCreature == null || string.IsNullOrEmpty(playerCreature.creatureId))
            {
                return null;
            }

            return creatureDatabase.GetCreatureById(playerCreature.creatureId);
        }

        private void GetEvolutionRequirements(
            int nextFormIndex,
            CreatureFormData nextForm,
            out int requiredLevel,
            out int requiredFood,
            out int requiredCrystal)
        {
            if (nextFormIndex == 1)
            {
                requiredLevel = GameConstants.FormTwoRequiredLevel;
                requiredFood = GameConstants.FormTwoFoodCost;
                requiredCrystal = 0;
                return;
            }

            if (nextFormIndex == 2)
            {
                requiredLevel = GameConstants.FormThreeRequiredLevel;
                requiredFood = GameConstants.FormThreeFoodCost;
                requiredCrystal = GameConstants.FormThreeCrystalCost;
                return;
            }

            requiredLevel = nextForm != null ? nextForm.requiredLevel : 0;
            requiredFood = nextForm != null ? nextForm.requiredFood : 0;
            requiredCrystal = nextForm != null ? nextForm.requiredCrystal : 0;
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
        }
    }
}
