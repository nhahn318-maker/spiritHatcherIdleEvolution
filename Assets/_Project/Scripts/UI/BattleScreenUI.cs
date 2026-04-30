using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using SpiritHatchers.Data;
using SpiritHatchers.Expedition;

namespace SpiritHatchers.UI
{
    public class BattleScreenUI : MonoBehaviour
    {
        private const int PlayerSlotCount = 3;
        private const int EnemySlotCount = 3;
        private const float CombatantSlotSize = 260f;
        private const float CombatantSpriteScale = 1.24f;
        private const float BaseAttackIntervalSeconds = 2f;
        private const float BaselineSpeed = 50f;
        private const float MeleeContactOffset = 44f;
        private const float MeleeLungeDuration = 0.34f;
        private const float MeleeReturnDuration = 0.28f;

        [SerializeField] private CreatureDatabase creatureDatabase;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private Transform enemyRoot;
        [SerializeField] private Transform playerRoot;
        [SerializeField] private Button closeButton;

        private readonly List<GameObject> spawnedViews = new List<GameObject>();
        private readonly List<CombatantView> playerCombatants = new List<CombatantView>();
        private readonly List<CombatantView> enemyCombatants = new List<CombatantView>();
        private Coroutine attackLoopCoroutine;

        private void Awake()
        {
            if (closeButton != null)
            {
                closeButton.onClick.AddListener(Hide);
            }
        }

        private void OnDestroy()
        {
            if (closeButton != null)
            {
                closeButton.onClick.RemoveListener(Hide);
            }
        }

        public static BattleScreenUI ShowOrCreate(Transform parent, ExpeditionData expedition, PlayerCreatureData selectedCreature, CreatureDatabase database)
        {
            BattleScreenUI battleScreen = FindObjectOfType<BattleScreenUI>(true);

            if (battleScreen == null)
            {
                Canvas canvas = parent != null ? parent.GetComponentInParent<Canvas>() : FindObjectOfType<Canvas>();
                Transform screenParent = canvas != null ? canvas.transform : parent;
                battleScreen = CreateRuntimeScreen(screenParent);
            }

            battleScreen.Show(expedition, selectedCreature, database);
            return battleScreen;
        }

        public void Show(ExpeditionData expedition, PlayerCreatureData selectedCreature, CreatureDatabase database)
        {
            creatureDatabase = database != null ? database : creatureDatabase;
            EnsureLayout();
            ClearSpawnedViews();
            ApplyBackground(expedition);
            SetText(titleText, expedition != null ? expedition.expeditionName : "Battle");
            CreateEnemies(expedition);
            CreatePlayerSlots(selectedCreature);
            gameObject.SetActive(true);
            StartAttackLoop();
        }

        public void Hide()
        {
            StopAttackLoop();
            gameObject.SetActive(false);
        }

        private static BattleScreenUI CreateRuntimeScreen(Transform parent)
        {
            GameObject root = new GameObject("BattleScreen", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(BattleScreenUI));
            root.transform.SetParent(parent, false);
            StretchFull(root.GetComponent<RectTransform>());
            root.SetActive(false);
            return root.GetComponent<BattleScreenUI>();
        }

