using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using SpiritHatchers.Data;

namespace SpiritHatchers.UI
{
    public class CreatureCardUI : MonoBehaviour
    {
        [Header("Card References")]
        [SerializeField] private Button button;
        [SerializeField] private Image creatureImage;
        [SerializeField] private Image lockedOverlayImage;
        [SerializeField] private TMP_Text creatureNameText;
        [SerializeField] private TMP_Text rarityText;
        [SerializeField] private TMP_Text levelText;

        private CreatureStaticData creatureData;
        private PlayerCreatureData playerCreatureData;
        private Action<CreatureStaticData, PlayerCreatureData> onClicked;

        private void Awake()
        {
            FindReferencesIfNeeded();
            EnsureCardSize();
            EnsureChildLayout();
            EnsureVisibleGraphics();
        }

        private void Reset()
        {
            FindReferencesIfNeeded();
            EnsureCardSize();
            EnsureChildLayout();
            EnsureVisibleGraphics();
        }

        private void FindReferencesIfNeeded()
        {
            if (button == null)
            {
                button = GetComponent<Button>();
            }

            if (creatureImage == null)
            {
                creatureImage = FindChildComponent<Image>("CreatureImage");
            }

            if (lockedOverlayImage == null)
            {
                lockedOverlayImage = FindChildComponent<Image>("LockedOverlay");
            }

            if (creatureNameText == null)
            {
                creatureNameText = FindChildComponent<TMP_Text>("NameText");
            }

            if (rarityText == null)
            {
                rarityText = FindChildComponent<TMP_Text>("RarityText");
            }

            if (levelText == null)
            {
                levelText = FindChildComponent<TMP_Text>("LevelText");
            }
        }

        private T FindChildComponent<T>(string childName) where T : Component
        {
            Transform child = transform.Find(childName);
            return child != null ? child.GetComponent<T>() : null;
        }

        private void EnsureCardSize()
        {
            RectTransform rectTransform = transform as RectTransform;
            if (rectTransform == null)
            {
                return;
            }

            if (rectTransform.sizeDelta.x < 240f || rectTransform.sizeDelta.y < 280f)
            {
                rectTransform.sizeDelta = new Vector2(260f, 310f);
            }
        }

        private void EnsureChildLayout()
        {
            transform.SetAsLastSibling();

            SetChildRect(creatureImage, new Vector2(0.5f, 1f), new Vector2(0f, -95f), new Vector2(170f, 170f));
            SetChildRect(creatureNameText, new Vector2(0.5f, 0f), new Vector2(0f, 82f), new Vector2(230f, 44f));
            SetChildRect(rarityText, new Vector2(0.5f, 0f), new Vector2(0f, 46f), new Vector2(230f, 34f));
            SetChildRect(levelText, new Vector2(0.5f, 0f), new Vector2(0f, 16f), new Vector2(230f, 30f));

            if (creatureImage != null)
            {
                creatureImage.transform.SetSiblingIndex(0);
            }

            if (lockedOverlayImage != null)
            {
                RectTransform overlayRect = lockedOverlayImage.rectTransform;
                overlayRect.anchorMin = Vector2.zero;
                overlayRect.anchorMax = Vector2.one;
                overlayRect.anchoredPosition = Vector2.zero;
                overlayRect.sizeDelta = Vector2.zero;
                lockedOverlayImage.transform.SetAsLastSibling();
            }

            if (creatureNameText != null)
            {
                creatureNameText.transform.SetAsLastSibling();
            }

            if (rarityText != null)
            {
                rarityText.transform.SetAsLastSibling();
            }

            if (levelText != null)
            {
                levelText.transform.SetAsLastSibling();
            }
        }

        private void SetChildRect(Graphic graphic, Vector2 anchor, Vector2 anchoredPosition, Vector2 size)
        {
            if (graphic == null)
            {
                return;
            }

            RectTransform rectTransform = graphic.rectTransform;
            rectTransform.anchorMin = anchor;
            rectTransform.anchorMax = anchor;
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = anchoredPosition;
            rectTransform.sizeDelta = size;
        }

