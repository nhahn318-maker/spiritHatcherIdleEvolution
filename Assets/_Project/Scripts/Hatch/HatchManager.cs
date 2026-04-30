using System;
using System.Collections.Generic;
using UnityEngine;
using SpiritHatchers.Core;
using SpiritHatchers.Data;
using SpiritHatchers.Resources;
using SpiritHatchers.Save;

namespace SpiritHatchers.Hatch
{
    public class HatchManager : MonoBehaviour
    {
        public static HatchManager Instance { get; private set; }

        public event Action<HatchResult> OnHatchCompleted;

        [Header("References")]
        [SerializeField] private CreatureDatabase creatureDatabase;
        [SerializeField] private ResourceManager resourceManager;
        [SerializeField] private GameManager gameManager;

        [Header("Optional")]
        [SerializeField] private Sprite placeholderSprite;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning("Duplicate HatchManager found. Destroying the new one.");
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void Start()
        {
            FindReferencesIfNeeded();
        }

        public bool CanHatch()
        {
            FindReferencesIfNeeded();

            if (resourceManager == null)
            {
                return false;
            }

            if (resourceManager.GetResource(ResourceType.EggTicket) > 0)
            {
                return true;
            }

            return resourceManager.CanAfford(ResourceType.Crystal, GameConstants.HatchCrystalCost);
        }

        public HatchResult HatchCreature()
        {
            FindReferencesIfNeeded();

            if (creatureDatabase == null)
            {
                Debug.LogWarning("Cannot hatch because CreatureDatabase is missing.");
                return null;
            }

            if (resourceManager == null)
            {
                Debug.LogWarning("Cannot hatch because ResourceManager is missing.");
                return null;
            }

            if (gameManager == null || gameManager.CurrentSaveData == null)
            {
                Debug.LogWarning("Cannot hatch because save data is missing.");
                return null;
            }

            ResourceType spentResourceType;
            int spentAmount;

            if (resourceManager.GetResource(ResourceType.EggTicket) > 0)
            {
                spentResourceType = ResourceType.EggTicket;
                spentAmount = GameConstants.HatchTicketCost;
            }
            else
            {
                spentResourceType = ResourceType.Crystal;
                spentAmount = GameConstants.HatchCrystalCost;
            }

            if (!resourceManager.SpendResource(spentResourceType, spentAmount))
            {
                Debug.Log("Hatch failed because the player cannot afford it.");
                return null;
            }

            CreatureRarity selectedRarity = RollRarity();
            CreatureStaticData selectedCreature = GetRandomCreatureForHatch(selectedRarity);

            if (selectedCreature == null)
            {
                Debug.LogWarning($"No creature found for rarity {selectedRarity}. Refunding hatch cost.");
                resourceManager.AddResource(spentResourceType, spentAmount);
                return null;
            }

            HatchResult result = BuildHatchResult(selectedCreature, spentResourceType, spentAmount);

            if (result.isDuplicate)
            {
                GiveDuplicateReward(selectedCreature.rarity, result);
            }
            else
            {
                AddNewCreatureToPlayer(selectedCreature);
            }

            gameManager.SaveGame();

            Debug.Log($"Hatched {selectedCreature.creatureName} ({selectedCreature.rarity}). Duplicate: {result.isDuplicate}");
            OnHatchCompleted?.Invoke(result);
            return result;
        }

        private HatchResult BuildHatchResult(CreatureStaticData creature, ResourceType spentResourceType, int spentAmount)
        {
            return new HatchResult
            {
                creature = creature,
                creatureName = creature.creatureName,
                rarity = creature.rarity,
                sprite = GetDisplaySprite(creature),
                idleFrames = GetDisplayIdleFrames(creature),
                displayScale = GetDisplayScale(creature),
                isDuplicate = PlayerOwnsCreature(creature.creatureId),
                duplicateFoodReward = 0,
                duplicateCrystalReward = 0,
                spentResourceType = spentResourceType,
                spentAmount = spentAmount
            };
        }

        private CreatureRarity RollRarity()
        {
            int roll = UnityEngine.Random.Range(1, 101);

            if (roll <= GameConstants.CommonRarityRate)
            {
                return CreatureRarity.Common;
            }

            if (roll <= GameConstants.CommonRarityRate + GameConstants.RareRarityRate)
            {
                return CreatureRarity.Rare;
            }

            return CreatureRarity.Epic;
        }

