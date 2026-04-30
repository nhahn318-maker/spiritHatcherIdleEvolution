using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using SpiritHatchers.Data;
using SpiritHatchers.Expedition;

namespace SpiritHatchers.UI
{
    public class CreatureSelectPopupUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ExpeditionManager expeditionManager;
        [SerializeField] private CreatureDatabase creatureDatabase;

        [Header("Popup")]
        [SerializeField] private GameObject popupRoot;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text messageText;
        [SerializeField] private Transform creatureButtonParent;
        [SerializeField] private Button creatureButtonPrefab;
        [SerializeField] private Button closeButton;

        private ExpeditionData selectedExpedition;

        private void OnEnable()
        {
            if (closeButton != null)
            {
                closeButton.onClick.AddListener(Hide);
            }

            Hide();
        }

        private void OnDisable()
        {
            if (closeButton != null)
            {
                closeButton.onClick.RemoveListener(Hide);
            }
        }

        public void Show(ExpeditionData expedition)
        {
            FindReferencesIfNeeded();
            selectedExpedition = expedition;
            ClearCreatureButtons();

            if (popupRoot != null)
            {
                popupRoot.SetActive(true);
            }

            SetText(titleText, expedition != null ? $"Select Creature - {expedition.expeditionName}" : "Select Creature");
            SetText(messageText, string.Empty);

            if (expeditionManager == null)
            {
                SetText(messageText, "Expedition manager missing.");
                return;
            }

            List<PlayerCreatureData> availableCreatures = expeditionManager.GetAvailableOwnedCreatures();

            if (availableCreatures.Count == 0)
            {
                SetText(messageText, "No available creatures.");
                return;
            }

            for (int i = 0; i < availableCreatures.Count; i++)
            {
                CreateCreatureButton(availableCreatures[i]);
            }
        }

        public void Hide()
        {
            if (popupRoot != null)
            {
                popupRoot.SetActive(false);
            }
        }

        private void CreateCreatureButton(PlayerCreatureData creature)
        {
            if (creatureButtonParent == null || creatureButtonPrefab == null || creature == null)
            {
                return;
            }

            Button button = Instantiate(creatureButtonPrefab, creatureButtonParent);
            TMP_Text text = button.GetComponentInChildren<TMP_Text>();
            Image image = button.GetComponentInChildren<Image>();
            Image buttonImage = button.GetComponent<Image>();

            CreatureStaticData staticData = creatureDatabase != null
                ? creatureDatabase.GetCreatureById(creature.creatureId)
                : null;

            StyleCreatureButton(button, buttonImage, image, text);
            SetText(text, BuildCreatureLabel(staticData, creature));

            CreatureFormData form = GetCreatureForm(staticData, creature);
            Sprite sprite = form != null ? form.sprite : null;
            if (image != null && image.gameObject != button.gameObject)
            {
                float displayScale = form != null ? form.displayScale : 1f;
                image.rectTransform.localScale = Vector3.one * Mathf.Max(0.1f, displayScale);
                image.sprite = sprite;
                image.enabled = sprite != null;
                image.preserveAspect = true;
            }

            button.onClick.AddListener(() => SelectCreature(creature));
        }

        private void StyleCreatureButton(Button button, Image buttonImage, Image creatureImage, TMP_Text text)
        {
            RectTransform buttonRect = button.transform as RectTransform;
            if (buttonRect != null)
            {
                buttonRect.sizeDelta = new Vector2(560f, 120f);
            }

            LayoutElement layoutElement = button.GetComponent<LayoutElement>();
            if (layoutElement == null)
            {
                layoutElement = button.gameObject.AddComponent<LayoutElement>();
            }

            layoutElement.preferredWidth = 560f;
            layoutElement.preferredHeight = 120f;

            if (buttonImage != null)
            {
                buttonImage.color = new Color(0.95f, 0.76f, 0.42f, 1f);
            }

            if (creatureImage != null && creatureImage.gameObject != button.gameObject)
            {
                RectTransform imageRect = creatureImage.rectTransform;
                imageRect.anchorMin = new Vector2(0f, 0.5f);
                imageRect.anchorMax = new Vector2(0f, 0.5f);
                imageRect.pivot = new Vector2(0.5f, 0.5f);
                imageRect.anchoredPosition = new Vector2(70f, 0f);
                imageRect.sizeDelta = new Vector2(92f, 92f);
                creatureImage.color = Color.white;
                creatureImage.preserveAspect = true;
            }

            if (text != null)
            {
                RectTransform textRect = text.rectTransform;
                textRect.anchorMin = new Vector2(0f, 0.5f);
                textRect.anchorMax = new Vector2(1f, 0.5f);
                textRect.pivot = new Vector2(0f, 0.5f);
                textRect.anchoredPosition = new Vector2(132f, 0f);
                textRect.sizeDelta = new Vector2(-156f, 96f);

                text.enableAutoSizing = false;
                text.fontSize = 26f;
                text.alignment = TextAlignmentOptions.Left;
                text.color = new Color(0.22f, 0.12f, 0.07f, 1f);
            }
        }

        private void SelectCreature(PlayerCreatureData creature)
        {
            if (expeditionManager == null || selectedExpedition == null || creature == null)
            {
                return;
            }

            if (expeditionManager.StartExpedition(selectedExpedition, creature))
            {
                Hide();
            }
        }

        private string BuildCreatureLabel(CreatureStaticData staticData, PlayerCreatureData creature)
        {
            string name = staticData != null ? staticData.creatureName : creature.creatureId;
            string rarity = staticData != null ? staticData.rarity.ToString() : "Unknown";
            return $"{name}\n{rarity}  Lv. {Mathf.Max(1, creature.level)}";
        }

        private CreatureFormData GetCreatureForm(CreatureStaticData staticData, PlayerCreatureData creature)
        {
            if (staticData == null || staticData.forms == null || staticData.forms.Count == 0)
            {
                return null;
            }

            CreatureFormData form = staticData.GetFormByIndex(creature.currentFormIndex);
            if (form == null)
            {
                form = staticData.forms[0];
            }

            return form;
        }

        private void ClearCreatureButtons()
        {
            if (creatureButtonParent == null)
            {
                return;
            }

            for (int i = creatureButtonParent.childCount - 1; i >= 0; i--)
            {
                Destroy(creatureButtonParent.GetChild(i).gameObject);
            }
        }

        private void FindReferencesIfNeeded()
        {
            if (expeditionManager == null)
            {
                expeditionManager = ExpeditionManager.Instance;
            }

            if (expeditionManager == null)
            {
                expeditionManager = FindObjectOfType<ExpeditionManager>();
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
