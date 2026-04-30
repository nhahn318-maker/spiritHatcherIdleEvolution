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
        [SerializeField] private Image backgroundImage;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text messageText;
        [SerializeField] private TMP_Text enemyListText;
        [SerializeField] private Transform creatureButtonParent;
        [SerializeField] private Button creatureButtonPrefab;
        [SerializeField] private GameObject battleStageRoot;
        [SerializeField] private Image selectedCreatureImage;
        [SerializeField] private TMP_Text selectedCreatureText;
        [SerializeField] private Transform enemyPreviewParent;
        [SerializeField] private Button startButton;
        [SerializeField] private TMP_Text startButtonText;
        [SerializeField] private Button closeButton;

        private ExpeditionData selectedExpedition;
        private PlayerCreatureData selectedCreature;

        private void OnEnable()
        {
            if (startButton != null)
            {
                startButton.onClick.AddListener(StartSelectedExpedition);
            }

            if (closeButton != null)
            {
                closeButton.onClick.AddListener(Hide);
            }

            Hide();
        }

        private void OnDisable()
        {
            if (startButton != null)
            {
                startButton.onClick.RemoveListener(StartSelectedExpedition);
            }

            if (closeButton != null)
            {
                closeButton.onClick.RemoveListener(Hide);
            }
        }

        public void Show(ExpeditionData expedition)
        {
            FindReferencesIfNeeded();
            selectedExpedition = expedition;
            selectedCreature = null;
            ClearCreatureButtons();
            EnsureBattlePreviewLayout();
            RefreshSelectedCreaturePreview(null);
            RefreshEnemyPreview(expedition);

            if (popupRoot != null)
            {
                popupRoot.SetActive(true);
            }

            SetText(titleText, expedition != null ? $"Select Creature - {expedition.expeditionName}" : "Select Creature");
            ApplyExpeditionBackground(expedition);
            string summary = BuildExpeditionSummary(expedition);
            if (enemyListText == null)
            {
                summary += $"\n\n{BuildEnemyList(expedition)}";
            }

            SetText(messageText, summary);
            SetText(enemyListText, BuildEnemyList(expedition));

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
            if (selectedExpedition == null || creature == null)
            {
                return;
            }

            selectedCreature = creature;
            RefreshSelectedCreaturePreview(creature);
        }

        private void StartSelectedExpedition()
        {
            if (expeditionManager == null || selectedExpedition == null || selectedCreature == null)
            {
                SetText(selectedCreatureText, "Select a creature first.");
                return;
            }

            if (expeditionManager.StartExpedition(selectedExpedition, selectedCreature))
            {
                BattleScreenUI.ShowOrCreate(transform, selectedExpedition, selectedCreature, creatureDatabase);
                Hide();
            }
        }

        private string BuildCreatureLabel(CreatureStaticData staticData, PlayerCreatureData creature)
        {
            string name = staticData != null ? staticData.creatureName : creature.creatureId;
            string rarity = staticData != null ? staticData.rarity.ToString() : "Unknown";
            string skill = staticData != null && !string.IsNullOrEmpty(staticData.skillName)
                ? staticData.skillName
                : "Spirit Strike";
            int power = expeditionManager != null ? expeditionManager.GetCreaturePower(creature) : 0;
            return $"{name}\n{rarity}  Lv. {Mathf.Max(1, creature.level)}  Power: {power}  Skill: {skill}";
        }

        private void RefreshSelectedCreaturePreview(PlayerCreatureData creature)
        {
            if (startButton != null)
            {
                startButton.interactable = creature != null;
            }

            SetText(startButtonText, "Start");

            if (creature == null)
            {
                if (selectedCreatureImage != null)
                {
                    selectedCreatureImage.sprite = null;
                    selectedCreatureImage.enabled = true;
                    selectedCreatureImage.color = new Color(1f, 1f, 1f, 0.18f);
                }

                SetText(selectedCreatureText, "Select Creature");
                return;
            }

            CreatureStaticData staticData = creatureDatabase != null
                ? creatureDatabase.GetCreatureById(creature.creatureId)
                : null;
            CreatureFormData form = GetCreatureForm(staticData, creature);
            Sprite sprite = form != null ? form.sprite : null;
            float displayScale = form != null ? form.displayScale : 1f;

            if (selectedCreatureImage != null)
            {
                selectedCreatureImage.sprite = sprite;
                selectedCreatureImage.enabled = true;
                selectedCreatureImage.color = sprite != null ? Color.white : new Color(1f, 1f, 1f, 0.18f);
                selectedCreatureImage.rectTransform.localScale = Vector3.one * Mathf.Max(0.1f, displayScale);
                selectedCreatureImage.preserveAspect = true;
            }

            SetText(selectedCreatureText, BuildCreatureLabel(staticData, creature));
        }

        private void RefreshEnemyPreview(ExpeditionData expedition)
        {
            if (enemyPreviewParent == null)
            {
                return;
            }

            for (int i = enemyPreviewParent.childCount - 1; i >= 0; i--)
            {
                Destroy(enemyPreviewParent.GetChild(i).gameObject);
            }

            if (expedition == null || expedition.enemies == null || expedition.enemies.Count == 0)
            {
                CreateEnemyPreview("No Enemies", null, 0, 0, Color.gray);
                return;
            }

            for (int i = 0; i < expedition.enemies.Count; i++)
            {
                ExpeditionEnemyData enemy = expedition.enemies[i];
                if (enemy == null)
                {
                    continue;
                }

                string enemyName = string.IsNullOrEmpty(enemy.enemyName) ? "Unknown Enemy" : enemy.enemyName;
                CreateEnemyPreview(enemyName, enemy.sprite, enemy.maxHealth, enemy.attack, GetElementColor(enemy.element));
            }
        }

        private void CreateEnemyPreview(string enemyName, Sprite sprite, int health, int attack, Color color)
        {
            GameObject root = new GameObject("EnemyPreview", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            root.transform.SetParent(enemyPreviewParent, false);

            Image panelImage = root.GetComponent<Image>();
            panelImage.color = new Color(0.12f, 0.1f, 0.08f, 0.62f);

            LayoutElement layoutElement = root.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = 118f;
            layoutElement.flexibleWidth = 1f;

            Image image = CreateRuntimeImage(root.transform, "EnemyImage", color);
            SetAnchored(image.rectTransform, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(58f, 0f), new Vector2(82f, 82f));
            image.sprite = sprite;
            image.preserveAspect = true;

            TMP_Text label = CreateRuntimeText(root.transform, "EnemyText", BuildEnemyPreviewText(enemyName, health, attack), 20f, TextAlignmentOptions.Left);
            SetAnchored(label.rectTransform, new Vector2(0f, 0.5f), new Vector2(1f, 0.5f), new Vector2(156f, 0f), new Vector2(-180f, 88f));
        }

        private string BuildEnemyPreviewText(string enemyName, int health, int attack)
        {
            if (health <= 0 && attack <= 0)
            {
                return enemyName;
            }

            return $"{enemyName}\nHP {health}  ATK {attack}";
        }

        private Color GetElementColor(CreatureElement element)
        {
            switch (element)
            {
                case CreatureElement.Fire:
                    return new Color(0.95f, 0.33f, 0.16f, 1f);
                case CreatureElement.Water:
                    return new Color(0.25f, 0.55f, 0.95f, 1f);
                case CreatureElement.Nature:
                    return new Color(0.26f, 0.7f, 0.33f, 1f);
                case CreatureElement.Shadow:
                    return new Color(0.35f, 0.25f, 0.55f, 1f);
                default:
                    return Color.gray;
            }
        }

        private string BuildExpeditionSummary(ExpeditionData expedition)
        {
            if (expedition == null)
            {
                return string.Empty;
            }

            string description = string.IsNullOrEmpty(expedition.description)
                ? "Choose one creature. It will auto battle through this scene while you wait."
                : expedition.description;

            return $"{description}\nRecommended Power: {expedition.recommendedPower}";
        }

        private string BuildEnemyList(ExpeditionData expedition)
        {
            if (expedition == null || expedition.enemies == null || expedition.enemies.Count == 0)
            {
                return "Enemies: None";
            }

            string text = "Enemies:";
            for (int i = 0; i < expedition.enemies.Count; i++)
            {
                ExpeditionEnemyData enemy = expedition.enemies[i];
                if (enemy == null)
                {
                    continue;
                }

                string enemyName = string.IsNullOrEmpty(enemy.enemyName) ? "Unknown Enemy" : enemy.enemyName;
                text += $"\n{i + 1}. {enemyName}  HP {enemy.maxHealth}  ATK {enemy.attack}";
            }

            return text;
        }

        private void ApplyExpeditionBackground(ExpeditionData expedition)
        {
            if (expedition == null)
            {
                return;
            }

            if (backgroundImage == null && popupRoot != null)
            {
                backgroundImage = popupRoot.GetComponent<Image>();
            }

            if (backgroundImage == null)
            {
                return;
            }

            backgroundImage.sprite = expedition.backgroundSprite;
            backgroundImage.color = expedition.backgroundSprite != null ? Color.white : expedition.sceneTintColor;
            backgroundImage.enabled = true;
            backgroundImage.preserveAspect = true;
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

        private void EnsureBattlePreviewLayout()
        {
            if (popupRoot == null || battleStageRoot != null)
            {
                return;
            }

            battleStageRoot = new GameObject("BattlePreview", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            battleStageRoot.transform.SetParent(popupRoot.transform, false);
            Image stageImage = battleStageRoot.GetComponent<Image>();
            stageImage.color = new Color(0f, 0f, 0f, 0.22f);
            SetAnchored(
                battleStageRoot.GetComponent<RectTransform>(),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0f, 100f),
                new Vector2(740f, 360f));

            selectedCreatureImage = CreateRuntimeImage(battleStageRoot.transform, "SelectedCreatureImage", new Color(1f, 1f, 1f, 0.18f));
            SetAnchored(selectedCreatureImage.rectTransform, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(165f, 42f), new Vector2(220f, 220f));

            selectedCreatureText = CreateRuntimeText(battleStageRoot.transform, "SelectedCreatureText", "Select Creature", 22f, TextAlignmentOptions.Center);
            SetAnchored(selectedCreatureText.rectTransform, new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(165f, 58f), new Vector2(280f, 80f));

            GameObject enemyList = new GameObject("EnemyPreviewList", typeof(RectTransform));
            enemyList.transform.SetParent(battleStageRoot.transform, false);
            SetAnchored(
                enemyList.GetComponent<RectTransform>(),
                new Vector2(1f, 0.5f),
                new Vector2(1f, 0.5f),
                new Vector2(-205f, 0f),
                new Vector2(350f, 310f));
            VerticalLayoutGroup enemyLayout = enemyList.AddComponent<VerticalLayoutGroup>();
            enemyLayout.spacing = 10f;
            enemyLayout.childForceExpandWidth = true;
            enemyLayout.childForceExpandHeight = false;
            enemyPreviewParent = enemyList.transform;

            if (creatureButtonParent != null)
            {
                RectTransform buttonListRect = creatureButtonParent as RectTransform;
                if (buttonListRect != null)
                {
                    SetAnchored(buttonListRect, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 230f), new Vector2(700f, 260f));
                }
            }

            startButton = CreateRuntimeButton(popupRoot.transform, "StartButton", "Start");
            SetAnchored(startButton.GetComponent<RectTransform>(), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(-205f, 90f), new Vector2(260f, 84f));
            startButtonText = startButton.GetComponentInChildren<TMP_Text>();
            startButton.onClick.AddListener(StartSelectedExpedition);
        }

        private Image CreateRuntimeImage(Transform parent, string name, Color color)
        {
            GameObject obj = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            obj.transform.SetParent(parent, false);
            Image image = obj.GetComponent<Image>();
            image.color = color;
            image.preserveAspect = true;
            return image;
        }

        private TMP_Text CreateRuntimeText(Transform parent, string name, string value, float fontSize, TextAlignmentOptions alignment)
        {
            GameObject obj = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
            obj.transform.SetParent(parent, false);
            TMP_Text text = obj.GetComponent<TMP_Text>();
            text.text = value;
            text.fontSize = fontSize;
            text.alignment = alignment;
            text.color = new Color(0.98f, 0.86f, 0.58f, 1f);
            text.enableWordWrapping = true;
            return text;
        }

        private Button CreateRuntimeButton(Transform parent, string name, string label)
        {
            Image image = CreateRuntimeImage(parent, name, new Color(0.93f, 0.58f, 0.22f, 1f));
            Button button = image.gameObject.AddComponent<Button>();
            TMP_Text text = CreateRuntimeText(button.transform, "Text (TMP)", label, 30f, TextAlignmentOptions.Center);
            StretchFull(text.rectTransform);
            text.color = new Color(0.22f, 0.12f, 0.07f, 1f);
            text.raycastTarget = false;
            return button;
        }

        private void SetAnchored(RectTransform rectTransform, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition, Vector2 sizeDelta)
        {
            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = anchoredPosition;
            rectTransform.sizeDelta = sizeDelta;
        }

        private void StretchFull(RectTransform rectTransform)
        {
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
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
