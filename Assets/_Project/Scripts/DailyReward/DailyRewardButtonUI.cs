using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SpiritHatchers.DailyReward
{
    public class DailyRewardButtonUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private DailyRewardManager dailyRewardManager;
        [SerializeField] private Button claimButton;

        [Header("TextMeshPro")]
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text bodyText;
        [SerializeField] private TMP_Text buttonText;

        private void Awake()
        {
            if (claimButton == null)
            {
                claimButton = GetComponent<Button>();
            }
        }

        private void OnEnable()
        {
            FindManagerIfNeeded();

            if (claimButton != null)
            {
                claimButton.onClick.AddListener(OnClaimClicked);
            }

            if (dailyRewardManager != null)
            {
                dailyRewardManager.OnDailyRewardStateChanged += Refresh;
            }

            Refresh();
        }

        private void OnDisable()
        {
            if (claimButton != null)
            {
                claimButton.onClick.RemoveListener(OnClaimClicked);
            }

            if (dailyRewardManager != null)
            {
                dailyRewardManager.OnDailyRewardStateChanged -= Refresh;
            }
        }

        public void Refresh()
        {
            FindManagerIfNeeded();

            SetText(titleText, "Daily Reward");
            SetText(bodyText, "1 Egg Ticket  +  100 Coin  +  50 Food");

            if (dailyRewardManager == null)
            {
                SetText(buttonText, "Unavailable");
                SetButtonInteractable(false);
                return;
            }

            bool canClaim = dailyRewardManager.CanClaim();
            SetText(buttonText, dailyRewardManager.GetStatusText());
            SetButtonInteractable(canClaim);
        }

        private void OnClaimClicked()
        {
            FindManagerIfNeeded();

            if (dailyRewardManager == null)
            {
                Debug.LogWarning("Daily reward button clicked, but DailyRewardManager is missing.");
                return;
            }

            dailyRewardManager.ClaimDailyReward();
            Refresh();
        }

        private void FindManagerIfNeeded()
        {
            if (dailyRewardManager == null)
            {
                dailyRewardManager = DailyRewardManager.Instance;
            }

            if (dailyRewardManager == null)
            {
                dailyRewardManager = FindObjectOfType<DailyRewardManager>();
            }
        }

        private void SetButtonInteractable(bool isInteractable)
        {
            if (claimButton != null)
            {
                claimButton.interactable = isInteractable;
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