        private CreatureStaticData GetRandomCreatureByRarity(CreatureRarity rarity)
        {
            List<CreatureStaticData> creatures = creatureDatabase.GetCreaturesByRarity(rarity);

            if (creatures == null || creatures.Count == 0)
            {
                return null;
            }

            int randomIndex = UnityEngine.Random.Range(0, creatures.Count);
            return creatures[randomIndex];
        }

        private bool PlayerOwnsCreature(string creatureId)
        {
            PlayerSaveData saveData = gameManager.CurrentSaveData;
            saveData.EnsureListsAreValid();

            for (int i = 0; i < saveData.ownedCreatures.Count; i++)
            {
                PlayerCreatureData ownedCreature = saveData.ownedCreatures[i];

                if (ownedCreature != null && ownedCreature.creatureId == creatureId)
                {
                    return true;
                }
            }

            return false;
        }

        private void AddNewCreatureToPlayer(CreatureStaticData creature)
        {
            PlayerSaveData saveData = gameManager.CurrentSaveData;
            saveData.EnsureListsAreValid();

            string uniqueId = $"{creature.creatureId}_{Guid.NewGuid():N}";
            PlayerCreatureData newCreature = new PlayerCreatureData(uniqueId, creature.creatureId);
            saveData.ownedCreatures.Add(newCreature);
        }

        private void GiveDuplicateReward(CreatureRarity rarity, HatchResult result)
        {
            switch (rarity)
            {
                case CreatureRarity.Common:
                    result.duplicateFoodReward = 50;
                    result.duplicateCrystalReward = 0;
                    break;
                case CreatureRarity.Rare:
                    result.duplicateFoodReward = 100;
                    result.duplicateCrystalReward = 5;
                    break;
                case CreatureRarity.Epic:
                    result.duplicateFoodReward = 200;
                    result.duplicateCrystalReward = 15;
                    break;
                default:
                    Debug.LogWarning($"Unknown rarity for duplicate reward: {rarity}");
                    break;
            }

            if (result.duplicateFoodReward > 0)
            {
                resourceManager.AddResources(0, result.duplicateFoodReward, result.duplicateCrystalReward, 0);
            }
        }

        private CreatureStaticData GetRandomCreatureForHatch(CreatureRarity selectedRarity)
        {
            CreatureStaticData selectedCreature = GetRandomCreatureByRarity(selectedRarity);
            if (selectedCreature != null)
            {
                return selectedCreature;
            }

            Debug.LogWarning($"No creature found for rolled rarity {selectedRarity}. Trying another populated rarity so the hatch can still complete.");

            CreatureRarity[] fallbackOrder =
            {
                CreatureRarity.Common,
                CreatureRarity.Rare,
                CreatureRarity.Epic
            };

            for (int i = 0; i < fallbackOrder.Length; i++)
            {
                selectedCreature = GetRandomCreatureByRarity(fallbackOrder[i]);
                if (selectedCreature != null)
                {
                    return selectedCreature;
                }
            }

            return null;
        }

        private Sprite GetDisplaySprite(CreatureStaticData creature)
        {
            CreatureFormData firstForm = GetFirstForm(creature);

            if (firstForm != null && firstForm.sprite != null)
            {
                return firstForm.sprite;
            }

            return placeholderSprite;
        }

        private Sprite[] GetDisplayIdleFrames(CreatureStaticData creature)
        {
            CreatureFormData firstForm = GetFirstForm(creature);
            return firstForm != null ? firstForm.idleFrames : null;
        }

        private float GetDisplayScale(CreatureStaticData creature)
        {
            CreatureFormData firstForm = GetFirstForm(creature);
            return firstForm != null ? Mathf.Max(0.1f, firstForm.displayScale) : 1f;
        }

        private CreatureFormData GetFirstForm(CreatureStaticData creature)
        {
            if (creature == null || creature.forms == null || creature.forms.Count == 0)
            {
                return null;
            }

            CreatureFormData firstForm = creature.GetFormByIndex(0);

            if (firstForm == null)
            {
                firstForm = creature.forms[0];
            }

            return firstForm;
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

    [Serializable]
    public class HatchResult
    {
        public CreatureStaticData creature;
        public string creatureName;
        public CreatureRarity rarity;
        public Sprite sprite;
        public Sprite[] idleFrames;
        public float displayScale;
        public bool isDuplicate;
        public int duplicateFoodReward;
        public int duplicateCrystalReward;
        public ResourceType spentResourceType;
        public int spentAmount;
    }
}
