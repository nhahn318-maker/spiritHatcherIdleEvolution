using System;
using UnityEngine;
using SpiritHatchers.Core;
using SpiritHatchers.Save;

namespace SpiritHatchers.Resources
{
    public class ResourceManager : MonoBehaviour
    {
        public static ResourceManager Instance { get; private set; }

        public event Action<ResourceType, int> OnResourceChanged;
        public event Action OnAnyResourceChanged;

        [SerializeField] private GameManager gameManager;

        private PlayerSaveData SaveData
        {
            get
            {
                if (gameManager == null)
                {
                    gameManager = GameManager.Instance;
                }

                if (gameManager != null && !gameManager.IsInitialized)
                {
                    gameManager.InitializeGame();
                }

                return gameManager != null ? gameManager.CurrentSaveData : null;
            }
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning("Duplicate ResourceManager found. Destroying the new one.");
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void Start()
        {
            if (gameManager == null)
            {
                gameManager = GameManager.Instance;
            }

            NotifyAllResourcesChanged();
        }

        public int GetResource(ResourceType type)
        {
            PlayerSaveData saveData = SaveData;
            if (saveData == null)
            {
                Debug.LogWarning($"Cannot get {type}. Save data is missing.");
                return 0;
            }

            switch (type)
            {
                case ResourceType.Coin:
                    return saveData.coin;
                case ResourceType.Food:
                    return saveData.food;
                case ResourceType.Crystal:
                    return saveData.crystal;
                case ResourceType.EggTicket:
                    return saveData.eggTicket;
                default:
                    Debug.LogWarning($"Unknown resource type: {type}");
                    return 0;
            }
        }

        public void AddResource(ResourceType type, int amount)
        {
            if (amount <= 0)
            {
                Debug.LogWarning($"AddResource ignored for {type}. Amount must be positive.");
                return;
            }

            PlayerSaveData saveData = SaveData;
            if (saveData == null)
            {
                Debug.LogWarning($"Cannot add {type}. Save data is missing.");
                return;
            }

            int newValue = GetResource(type) + amount;
            SetResource(type, newValue);

            Debug.Log($"Added {amount} {type}. New total: {newValue}");
            SaveAndNotify(type);
        }

        public void AddResources(int coinAmount, int foodAmount, int crystalAmount, int eggTicketAmount)
        {
            if (coinAmount <= 0 && foodAmount <= 0 && crystalAmount <= 0 && eggTicketAmount <= 0)
            {
                Debug.LogWarning("AddResources ignored because all amounts were zero or negative.");
                return;
            }

            PlayerSaveData saveData = SaveData;
            if (saveData == null)
            {
                Debug.LogWarning("Cannot add resources. Save data is missing.");
                return;
            }

            if (coinAmount > 0)
            {
                SetResource(ResourceType.Coin, saveData.coin + coinAmount);
            }

            if (foodAmount > 0)
            {
                SetResource(ResourceType.Food, saveData.food + foodAmount);
            }

            if (crystalAmount > 0)
            {
                SetResource(ResourceType.Crystal, saveData.crystal + crystalAmount);
            }

            if (eggTicketAmount > 0)
            {
                SetResource(ResourceType.EggTicket, saveData.eggTicket + eggTicketAmount);
            }

            Debug.Log($"Added resources. Coin +{Mathf.Max(0, coinAmount)}, Food +{Mathf.Max(0, foodAmount)}, Crystal +{Mathf.Max(0, crystalAmount)}, EggTicket +{Mathf.Max(0, eggTicketAmount)}.");
            SaveAndNotifyAllResources();
        }

        public bool SpendResource(ResourceType type, int amount)
        {
            if (amount <= 0)
            {
                Debug.LogWarning($"SpendResource ignored for {type}. Amount must be positive.");
                return false;
            }

            if (!CanAfford(type, amount))
            {
                Debug.Log($"Not enough {type}. Need {amount}, have {GetResource(type)}.");
                return false;
            }

            int newValue = GetResource(type) - amount;
            SetResource(type, newValue);

            Debug.Log($"Spent {amount} {type}. New total: {newValue}");
            SaveAndNotify(type);
            return true;
        }

        public bool CanAfford(ResourceType type, int amount)
        {
            if (amount <= 0)
            {
                return true;
            }

            return GetResource(type) >= amount;
        }

        [ContextMenu("Debug/Add 1000 Coin")]
        public void DebugAdd1000Coin()
        {
            AddResource(ResourceType.Coin, 1000);
        }

        [ContextMenu("Debug/Add 500 Food")]
        public void DebugAdd500Food()
        {
            AddResource(ResourceType.Food, 500);
        }

        [ContextMenu("Debug/Add 100 Crystal")]
        public void DebugAdd100Crystal()
        {
            AddResource(ResourceType.Crystal, 100);
        }

        [ContextMenu("Debug/Add 5 EggTicket")]
        public void DebugAdd5EggTicket()
        {
            AddResource(ResourceType.EggTicket, 5);
        }

        private void SetResource(ResourceType type, int value)
        {
            PlayerSaveData saveData = SaveData;
            if (saveData == null)
            {
                return;
            }

            int safeValue = Mathf.Max(0, value);

            switch (type)
            {
                case ResourceType.Coin:
                    saveData.coin = safeValue;
                    break;
                case ResourceType.Food:
                    saveData.food = safeValue;
                    break;
                case ResourceType.Crystal:
                    saveData.crystal = safeValue;
                    break;
                case ResourceType.EggTicket:
                    saveData.eggTicket = safeValue;
                    break;
                default:
                    Debug.LogWarning($"Unknown resource type: {type}");
                    break;
            }
        }

        private void SaveAndNotify(ResourceType type)
        {
            if (gameManager != null)
            {
                gameManager.SaveGame();
            }
            else
            {
                Debug.LogWarning("Resource changed, but GameManager was missing so the game could not be saved.");
            }

            OnResourceChanged?.Invoke(type, GetResource(type));
            OnAnyResourceChanged?.Invoke();
        }

        private void SaveAndNotifyAllResources()
        {
            if (gameManager != null)
            {
                gameManager.SaveGame();
            }
            else
            {
                Debug.LogWarning("Resources changed, but GameManager was missing so the game could not be saved.");
            }

            NotifyAllResourcesChanged();
        }

        private void NotifyAllResourcesChanged()
        {
            OnResourceChanged?.Invoke(ResourceType.Coin, GetResource(ResourceType.Coin));
            OnResourceChanged?.Invoke(ResourceType.Food, GetResource(ResourceType.Food));
            OnResourceChanged?.Invoke(ResourceType.Crystal, GetResource(ResourceType.Crystal));
            OnResourceChanged?.Invoke(ResourceType.EggTicket, GetResource(ResourceType.EggTicket));
            OnAnyResourceChanged?.Invoke();
        }
    }
}