        private void EnsureVisibleGraphics()
        {
            Image cardImage = GetComponent<Image>();
            if (cardImage != null)
            {
                cardImage.enabled = true;
                cardImage.color = new Color(0.95f, 0.76f, 0.42f, 1f);
            }

            SetTextColor(creatureNameText);
            SetTextColor(rarityText);
            SetTextColor(levelText);

            if (creatureImage != null)
            {
                creatureImage.color = Color.white;
                creatureImage.enabled = true;
            }
        }

        private void SetTextColor(TMP_Text targetText)
        {
            if (targetText == null)
            {
                return;
            }

            targetText.enabled = true;
            targetText.color = new Color(0.22f, 0.12f, 0.07f, 1f);
        }

        private void OnEnable()
        {
            if (button != null)
            {
                button.onClick.AddListener(HandleClicked);
            }
        }

        private void OnDisable()
        {
            if (button != null)
            {
                button.onClick.RemoveListener(HandleClicked);
            }
        }

        public void Setup(
            CreatureStaticData creature,
            PlayerCreatureData playerCreature,
            Sprite lockedPlaceholderSprite,
            Action<CreatureStaticData, PlayerCreatureData> clickCallback)
        {
            FindReferencesIfNeeded();
            EnsureCardSize();
            EnsureChildLayout();
            EnsureVisibleGraphics();

            creatureData = creature;
            playerCreatureData = playerCreature;
            onClicked = clickCallback;

            bool isOwned = playerCreatureData != null;

            if (isOwned)
            {
                ShowOwnedCreature();
            }
            else
            {
                ShowLockedCreature(lockedPlaceholderSprite);
            }
        }

        private void ShowOwnedCreature()
        {
            CreatureFormData form = GetCurrentForm();
            Sprite sprite = form != null ? form.sprite : null;
            float displayScale = form != null ? form.displayScale : 1f;

            SetImage(creatureImage, sprite, displayScale);

            if (lockedOverlayImage != null)
            {
                lockedOverlayImage.gameObject.SetActive(false);
            }

            SetText(creatureNameText, creatureData != null ? creatureData.creatureName : "Unknown");
            SetText(rarityText, creatureData != null ? creatureData.rarity.ToString() : string.Empty);
            SetText(levelText, playerCreatureData != null ? $"Lv. {Mathf.Max(1, playerCreatureData.level)}" : "Lv. 1");
        }

        private void ShowLockedCreature(Sprite lockedPlaceholderSprite)
        {
            SetImage(creatureImage, lockedPlaceholderSprite, 1f);

            if (lockedOverlayImage != null)
            {
                lockedOverlayImage.gameObject.SetActive(true);
            }

            SetText(creatureNameText, "???");
            SetText(rarityText, string.Empty);
            SetText(levelText, string.Empty);
        }

        private CreatureFormData GetCurrentForm()
        {
            if (creatureData == null || creatureData.forms == null || creatureData.forms.Count == 0)
            {
                return null;
            }

            int formIndex = playerCreatureData != null ? playerCreatureData.currentFormIndex : 0;
            CreatureFormData form = creatureData.GetFormByIndex(formIndex);

            if (form == null)
            {
                form = creatureData.forms[0];
            }

            return form;
        }

        private void HandleClicked()
        {
            onClicked?.Invoke(creatureData, playerCreatureData);
        }

        private void SetImage(Image targetImage, Sprite sprite)
        {
            SetImage(targetImage, sprite, 1f);
        }

        private void SetImage(Image targetImage, Sprite sprite, float displayScale)
        {
            if (targetImage == null)
            {
                Debug.LogWarning($"{nameof(CreatureCardUI)} is missing an Image reference on {name}.");
                return;
            }

            targetImage.rectTransform.localScale = Vector3.one * Mathf.Max(0.1f, displayScale);
            targetImage.sprite = sprite;
            targetImage.enabled = true;
            targetImage.gameObject.SetActive(true);
            targetImage.preserveAspect = true;
        }

        private void SetText(TMP_Text targetText, string value)
        {
            if (targetText != null)
            {
                targetText.gameObject.SetActive(true);
                targetText.text = value;
            }
            else
            {
                Debug.LogWarning($"{nameof(CreatureCardUI)} is missing a TMP_Text reference on {name}.");
            }
        }
    }
}
