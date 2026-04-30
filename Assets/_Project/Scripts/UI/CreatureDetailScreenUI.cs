using TMPro;
using UnityEngine;
using UnityEngine.UI;
using SpiritHatchers.Creature;
using SpiritHatchers.Data;

namespace SpiritHatchers.UI
{
    public class CreatureDetailScreenUI : MonoBehaviour
    {
        [Header("Managers")]
        [SerializeField] private UIManager uiManager;
        [SerializeField] private CreatureProgressionManager progressionManager;

        [Header("Creature Display")]
        [SerializeField] private Image creatureImage;
        [SerializeField] private CreatureSpriteAnimationUI creatureAnimation;
        [SerializeField] private Sprite placeholderSprite;

        [Header("Text")]
        [SerializeField] private TMP_Text creatureNameText;
        [SerializeField] private TMP_Text elementText;
        [SerializeField] private TMP_Text rarityText;
        [SerializeField] private TMP_Text formNameText;
        [SerializeField] private TMP_Text levelText;
        [SerializeField] private TMP_Text powerText;
        [SerializeField] private TMP_Text messageText;

        [Header("Buttons")]
        [SerializeField] private Button upgradeButton;
        [SerializeField] private Button evolveButton;
        [SerializeField] private Button backButton;
        [SerializeField] private TMP_Text upgradeButtonText;
        [SerializeField] private TMP_Text evolveButtonText;

        private CreatureStaticData currentCreatureData;
        private PlayerCreatureData currentPlayerCreatureData;

        private void OnEnable()
        {
            if (upgradeButton != null)
            {
                upgradeButton.onClick.AddListener(OnUpgradeClicked);
            }

            if (evolveButton != null)
            {
                evolveButton.onClick.AddListener(OnEvolveClicked);
            }

            if (backButton != null)
            {
                backButton.onClick.AddListener(OnBackClicked);
            }
        }

        private void OnDisable()
        {
            if (upgradeButton != null)
            {
                upgradeButton.onClick.RemoveListener(OnUpgradeClicked);
            }

            if (evolveButton != null)
            {
                evolveButton.onClick.RemoveListener(OnEvolveClicked);
            }

            if (backButton != null)
            {
                backButton.onClick.RemoveListener(OnBackClicked);
            }
        }

        public void ShowCreature(CreatureStaticData creatureData, PlayerCreatureData playerCreatureData)
        {
            currentCreatureData = creatureData;
            currentPlayerCreatureData = playerCreatureData;
            Refresh();
        }

        public void Refresh()
        {
            FindProgressionManagerIfNeeded();

            if (currentCreatureData == null || currentPlayerCreatureData == null)
            {
                ShowEmptyState();
                return;
            }

            CreatureFormData currentForm = progressionManager != null
                ? progressionManager.GetCurrentForm(currentCreatureData, currentPlayerCreatureData)
                : GetCurrentForm();
            Sprite sprite = currentForm != null ? currentForm.sprite : placeholderSprite;
            Sprite[] idleFrames = currentForm != null ? currentForm.idleFrames : null;
            float displayScale = currentForm != null ? currentForm.displayScale : 1f;

            SetImage(creatureImage, sprite, idleFrames, displayScale);
            SetText(creatureNameText, currentCreatureData.creatureName);
            SetText(elementText, currentCreatureData.element.ToString());
            SetText(rarityText, currentCreatureData.rarity.ToString());
            SetText(formNameText, currentForm != null ? currentForm.formName : "Unknown Form");
            SetText(levelText, $"Lv. {Mathf.Max(1, currentPlayerCreatureData.level)}");
            SetText(powerText, CalculatePower().ToString());
            SetText(messageText, string.Empty);
            RefreshButtons();
        }

        private void ShowEmptyState()
        {
            SetImage(creatureImage, placeholderSprite, null, 1f);
            SetText(creatureNameText, "No Creature Selected");
            SetText(elementText, string.Empty);
            SetText(rarityText, string.Empty);
            SetText(formNameText, string.Empty);
            SetText(levelText, string.Empty);
            SetText(powerText, string.Empty);
            SetText(messageText, string.Empty);
            SetText(upgradeButtonText, "Upgrade");
            SetText(evolveButtonText, "Evolve");

            if (upgradeButton != null)
            {
                upgradeButton.interactable = false;
            }

            if (evolveButton != null)
            {
                evolveButton.interactable = false;
            }
        }

        private CreatureFormData GetCurrentForm()
        {
            if (currentCreatureData == null)
            {
                return null;
            }

            CreatureFormData form = currentCreatureData.GetFormByIndex(currentPlayerCreatureData.currentFormIndex);

            if (form == null && currentCreatureData.forms != null && currentCreatureData.forms.Count > 0)
            {
                form = currentCreatureData.forms[0];
            }

            return form;
        }