        private void EnsureLayout()
        {
            RectTransform rootRect = transform as RectTransform;
            if (rootRect != null)
            {
                StretchFull(rootRect);
            }

            if (backgroundImage == null)
            {
                backgroundImage = GetComponent<Image>();
            }

            if (enemyRoot == null)
            {
                GameObject enemies = new GameObject("EnemySlots", typeof(RectTransform));
                enemies.transform.SetParent(transform, false);
                SetAnchored(enemies.GetComponent<RectTransform>(), new Vector2(0f, 0.52f), new Vector2(1f, 0.84f), Vector2.zero, Vector2.zero);
                HorizontalLayoutGroup layout = enemies.AddComponent<HorizontalLayoutGroup>();
                layout.childAlignment = TextAnchor.MiddleCenter;
                layout.spacing = 28f;
                layout.padding = new RectOffset(80, 80, 20, 20);
                layout.childForceExpandWidth = false;
                layout.childForceExpandHeight = false;
                enemyRoot = enemies.transform;
            }

            if (playerRoot == null)
            {
                GameObject players = new GameObject("PlayerSlots", typeof(RectTransform));
                players.transform.SetParent(transform, false);
                SetAnchored(players.GetComponent<RectTransform>(), new Vector2(0f, 0.16f), new Vector2(1f, 0.48f), Vector2.zero, Vector2.zero);
                HorizontalLayoutGroup layout = players.AddComponent<HorizontalLayoutGroup>();
                layout.childAlignment = TextAnchor.MiddleCenter;
                layout.spacing = 28f;
                layout.padding = new RectOffset(80, 80, 20, 20);
                layout.childForceExpandWidth = false;
                layout.childForceExpandHeight = false;
                playerRoot = players.transform;
            }

            if (titleText == null)
            {
                titleText = CreateText(transform, "BattleTitle", "Battle", 44f, TextAlignmentOptions.Center);
                SetAnchored(titleText.rectTransform, new Vector2(0f, 0.91f), new Vector2(1f, 0.98f), Vector2.zero, Vector2.zero);
            }

            if (closeButton == null)
            {
                closeButton = CreateButton(transform, "BackButton", "Back");
                SetAnchored(closeButton.GetComponent<RectTransform>(), new Vector2(0.03f, 0.91f), new Vector2(0.25f, 0.98f), Vector2.zero, Vector2.zero);
                closeButton.onClick.AddListener(Hide);
            }
        }

        private void ApplyBackground(ExpeditionData expedition)
        {
            if (backgroundImage == null)
            {
                return;
            }

            backgroundImage.sprite = expedition != null ? expedition.backgroundSprite : null;
            backgroundImage.color = expedition != null && expedition.backgroundSprite == null ? expedition.sceneTintColor : Color.white;
            backgroundImage.preserveAspect = false;
            backgroundImage.enabled = true;
        }

        private void CreateEnemies(ExpeditionData expedition)
        {
            for (int i = 0; i < EnemySlotCount; i++)
            {
                ExpeditionEnemyData enemy = expedition != null && expedition.enemies != null && i < expedition.enemies.Count
                    ? expedition.enemies[i]
                    : null;

                if (enemy == null)
                {
                    CreateEmptyEnemySlot(i + 1);
                    continue;
                }

                string enemyName = string.IsNullOrEmpty(enemy.enemyName) ? "Enemy" : enemy.enemyName;
                CreatureStaticData enemyCreatureData = creatureDatabase != null
                    ? creatureDatabase.GetCreatureById(enemy.enemyId)
                    : null;
                CreatureFormData enemyForm = GetDefaultCreatureForm(enemyCreatureData);
                Sprite enemySprite = enemyForm != null && enemyForm.sprite != null ? enemyForm.sprite : enemy.sprite;
                Sprite[] enemyIdleFrames = enemyForm != null ? enemyForm.idleFrames : null;
                float enemyDisplayScale = enemyForm != null ? enemyForm.displayScale : 1f;
                int maxHealth = Mathf.Max(1, enemy.maxHealth);
                int attackPower = Mathf.Max(1, enemy.attack);

                CombatantView combatant = CreateCombatant(
                    enemyRoot,
                    enemyName,
                    enemySprite,
                    GetElementColor(enemy.element),
                    CombatantSlotSize,
                    true,
                    enemyDisplayScale,
                    enemyIdleFrames,
                    enemyCreatureData != null ? enemyCreatureData.attackFrames : null,
                    enemyCreatureData != null ? enemyCreatureData.skillRange : CreatureSkillRange.Melee,
                    enemyCreatureData != null ? enemyCreatureData.speed : BaselineSpeed,
                    maxHealth,
                    attackPower);
                enemyCombatants.Add(combatant);
            }
        }

