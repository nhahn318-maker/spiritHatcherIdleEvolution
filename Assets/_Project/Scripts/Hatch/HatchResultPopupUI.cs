using TMPro;
using UnityEngine;
using UnityEngine.UI;
using SpiritHatchers.UI;

namespace SpiritHatchers.Hatch
{
    public class HatchResultPopupUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private HatchManager hatchManager;

        [Header("Popup Root")]
        [SerializeField] private GameObject popupRoot;

        [Header("Result UI")]
        [SerializeField] private TMP_Text creatureNameText;
        [SerializeField] private TMP_Text rarityText;
        [SerializeField] private Image creatureImage;
        [SerializeField] private CreatureSpriteAnimationUI creatureAnimation;
        [SerializeField] private TMP_Text duplicateRewardText;

        [Header("Buttons")]
        [SerializeField] private Button closeButton;

        private void Awake()
        {
            if (popupRoot == null)
            {
                Debug.LogWarning("HatchResultPopupUI needs a popupRoot assigned. Use a child panel so this script can stay active while the popup is hidden.");
            }
        }

        private void OnEnable()
        {
            FindHatchManagerIfNeeded();

            if (hatchManager != null)
            {
                hatchManager.OnHatchCompleted += ShowResult;
            }

            if (closeButton != null)
            {
                closeButton.onClick.AddListener(Hide);
            }

            if (popupRoot != null && popupRoot != gameObject)
            {
                Hide();
            }
        }

        private void OnDisable()
        {
            if (hatchManager != null)
            {
                hatchManager.OnHatchCompleted -= ShowResult;
            }

            if (closeButton != null)
            {
                closeButton.onClick.RemoveListener(Hide);
            }
        }

        public void ShowResult(HatchResult result)
        {
            if (result == null)
            {
                return;
            }

            if (creatureNameText != null)
            {
                creatureNameText.text = result.creatureName;
            }

            if (rarityText != null)
            {
                rarityText.text = result.rarity.ToString();
            }

            if (creatureImage != null)
            {
                creatureImage.rectTransform.localScale = Vector3.one * Mathf.Max(0.1f, result.displayScale);
                FindCreatureAnimationIfNeeded();

                if (creatureAnimation != null && result.idleFrames != null && result.idleFrames.Length > 0)
                {
                    creatureAnimation.Play(result.idleFrames, result.sprite);
                }
                else
                {
                    if (creatureAnimation != null)
                    {
                        creatureAnimation.Stop(result.sprite);
                        return;
                    }

                    creatureImage.sprite = result.sprite;
                    creatureImage.enabled = result.sprite != null;
                    creatureImage.preserveAspect = true;
                }
            }

            if (duplicateRewardText != null)
            {
                duplicateRewardText.text = BuildDuplicateRewardText(result);
                duplicateRewardText.gameObject.SetActive(result.isDuplicate);
            }

            if (popupRoot != null)
            {
                popupRoot.SetActive(true);
            }
        }

        public void Hide()
        {
            if (popupRoot != null)
            {
                popupRoot.SetActive(false);
            }
        }

        private string BuildDuplicateRewardText(HatchResult result)
        {
            if (!result.isDuplicate)
            {
                return string.Empty;
            }

            string rewardText = "Duplicate Reward:";

            if (result.duplicateFoodReward > 0)
            {
                rewardText += $" {result.duplicateFoodReward} Food";
            }

            if (result.duplicateCrystalReward > 0)
            {
                if (result.duplicateFoodReward > 0)
                {
                    rewardText += " +";
                }

                rewardText += $" {result.duplicateCrystalReward} Crystal";
            }

            return rewardText;
        }

        private void FindHatchManagerIfNeeded()
        {
            if (hatchManager == null)
            {
                hatchManager = HatchManager.Instance;
            }

            if (hatchManager == null)
            {
                hatchManager = FindObjectOfType<HatchManager>();
            }
        }

        private void FindCreatureAnimationIfNeeded()
        {
            if (creatureAnimation == null && creatureImage != null)
            {
                creatureAnimation = creatureImage.GetComponent<CreatureSpriteAnimationUI>();

                if (creatureAnimation == null)
                {
                    creatureAnimation = creatureImage.gameObject.AddComponent<CreatureSpriteAnimationUI>();
                }
            }
        }
    }
}
