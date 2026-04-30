using System;
using UnityEngine;
using SpiritHatchers.Core;
using SpiritHatchers.Resources;
using SpiritHatchers.Save;

namespace SpiritHatchers.DailyReward
{
    public class DailyRewardManager : MonoBehaviour
    {
        public static DailyRewardManager Instance { get; private set; }

        public event Action OnDailyRewardStateChanged;

        [Header("References")]
        [SerializeField] private GameManager gameManager;
        [SerializeField] private ResourceManager resourceManager;

        public string TodayUtcDate
        {
            get { return DateTime.UtcNow.ToString("yyyy-MM-dd"); }
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning("Duplicate DailyRewardManager found. Destroying the new one.");
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void Start()
        {
            FindReferencesIfNeeded();
            OnDailyRewardStateChanged?.Invoke();
        }

        public bool CanClaim()
        {
            PlayerSaveData saveData = GetSaveData();

            if (saveData == null)
            {
                return false;
            }

            return saveData.lastDailyRewardClaimDate != TodayUtcDate;
        }

        public bool ClaimDailyReward()
        {
            FindReferencesIfNeeded();

            PlayerSaveData saveData = GetSaveData();
            if (saveData == null)
            {
                Debug.LogWarning("Daily reward claim failed because save data is missing.");
                return false;
            }

            if (resourceManager == null)
            {
                Debug.LogWarning("Daily reward claim failed because ResourceManager is missing.");
                return false;
            }

            if (!CanClaim())
            {
                Debug.Log("Daily reward already claimed today.");
                OnDailyRewardStateChanged?.Invoke();
                return false;
            }

            saveData.lastDailyRewardClaimDate = TodayUtcDate;
            resourceManager.AddResources(
                GameConstants.DailyRewardCoins,
                GameConstants.DailyRewardFood,
                0,
                GameConstants.DailyRewardEggTickets);

            if (gameManager != null)
            {
                gameManager.SaveGame();
            }

            Debug.Log($"Claimed daily reward for {TodayUtcDate}: {GameConstants.DailyRewardEggTickets} EggTicket, {GameConstants.DailyRewardCoins} Coin, {GameConstants.DailyRewardFood} Food.");
            OnDailyRewardStateChanged?.Invoke();
            return true;
        }

        public string GetStatusText()
        {
            return CanClaim() ? "Claim" : "Claimed today";
        }

        private PlayerSaveData GetSaveData()
        {
            FindReferencesIfNeeded();

            if (gameManager == null)
            {
                return null;
            }

            if (!gameManager.IsInitialized)
            {
                gameManager.InitializeGame();
            }

            return gameManager.CurrentSaveData;
        }

        private void FindReferencesIfNeeded()
        {
            if (gameManager == null)
            {
                gameManager = GameManager.Instance;
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
