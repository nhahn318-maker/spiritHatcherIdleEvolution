using TMPro;
using UnityEngine;
using UnityEngine.UI;
using SpiritHatchers.Resources;

namespace SpiritHatchers.Ads
{
    public enum RewardedAdRewardType
    {
        EggTicket,
        DoubleExpeditionReward
    }

    public class RewardedAdButtonUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private MockRewardedAdService rewardedAdService;
        [SerializeField] private ResourceManager resourceManager;
        [SerializeField] private Button button;
        [SerializeField] private TMP_Text buttonText;

        [Header("Reward")]
        [SerializeField] private RewardedAdRewardType rewardType = RewardedAdRewardType.EggTicket;
        [SerializeField] private string placementId = "reward_egg_ticket";
        [SerializeField] private int eggTicketRewardAmount = 1;

        [Header("Button Labels")]
        [SerializeField] private string readyLabel = "Watch Ad";
        [SerializeField] private string loadingLabel = "Watching...";

        private bool isWaitingForAd;

        private void Awake()
        {
            if (button == null)
            {
                button = GetComponent<Button>();
            }
        }

        private void OnEnable()
        {
            FindReferencesIfNeeded();

            if (button != null)
            {
                button.onClick.AddListener(OnButtonClicked);
            }

            RefreshButton();
        }

        private void OnDisable()
        {
            if (button != null)
            {
                button.onClick.RemoveListener(OnButtonClicked);
            }
        }

        private void OnButtonClicked()
        {
            FindReferencesIfNeeded();

            if (rewardedAdService == null)
            {
                Debug.LogWarning("Rewarded ad button clicked, but no rewarded ad service is assigned.");
                return;
            }

            if (!rewardedAdService.IsAdReady())
            {
                Debug.Log("Rewarded ad is not ready yet.");
                return;
            }

            isWaitingForAd = true;
            RefreshButton();

            rewardedAdService.ShowRewardedAd(placementId, HandleRewardedAdCompleted);
        }

        private void HandleRewardedAdCompleted()
        {
            if (rewardType == RewardedAdRewardType.EggTicket)
            {
                GiveEggTicketReward();
            }
            else
            {
                Debug.Log("Mock rewarded ad completed for future double expedition reward placement.");
            }

            isWaitingForAd = false;
            RefreshButton();
        }

        private void GiveEggTicketReward()
        {
            FindReferencesIfNeeded();

            if (resourceManager == null)
            {
                Debug.LogWarning("Cannot give mock ad EggTicket reward because ResourceManager is missing.");
                return;
            }

            resourceManager.AddResource(ResourceType.EggTicket, eggTicketRewardAmount);
            Debug.Log($"Granted mock rewarded ad reward: {eggTicketRewardAmount} EggTicket.");
        }

        private void RefreshButton()
        {
            bool canClick = rewardedAdService != null && rewardedAdService.IsAdReady() && !isWaitingForAd;

            if (button != null)
            {
                button.interactable = canClick;
            }

            SetText(buttonText, isWaitingForAd ? loadingLabel : readyLabel);
        }

        private void FindReferencesIfNeeded()
        {
            if (rewardedAdService == null)
            {
                rewardedAdService = MockRewardedAdService.Instance;
            }

            if (rewardedAdService == null)
            {
                rewardedAdService = FindObjectOfType<MockRewardedAdService>();
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

        private void SetText(TMP_Text targetText, string value)
        {
            if (targetText != null)
            {
                targetText.text = value;
            }
        }
    }
}
