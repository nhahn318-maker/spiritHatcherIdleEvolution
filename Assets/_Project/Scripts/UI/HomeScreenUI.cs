using TMPro;
using UnityEngine;
using UnityEngine.UI;
using SpiritHatchers.Core;
using SpiritHatchers.Data;
using SpiritHatchers.Save;

namespace SpiritHatchers.UI
{
    public class HomeScreenUI : MonoBehaviour
    {
        [Header("Managers")]
        [SerializeField] private GameManager gameManager;
        [SerializeField] private UIManager uiManager;
        [SerializeField] private CreatureDatabase creatureDatabase;

        [Header("Top Resource Bar")]
        [SerializeField] private GameObject topResourceBar;

        [Header("Featured Creature")]
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image featuredCreatureImage;
        [SerializeField] private CreatureSpriteAnimationUI featuredCreatureAnimation;
        [SerializeField, Min(0.1f)] private float featuredCreatureScaleMultiplier = 1.25f;
        [SerializeField] private Sprite placeholderCreatureSprite;
        [SerializeField] private CreatureStaticData fallbackFeaturedCreature;

        [Header("Creature Info Card")]
        [SerializeField] private Image creaturePortraitImage;
        [SerializeField] private CreatureSpriteAnimationUI creaturePortraitAnimation;
        [SerializeField] private Image elementIconImage;
        [SerializeField] private Image rarityFrameImage;
        [SerializeField] private TMP_Text creatureNameText;
        [SerializeField] private TMP_Text rarityText;
        [SerializeField] private TMP_Text levelText;
        [SerializeField] private TMP_Text powerText;

        [Header("Daily Reward Banner")]
        [SerializeField] private GameObject dailyRewardBanner;
        [SerializeField] private Button dailyRewardButton;
        [SerializeField] private TMP_Text dailyRewardTitleText;
        [SerializeField] private TMP_Text dailyRewardBodyText;

        [Header("Bottom Navigation")]
        [SerializeField] private Button hatchButton;
        [SerializeField] private Button collectionButton;
        [SerializeField] private Button expeditionButton;
        [SerializeField] private TMP_Text hatchButtonText;
        [SerializeField] private TMP_Text collectionButtonText;
        [SerializeField] private TMP_Text expeditionButtonText;

        private void OnEnable()
        {
            FindManagersIfNeeded();
            RegisterButtonEvents();
            Refresh();
        }

        private void OnDisable()
        {
            UnregisterButtonEvents();
        }

        public void Refresh()
        {
            FindManagersIfNeeded();
            RefreshStaticLabels();
            RefreshFeaturedCreature();
        }

        private void RegisterButtonEvents()
        {
            if (dailyRewardButton != null)
            {
                dailyRewardButton.onClick.AddListener(OnDailyRewardClicked);
            }

            if (hatchButton != null)
            {
                hatchButton.onClick.AddListener(OnHatchClicked);
            }

            if (collectionButton != null)
            {
                collectionButton.onClick.AddListener(OnCollectionClicked);
            }

            if (expeditionButton != null)
            {
                expeditionButton.onClick.AddListener(OnExpeditionClicked);
            }
        }

        private void UnregisterButtonEvents()
        {
            if (dailyRewardButton != null)
            {
                dailyRewardButton.onClick.RemoveListener(OnDailyRewardClicked);
            }

            if (hatchButton != null)
            {
                hatchButton.onClick.RemoveListener(OnHatchClicked);
            }

            if (collectionButton != null)
            {
                collectionButton.onClick.RemoveListener(OnCollectionClicked);
            }

            if (expeditionButton != null)
            {
                expeditionButton.onClick.RemoveListener(OnExpeditionClicked);
            }
        }

        private void RefreshStaticLabels()
        {
            if (topResourceBar != null)
            {
                topResourceBar.SetActive(true);
            }

            if (dailyRewardBanner != null)
            {
                dailyRewardBanner.SetActive(true);
            }

            SetText(dailyRewardTitleText, "Daily Reward");
            SetText(dailyRewardBodyText, "Claim your free rewards!");
            SetText(hatchButtonText, "Hatch");
            SetText(collectionButtonText, "Collection");
            SetText(expeditionButtonText, "Expedition");
        }

        private void RefreshFeaturedCreature()
        {
            PlayerCreatureData playerCreature = GetFeaturedPlayerCreature();
            CreatureStaticData staticCreature = GetStaticCreature(playerCreature);
            CreatureFormData currentForm = GetCreatureForm(staticCreature, playerCreature);

            Sprite displaySprite = currentForm != null && currentForm.sprite != null
                ? currentForm.sprite
                : placeholderCreatureSprite;
            Sprite[] idleFrames = currentForm != null ? currentForm.idleFrames : null;
            float displayScale = currentForm != null ? currentForm.displayScale : 1f;
            string creatureName = staticCreature != null ? staticCreature.creatureName : "No Creature Yet";
            string rarity = staticCreature != null ? staticCreature.rarity.ToString() : "Common";
            int level = playerCreature != null ? Mathf.Max(1, playerCreature.level) : 1;
            int power = CalculatePower(staticCreature, playerCreature);

            SetImage(featuredCreatureImage, displaySprite, idleFrames, displayScale * featuredCreatureScaleMultiplier, ref featuredCreatureAnimation);
            SetImage(creaturePortraitImage, displaySprite, null, 1f, ref creaturePortraitAnimation);

            SetText(creatureNameText, creatureName);
            SetText(rarityText, rarity);
            SetText(levelText, $"Lv. {level}");
            SetText(powerText, power.ToString());
        }