        private int CalculatePower()
        {
            if (currentCreatureData == null || currentPlayerCreatureData == null)
            {
                return 0;
            }

            if (progressionManager != null)
            {
                return progressionManager.CalculatePower(currentCreatureData, currentPlayerCreatureData);
            }

            CreatureFormData form = GetCurrentForm();
            float multiplier = form != null ? Mathf.Max(0f, form.powerMultiplier) : 1f;
            int level = Mathf.Max(1, currentPlayerCreatureData.level);

            return Mathf.RoundToInt(currentCreatureData.basePower * multiplier + level * 5);
        }

        private void OnUpgradeClicked()
        {
            FindProgressionManagerIfNeeded();

            if (progressionManager == null)
            {
                SetText(messageText, "Upgrade unavailable.");
                Debug.LogWarning("Upgrade failed because CreatureProgressionManager is missing.");
                return;
            }

            int cost = progressionManager.GetUpgradeCost(currentPlayerCreatureData);

            if (progressionManager.TryUpgrade(currentPlayerCreatureData))
            {
                Refresh();
                SetText(messageText, $"Upgraded! Spent {cost} Coin.");
            }
            else
            {
                SetText(messageText, $"Need {cost} Coin.");
                RefreshButtons();
            }
        }

        private void OnEvolveClicked()
        {
            FindProgressionManagerIfNeeded();

            if (progressionManager == null)
            {
                SetText(messageText, "Evolution unavailable.");
                Debug.LogWarning("Evolution failed because CreatureProgressionManager is missing.");
                return;
            }

            if (progressionManager.TryEvolve(currentCreatureData, currentPlayerCreatureData, out string message))
            {
                Refresh();
                SetText(messageText, message);
            }
            else
            {
                SetText(messageText, $"Missing: {message}");
                RefreshButtons();
            }
        }

        private void OnBackClicked()
        {
            if (uiManager == null)
            {
                uiManager = FindObjectOfType<UIManager>();
            }

            if (uiManager != null)
            {
                uiManager.ShowCollectionScreen();
            }
        }

        private void SetImage(Image targetImage, Sprite sprite)
        {
            SetImage(targetImage, sprite, null, 1f);
        }

        private void SetImage(Image targetImage, Sprite sprite, Sprite[] idleFrames, float displayScale)
        {
            if (targetImage == null)
            {
                return;
            }

            targetImage.rectTransform.localScale = Vector3.one * Mathf.Max(0.1f, displayScale);

            if (targetImage == creatureImage)
            {
                FindCreatureAnimationIfNeeded();

                if (creatureAnimation != null && idleFrames != null && idleFrames.Length > 0)
                {
                    creatureAnimation.Play(idleFrames, sprite);
                    return;
                }

                if (creatureAnimation != null)
                {
                    creatureAnimation.Stop(sprite);
                    return;
                }
            }

            targetImage.sprite = sprite;
            targetImage.enabled = sprite != null;
            targetImage.preserveAspect = true;
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

        private void SetText(TMP_Text targetText, string value)
        {
            if (targetText != null)
            {
                targetText.text = value;
            }
        }

        private void RefreshButtons()
        {
            if (progressionManager == null || currentCreatureData == null || currentPlayerCreatureData == null)
            {
                return;
            }

            int upgradeCost = progressionManager.GetUpgradeCost(currentPlayerCreatureData);
            SetText(upgradeButtonText, $"Upgrade\n{upgradeCost} Coin");

            if (upgradeButton != null)
            {
                upgradeButton.interactable = progressionManager.CanUpgrade(currentPlayerCreatureData);
            }

            if (progressionManager.IsFinalForm(currentCreatureData, currentPlayerCreatureData))
            {
                SetText(evolveButtonText, "Final Form");

                if (evolveButton != null)
                {
                    evolveButton.interactable = false;
                }

                return;
            }

            CreatureFormData nextForm = progressionManager.GetNextForm(currentCreatureData, currentPlayerCreatureData);
            string evolveLabel = nextForm != null ? $"Evolve\n{nextForm.formName}" : "Evolve";
            SetText(evolveButtonText, evolveLabel);

            bool canEvolve = progressionManager.CanEvolve(
                currentCreatureData,
                currentPlayerCreatureData,
                out string missingRequirements);

            if (evolveButton != null)
            {
                evolveButton.interactable = canEvolve;
            }

            if (!canEvolve && !string.IsNullOrEmpty(missingRequirements))
            {
                SetText(messageText, $"Missing: {missingRequirements}");
            }
        }

        private void FindProgressionManagerIfNeeded()
        {
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
}
