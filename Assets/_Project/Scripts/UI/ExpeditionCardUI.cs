using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using SpiritHatchers.Expedition;
using SpiritHatchers.Save;

namespace SpiritHatchers.UI
{
    public class ExpeditionCardUI : MonoBehaviour
    {
        [Header("Text")]
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text durationText;
        [SerializeField] private TMP_Text rewardText;
        [SerializeField] private TMP_Text statusText;

        [Header("Buttons")]
        [SerializeField] private Button startButton;
        [SerializeField] private Button claimButton;

        private ExpeditionData expeditionData;
        private PlayerExpeditionData activeExpedition;
        private ExpeditionManager expeditionManager;
        private Action<ExpeditionData> onStartClicked;
        private Action<PlayerExpeditionData> onClaimClicked;

        private void Awake()
        {
            EnsureLayout();
        }

        private void Reset()
        {
            EnsureLayout();
        }

        private void OnEnable()
        {
            if (startButton != null)
            {
                startButton.onClick.AddListener(HandleStartClicked);
            }

            if (claimButton != null)
            {
                claimButton.onClick.AddListener(HandleClaimClicked);
            }
        }

        private void OnDisable()
        {
            if (startButton != null)
            {
                startButton.onClick.RemoveListener(HandleStartClicked);
            }

            if (claimButton != null)
            {
                claimButton.onClick.RemoveListener(HandleClaimClicked);
            }
        }

        public void Setup(
            ExpeditionData expedition,
            PlayerExpeditionData active,
            ExpeditionManager manager,
            Action<ExpeditionData> startCallback,
            Action<PlayerExpeditionData> claimCallback)
        {
            expeditionData = expedition;
            activeExpedition = active;
            expeditionManager = manager;
            onStartClicked = startCallback;
            onClaimClicked = claimCallback;

            EnsureLayout();
            Refresh();
        }

        private void EnsureLayout()
        {
            RectTransform cardRect = transform as RectTransform;
            if (cardRect != null)
            {
                cardRect.sizeDelta = new Vector2(860f, 250f);
            }

            LayoutElement layoutElement = GetComponent<LayoutElement>();
            if (layoutElement != null)
            {
                layoutElement.preferredWidth = 860f;
                layoutElement.preferredHeight = 250f;
            }

            Image cardImage = GetComponent<Image>();
            if (cardImage != null)
            {
                cardImage.enabled = true;
                cardImage.color = new Color(0.92f, 0.71f, 0.38f, 1f);
            }

            SetTextRect(nameText, new Vector2(0f, 1f), new Vector2(28f, -30f), new Vector2(520f, 44f), 30);
            SetTextRect(durationText, new Vector2(0f, 1f), new Vector2(28f, -78f), new Vector2(520f, 34f), 22);
            SetTextRect(rewardText, new Vector2(0f, 1f), new Vector2(28f, -120f), new Vector2(570f, 54f), 20);
            SetTextRect(statusText, new Vector2(0f, 1f), new Vector2(28f, -184f), new Vector2(520f, 36f), 22);

            SetButtonRect(startButton, new Vector2(1f, 0.5f), new Vector2(-120f, 42f), new Vector2(190f, 68f));
            SetButtonRect(claimButton, new Vector2(1f, 0.5f), new Vector2(-120f, -42f), new Vector2(190f, 68f));
        }

        private void SetTextRect(TMP_Text text, Vector2 anchor, Vector2 anchoredPosition, Vector2 size, float fontSize)
        {
            if (text == null)
            {
                return;
            }

            RectTransform rectTransform = text.rectTransform;
            rectTransform.anchorMin = anchor;
            rectTransform.anchorMax = anchor;
            rectTransform.pivot = new Vector2(0f, 0.5f);
            rectTransform.anchoredPosition = anchoredPosition;
            rectTransform.sizeDelta = size;

            text.enableAutoSizing = false;
            text.fontSize = fontSize;
            text.alignment = TextAlignmentOptions.Left;
            text.color = new Color(0.22f, 0.12f, 0.07f, 1f);
        }

        private void SetButtonRect(Button button, Vector2 anchor, Vector2 anchoredPosition, Vector2 size)
        {
            if (button == null)
            {
                return;
            }

            RectTransform rectTransform = button.transform as RectTransform;
            if (rectTransform == null)
            {
                return;
            }

            rectTransform.anchorMin = anchor;
            rectTransform.anchorMax = anchor;
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = anchoredPosition;
            rectTransform.sizeDelta = size;
        }

        public void Refresh()
        {
            if (expeditionData == null)
            {
                return;
            }

            SetText(nameText, expeditionData.expeditionName);
            SetText(durationText, FormatDuration(expeditionData.durationSeconds));
            int enemyCount = expeditionData.enemies != null ? expeditionData.enemies.Count : 0;
            SetText(
                rewardText,
                $"Power {expeditionData.recommendedPower}  Enemies {enemyCount}  Base: {expeditionData.baseCoin} Coin  {expeditionData.baseFood} Food  {expeditionData.baseCrystal} Crystal");

            bool hasActive = activeExpedition != null;
            bool isComplete = hasActive && expeditionManager != null && expeditionManager.IsComplete(activeExpedition);

            if (!hasActive)
            {
                SetText(statusText, "Ready");
                SetButtonActive(startButton, true);
                SetButtonActive(claimButton, false);
                return;
            }

            if (isComplete)
            {
                SetText(statusText, "Complete!");
                SetButtonActive(startButton, false);
                SetButtonActive(claimButton, true);
                return;
            }

            TimeSpan remaining = expeditionManager != null
                ? expeditionManager.GetRemainingTime(activeExpedition)
                : TimeSpan.Zero;

            SetText(statusText, $"Time left: {FormatTimeSpan(remaining)}");
            SetButtonActive(startButton, false);
            SetButtonActive(claimButton, false);
        }

        private void HandleStartClicked()
        {
            onStartClicked?.Invoke(expeditionData);
        }

        private void HandleClaimClicked()
        {
            onClaimClicked?.Invoke(activeExpedition);
        }

        private string FormatDuration(int seconds)
        {
            return $"Duration: {FormatTimeSpan(TimeSpan.FromSeconds(seconds))}";
        }

        private string FormatTimeSpan(TimeSpan timeSpan)
        {
            if (timeSpan.TotalHours >= 1)
            {
                return $"{(int)timeSpan.TotalHours:00}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}";
            }

            return $"{timeSpan.Minutes:00}:{timeSpan.Seconds:00}";
        }

        private void SetButtonActive(Button button, bool isActive)
        {
            if (button != null)
            {
                button.gameObject.SetActive(isActive);
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