        private PlayerCreatureData GetFeaturedPlayerCreature()
        {
            if (gameManager == null || gameManager.CurrentSaveData == null)
            {
                return null;
            }

            PlayerSaveData saveData = gameManager.CurrentSaveData;
            saveData.EnsureListsAreValid();

            for (int i = 0; i < saveData.ownedCreatures.Count; i++)
            {
                PlayerCreatureData creature = saveData.ownedCreatures[i];

                if (creature != null && creature.isFavorite)
                {
                    return creature;
                }
            }

            if (saveData.ownedCreatures.Count > 0)
            {
                return saveData.ownedCreatures[0];
            }

            return null;
        }

        private CreatureStaticData GetStaticCreature(PlayerCreatureData playerCreature)
        {
            if (playerCreature != null && creatureDatabase != null)
            {
                CreatureStaticData creature = creatureDatabase.GetCreatureById(playerCreature.creatureId);

                if (creature != null)
                {
                    return creature;
                }
            }

            return fallbackFeaturedCreature;
        }

        private CreatureFormData GetCreatureForm(CreatureStaticData staticCreature, PlayerCreatureData playerCreature)
        {
            if (staticCreature != null)
            {
                int formIndex = playerCreature != null ? playerCreature.currentFormIndex : 0;
                CreatureFormData form = staticCreature.GetFormByIndex(formIndex);

                if (form == null && staticCreature.forms != null && staticCreature.forms.Count > 0)
                {
                    form = staticCreature.forms[0];
                }

                return form;
            }

            return null;
        }

        private int CalculatePower(CreatureStaticData staticCreature, PlayerCreatureData playerCreature)
        {
            if (staticCreature == null)
            {
                return 0;
            }

            int level = playerCreature != null ? Mathf.Max(1, playerCreature.level) : 1;
            int formIndex = playerCreature != null ? playerCreature.currentFormIndex : 0;
            float multiplier = 1f;

            CreatureFormData form = staticCreature.GetFormByIndex(formIndex);
            if (form != null)
            {
                multiplier = Mathf.Max(0f, form.powerMultiplier);
            }

            return Mathf.RoundToInt(staticCreature.basePower * multiplier + level * 5);
        }

        private void OnDailyRewardClicked()
        {
            FindManagersIfNeeded();

            if (uiManager != null)
            {
                uiManager.ShowDailyRewardPopup();
            }
            else
            {
                Debug.Log("Daily reward clicked, but UIManager is not assigned yet.");
            }
        }

        private void OnHatchClicked()
        {
            if (uiManager != null)
            {
                uiManager.ShowHatchScreen();
            }
        }

        private void OnCollectionClicked()
        {
            if (uiManager != null)
            {
                uiManager.ShowCollectionScreen();
            }
        }

        private void OnExpeditionClicked()
        {
            if (uiManager != null)
            {
                uiManager.ShowExpeditionScreen();
            }
        }

        private void FindManagersIfNeeded()
        {
            if (gameManager == null)
            {
                gameManager = GameManager.Instance;
            }

            if (gameManager != null && !gameManager.IsInitialized)
            {
                gameManager.InitializeGame();
            }

            if (uiManager == null)
            {
                uiManager = FindObjectOfType<UIManager>();
            }
        }

        private void SetText(TMP_Text targetText, string value)
        {
            if (targetText != null)
            {
                targetText.text = value;
            }
        }

        private void SetImage(Image targetImage, Sprite sprite)
        {
            SetImage(targetImage, sprite, null, 1f, ref featuredCreatureAnimation);
        }

        private void SetImage(Image targetImage, Sprite sprite, Sprite[] idleFrames, float displayScale, ref CreatureSpriteAnimationUI animation)
        {
            if (targetImage == null)
            {
                return;
            }

            targetImage.rectTransform.localScale = Vector3.one * Mathf.Max(0.1f, displayScale);

            if (animation == null)
            {
                animation = targetImage.GetComponent<CreatureSpriteAnimationUI>();

                if (animation == null)
                {
                    animation = targetImage.gameObject.AddComponent<CreatureSpriteAnimationUI>();
                }
            }

            if (animation != null)
            {
                if (idleFrames != null && idleFrames.Length > 0)
                {
                    animation.Play(idleFrames, sprite);
                }
                else
                {
                    animation.Stop(sprite);
                }

                return;
            }

            targetImage.sprite = sprite;
            targetImage.enabled = sprite != null;
            targetImage.preserveAspect = true;
        }
    }
}