        private void CreateEmptyEnemySlot(int slotNumber)
        {
            GameObject slot = new GameObject($"EmptyEnemySlot{slotNumber}", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            slot.transform.SetParent(enemyRoot, false);
            spawnedViews.Add(slot);

            Image image = slot.GetComponent<Image>();
            image.color = new Color(1f, 1f, 1f, 0.12f);

            LayoutElement layout = slot.AddComponent<LayoutElement>();
            layout.preferredWidth = 190f;
            layout.preferredHeight = 190f;
            layout.flexibleWidth = 0f;
            layout.flexibleHeight = 0f;

            TMP_Text label = CreateText(slot.transform, "EmptyText", "Empty", 22f, TextAlignmentOptions.Center);
            label.color = new Color(1f, 0.92f, 0.72f, 0.5f);
            StretchFull(label.rectTransform);
        }

        private void CreatePlayerSlots(PlayerCreatureData selectedCreature)
        {
            for (int i = 0; i < PlayerSlotCount; i++)
            {
                if (i == 0 && selectedCreature != null)
                {
                    CreatureStaticData staticData = creatureDatabase != null
                        ? creatureDatabase.GetCreatureById(selectedCreature.creatureId)
                        : null;
                    CreatureFormData form = GetCreatureForm(staticData, selectedCreature);
                    string creatureName = staticData != null ? staticData.creatureName : selectedCreature.creatureId;
                    int maxHealth = Mathf.Max(1, staticData != null ? staticData.basePower * 3 + selectedCreature.level * 5 : 30);
                    int attackPower = Mathf.Max(1, Mathf.RoundToInt((staticData != null ? staticData.basePower : 10) * (staticData != null ? staticData.skillPowerMultiplier : 1f)));

                    CombatantView combatant = CreateCombatant(
                        playerRoot,
                        creatureName,
                        form != null ? form.sprite : null,
                        new Color(1f, 0.82f, 0.38f, 0.48f),
                        CombatantSlotSize,
                        false,
                        form != null ? form.displayScale : 1f,
                        form != null ? form.idleFrames : null,
                        staticData != null ? staticData.attackFrames : null,
                        staticData != null ? staticData.skillRange : CreatureSkillRange.Melee,
                        staticData != null ? staticData.speed : BaselineSpeed,
                        maxHealth,
                        attackPower);
                    playerCombatants.Add(combatant);
                    continue;
                }

                CreateEmptyPlayerSlot(i + 1);
            }
        }

        private void CreateEmptyPlayerSlot(int slotNumber)
        {
            GameObject slot = new GameObject($"EmptySlot{slotNumber}", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            slot.transform.SetParent(playerRoot, false);
            spawnedViews.Add(slot);

            Image image = slot.GetComponent<Image>();
            image.color = new Color(1f, 1f, 1f, 0.16f);

            LayoutElement layout = slot.AddComponent<LayoutElement>();
            layout.preferredWidth = 220f;
            layout.preferredHeight = 220f;
            layout.flexibleWidth = 0f;
            layout.flexibleHeight = 0f;

            TMP_Text label = CreateText(slot.transform, "EmptyText", "Empty", 24f, TextAlignmentOptions.Center);
            label.color = new Color(1f, 0.92f, 0.72f, 0.62f);
            StretchFull(label.rectTransform);
        }

        private CombatantView CreateCombatant(
            Transform parent,
            string labelText,
            Sprite sprite,
            Color fallbackColor,
            float preferredSize,
            bool enemy,
            float displayScale = 1f,
            Sprite[] idleFrames = null,
            Sprite[] attackFrames = null,
            CreatureSkillRange skillRange = CreatureSkillRange.Melee,
            float speed = BaselineSpeed,
            int maxHealth = 1,
            int attackPower = 1)
        {
            GameObject root = new GameObject(labelText, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            root.transform.SetParent(parent, false);
            spawnedViews.Add(root);

            Image panel = root.GetComponent<Image>();
            panel.color = sprite != null ? Color.clear : new Color(0f, 0f, 0f, 0.18f);

            LayoutElement layout = root.AddComponent<LayoutElement>();
            layout.preferredWidth = preferredSize;
            layout.preferredHeight = preferredSize;
            layout.flexibleWidth = 0f;
            layout.flexibleHeight = 0f;

            Image spriteImage = CreateImage(root.transform, "Sprite", fallbackColor);
            SetAnchored(spriteImage.rectTransform, new Vector2(0f, 0.12f), new Vector2(1f, 1f), Vector2.zero, Vector2.zero);
            spriteImage.sprite = sprite;
            spriteImage.color = sprite != null ? Color.white : fallbackColor;
            spriteImage.preserveAspect = true;
            spriteImage.rectTransform.localScale = Vector3.one * Mathf.Max(0.1f, displayScale * CombatantSpriteScale);

            CreatureSpriteAnimationUI animation = spriteImage.gameObject.AddComponent<CreatureSpriteAnimationUI>();
            animation.Play(idleFrames, sprite);

            TMP_Text label = CreateText(root.transform, "Name", labelText, enemy ? 20f : 22f, TextAlignmentOptions.Center);
            SetAnchored(label.rectTransform, new Vector2(0f, 0f), new Vector2(1f, 0.18f), Vector2.zero, Vector2.zero);

            CombatantView combatant = new CombatantView
            {
                root = root.GetComponent<RectTransform>(),
                spriteImage = spriteImage,
                animation = animation,
                nameLabel = label,
                displayName = labelText,
                idleFrames = idleFrames,
                attackFrames = attackFrames,
                fallbackSprite = sprite,
                isEnemy = enemy,
                skillRange = skillRange,
                speed = Mathf.Max(1f, speed),
                maxHealth = Mathf.Max(1, maxHealth),
                currentHealth = Mathf.Max(1, maxHealth),
                attackPower = Mathf.Max(1, attackPower),
                attackTimer = 0f
            };
            UpdateCombatantLabel(combatant);
            return combatant;
        }

        private CreatureFormData GetCreatureForm(CreatureStaticData staticData, PlayerCreatureData creature)
        {
            if (staticData == null || creature == null || staticData.forms == null || staticData.forms.Count == 0)
            {
                return null;
            }

            CreatureFormData form = staticData.GetFormByIndex(creature.currentFormIndex);
            return form != null ? form : staticData.forms[0];
        }

        private CreatureFormData GetDefaultCreatureForm(CreatureStaticData staticData)
        {
            if (staticData == null || staticData.forms == null || staticData.forms.Count == 0)
            {
                return null;
            }

            CreatureFormData form = staticData.GetFormByIndex(0);
            return form != null ? form : staticData.forms[0];
        }

        private void ClearSpawnedViews()
        {
            StopAttackLoop();
            playerCombatants.Clear();
            enemyCombatants.Clear();

            for (int i = 0; i < spawnedViews.Count; i++)
            {
                if (spawnedViews[i] != null)
                {
                    Destroy(spawnedViews[i]);
                }
            }

            spawnedViews.Clear();
        }

        private void StartAttackLoop()
        {
            StopAttackLoop();
            attackLoopCoroutine = StartCoroutine(AutoAttackLoop());
        }

        private void StopAttackLoop()
        {
            if (attackLoopCoroutine != null)
            {
                StopCoroutine(attackLoopCoroutine);
                attackLoopCoroutine = null;
            }
        }

        private IEnumerator AutoAttackLoop()
        {
            while (true)
            {
                TickCombatants(playerCombatants, enemyCombatants);
                TickCombatants(enemyCombatants, playerCombatants);
                yield return null;
            }
        }

        private void TickCombatants(List<CombatantView> attackers, List<CombatantView> defenders)
        {
            for (int i = 0; i < attackers.Count; i++)
            {
                CombatantView attacker = attackers[i];
                if (attacker == null || !attacker.IsAlive || attacker.isAttacking)
                {
                    continue;
                }

                CombatantView target = GetFirstLivingCombatant(defenders);
                if (target == null)
                {
                    continue;
                }

                attacker.attackTimer += Time.deltaTime;
                float attackInterval = CalculateAttackInterval(attacker.speed);
                if (attacker.attackTimer < attackInterval)
                {
                    continue;
                }

                attacker.attackTimer = 0f;
                StartCoroutine(PerformAttack(attacker, target));
            }
        }

        private IEnumerator PerformAttack(CombatantView attacker, CombatantView target)
        {
            attacker.isAttacking = true;

            Vector3 startPosition = attacker.spriteImage.rectTransform.localPosition;
            if (attacker.skillRange == CreatureSkillRange.Melee)
            {
                Vector3 lungePosition = GetMeleeContactPosition(attacker, target, startPosition);
                yield return MoveSprite(attacker.spriteImage.rectTransform, startPosition, lungePosition, MeleeLungeDuration);
                yield return PlayAttackFrames(attacker);
                ApplyDamage(target, attacker.attackPower);
                yield return MoveSprite(attacker.spriteImage.rectTransform, attacker.spriteImage.rectTransform.localPosition, startPosition, MeleeReturnDuration);
            }
            else
            {
                yield return PlayAttackFrames(attacker);
                ApplyDamage(target, attacker.attackPower);
            }

            attacker.spriteImage.rectTransform.localPosition = startPosition;
            attacker.animation.Play(attacker.idleFrames, attacker.fallbackSprite);
            attacker.isAttacking = false;
        }

        private Vector3 GetMeleeContactPosition(CombatantView attacker, CombatantView target, Vector3 attackerStartLocalPosition)
        {
            if (attacker == null || target == null || attacker.spriteImage == null || target.spriteImage == null)
            {
                return attackerStartLocalPosition;
            }

            RectTransform attackerSprite = attacker.spriteImage.rectTransform;
            RectTransform attackerParent = attackerSprite.parent as RectTransform;
            RectTransform targetSprite = target.spriteImage.rectTransform;

            if (attackerParent == null)
            {
                return attackerStartLocalPosition;
            }

            Vector3 targetWorldPosition = targetSprite.TransformPoint(targetSprite.rect.center);
            Vector3 targetLocalPosition = attackerParent.InverseTransformPoint(targetWorldPosition);
            Vector3 direction = targetLocalPosition - attackerStartLocalPosition;

            if (direction.sqrMagnitude <= 0.01f)
            {
                return attackerStartLocalPosition;
            }

            direction.Normalize();
            return targetLocalPosition - direction * MeleeContactOffset;
        }

        private IEnumerator PlayAttackFrames(CombatantView attacker)
        {
            if (attacker.attackFrames == null || attacker.attackFrames.Length == 0)
            {
                yield return new WaitForSeconds(0.16f);
                yield break;
            }

            attacker.animation.Stop(attacker.fallbackSprite);
            float frameDuration = 0.045f;

            for (int i = 0; i < attacker.attackFrames.Length; i++)
            {
                if (attacker.attackFrames[i] != null)
                {
                    attacker.spriteImage.sprite = attacker.attackFrames[i];
                    attacker.spriteImage.enabled = true;
                }

                yield return new WaitForSeconds(frameDuration);
            }
        }

        private IEnumerator MoveSprite(RectTransform spriteTransform, Vector3 from, Vector3 to, float duration)
        {
            float timer = 0f;
            while (timer < duration)
            {
                timer += Time.deltaTime;
                float t = Mathf.Clamp01(timer / Mathf.Max(0.01f, duration));
                spriteTransform.localPosition = Vector3.Lerp(from, to, t);
                yield return null;
            }

            spriteTransform.localPosition = to;
        }

        private void ApplyDamage(CombatantView target, int damage)
        {
            if (target == null || !target.IsAlive)
            {
                return;
            }

            target.currentHealth = Mathf.Max(0, target.currentHealth - Mathf.Max(1, damage));
            UpdateCombatantLabel(target);

            if (!target.IsAlive)
            {
                target.spriteImage.color = new Color(1f, 1f, 1f, 0.28f);
                target.animation.Stop(target.fallbackSprite);
            }
        }

        private CombatantView GetFirstLivingCombatant(List<CombatantView> combatants)
        {
            for (int i = 0; i < combatants.Count; i++)
            {
                if (combatants[i] != null && combatants[i].IsAlive)
                {
                    return combatants[i];
                }
            }

            return null;
        }

        private float CalculateAttackInterval(float speed)
        {
            return Mathf.Clamp(BaseAttackIntervalSeconds * BaselineSpeed / Mathf.Max(1f, speed), 0.35f, 6f);
        }

        private void UpdateCombatantLabel(CombatantView combatant)
        {
            if (combatant == null || combatant.nameLabel == null)
            {
                return;
            }

            combatant.nameLabel.text = $"{combatant.displayName}\nHP {combatant.currentHealth}/{combatant.maxHealth}";
        }

        private Color GetElementColor(CreatureElement element)
        {
            switch (element)
            {
                case CreatureElement.Fire:
                    return new Color(0.95f, 0.33f, 0.16f, 0.75f);
                case CreatureElement.Water:
                    return new Color(0.25f, 0.55f, 0.95f, 0.75f);
                case CreatureElement.Nature:
                    return new Color(0.26f, 0.7f, 0.33f, 0.75f);
                case CreatureElement.Shadow:
                    return new Color(0.35f, 0.25f, 0.55f, 0.75f);
                default:
                    return new Color(0.45f, 0.45f, 0.45f, 0.75f);
            }
        }

        private static Image CreateImage(Transform parent, string name, Color color)
        {
            GameObject obj = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            obj.transform.SetParent(parent, false);
            Image image = obj.GetComponent<Image>();
            image.color = color;
            image.preserveAspect = true;
            return image;
        }

        private static TMP_Text CreateText(Transform parent, string name, string value, float fontSize, TextAlignmentOptions alignment)
        {
            GameObject obj = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
            obj.transform.SetParent(parent, false);
            TMP_Text text = obj.GetComponent<TMP_Text>();
            text.text = value;
            text.fontSize = fontSize;
            text.alignment = alignment;
            text.color = new Color(1f, 0.91f, 0.66f, 1f);
            text.enableWordWrapping = true;
            return text;
        }

        private static Button CreateButton(Transform parent, string name, string label)
        {
            Image image = CreateImage(parent, name, new Color(0.93f, 0.58f, 0.22f, 0.95f));
            Button button = image.gameObject.AddComponent<Button>();
            TMP_Text text = CreateText(button.transform, "Text (TMP)", label, 28f, TextAlignmentOptions.Center);
            StretchFull(text.rectTransform);
            text.color = new Color(0.22f, 0.12f, 0.07f, 1f);
            text.raycastTarget = false;
            return button;
        }

        private static void SetText(TMP_Text targetText, string value)
        {
            if (targetText != null)
            {
                targetText.text = value;
            }
        }

        private static void SetAnchored(RectTransform rectTransform, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition, Vector2 sizeDelta)
        {
            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = anchoredPosition;
            rectTransform.sizeDelta = sizeDelta;
        }

        private static void StretchFull(RectTransform rectTransform)
        {
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
        }

        private class CombatantView
        {
            public RectTransform root;
            public Image spriteImage;
            public CreatureSpriteAnimationUI animation;
            public TMP_Text nameLabel;
            public string displayName;
            public Sprite[] idleFrames;
            public Sprite[] attackFrames;
            public Sprite fallbackSprite;
            public bool isEnemy;
            public CreatureSkillRange skillRange;
            public float speed;
            public int maxHealth;
            public int currentHealth;
            public int attackPower;
            public float attackTimer;
            public bool isAttacking;

            public bool IsAlive => currentHealth > 0;
        }
    }
}
