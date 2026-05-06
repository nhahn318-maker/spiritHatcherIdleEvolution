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
        private const float CombatantSlotSize = 108f;
        private const float CombatantSpriteScale = 3f;
        private const float BaseAttackIntervalSeconds = 2f;
        private const float BaselineSpeed = 50f;
        private const float MeleeContactOffset = 18f;
        private const float MeleeLungeDuration = 0.34f;
        private const float ImpactFrameDuration = 0.045f;
        private const float ImpactEffectSize = 152f;
        private const float OrbitHitEffectFrameDuration = 0.08f;
        private const float OrbitHitEffectSize = 220f;
        private const float ProjectileFrameDuration = 0.045f;
        private const float ProjectileSize = 76f;
        private const float ProjectileSpeed = 900f;
        private const float ProjectileStartOffset = 36f;
        private const float OrbitEffectScale = 1.65f;
        private const float DefaultOrbitOrbSize = 34f;
        private const float DefaultOrbitOrbRadius = 44f;
        private const float DefaultOrbitOrbSpeedDegrees = 180f;
        private const float DefaultOrbitOrbHitRadius = 30f;
        private const float DefaultOrbitOrbHitCooldown = 0.55f;
        private const float UltimateRageGainOnAttack = 34f;
        private const float UltimateRageGainWhenHit = 22f;
        private const float UltimateFrameDuration = 0.055f;

        [SerializeField] private CreatureDatabase creatureDatabase;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private Transform enemyRoot;
        [SerializeField] private Transform playerRoot;
        [SerializeField] private Button closeButton;
        [SerializeField] private bool debugClickUltimate = true;

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
            ForceCombatantLayout();
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
                SetAnchored(enemies.GetComponent<RectTransform>(), new Vector2(0.54f, 0.08f), new Vector2(0.98f, 0.9f), Vector2.zero, Vector2.zero);
                VerticalLayoutGroup layout = enemies.AddComponent<VerticalLayoutGroup>();
                layout.childAlignment = TextAnchor.MiddleCenter;
                layout.spacing = 28f;
                layout.padding = new RectOffset(0, 0, 0, 0);
                layout.childForceExpandWidth = false;
                layout.childForceExpandHeight = false;
                enemyRoot = enemies.transform;
            }
            ConfigureCombatantSlotRoot(enemyRoot, new Vector2(0.54f, 0.08f), new Vector2(0.98f, 0.9f));

            if (playerRoot == null)
            {
                GameObject players = new GameObject("PlayerSlots", typeof(RectTransform));
                players.transform.SetParent(transform, false);
                SetAnchored(players.GetComponent<RectTransform>(), new Vector2(0.02f, 0.08f), new Vector2(0.46f, 0.9f), Vector2.zero, Vector2.zero);
                VerticalLayoutGroup layout = players.AddComponent<VerticalLayoutGroup>();
                layout.childAlignment = TextAnchor.MiddleCenter;
                layout.spacing = 28f;
                layout.padding = new RectOffset(0, 0, 0, 0);
                layout.childForceExpandWidth = false;
                layout.childForceExpandHeight = false;
                playerRoot = players.transform;
            }
            ConfigureCombatantSlotRoot(playerRoot, new Vector2(0.02f, 0.08f), new Vector2(0.46f, 0.9f));

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

        private void ConfigureCombatantSlotRoot(Transform slotRoot, Vector2 anchorMin, Vector2 anchorMax)
        {
            RectTransform rectTransform = slotRoot as RectTransform;
            if (rectTransform == null)
            {
                return;
            }

            SetAnchored(rectTransform, anchorMin, anchorMax, Vector2.zero, Vector2.zero);

            HorizontalLayoutGroup horizontalLayout = slotRoot.GetComponent<HorizontalLayoutGroup>();
            if (horizontalLayout != null)
            {
                horizontalLayout.enabled = false;
            }

            VerticalLayoutGroup verticalLayout = slotRoot.GetComponent<VerticalLayoutGroup>();
            if (verticalLayout == null)
            {
                verticalLayout = slotRoot.gameObject.AddComponent<VerticalLayoutGroup>();
            }

            verticalLayout.enabled = true;
            verticalLayout.childAlignment = TextAnchor.MiddleCenter;
            verticalLayout.spacing = 28f;
            verticalLayout.padding = new RectOffset(0, 0, 0, 0);
            verticalLayout.childForceExpandWidth = false;
            verticalLayout.childForceExpandHeight = false;
        }

        private void ForceCombatantLayout()
        {
            Canvas.ForceUpdateCanvases();
            ForceRebuildLayout(enemyRoot);
            ForceRebuildLayout(playerRoot);
            Canvas.ForceUpdateCanvases();
        }

        private void ForceRebuildLayout(Transform root)
        {
            RectTransform rectTransform = root as RectTransform;
            if (rectTransform != null)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
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
                CreatureSkillData enemySkill = enemyCreatureData != null ? enemyCreatureData.GetActiveSkill(enemyForm) : null;
                CreatureSkillData enemyOrbitSkill = enemyCreatureData != null ? enemyCreatureData.GetActiveOrbitSkill(enemyForm) : null;
                float enemyDisplayScale = enemyForm != null ? enemyForm.displayScale : 1f;
                Vector2 enemyVisualOffset = enemyForm != null ? enemyForm.battleVisualOffset : Vector2.zero;
                int maxHealth = Mathf.Max(1, enemy.maxHealth);
                int attackPower = Mathf.Max(1, Mathf.RoundToInt(enemy.attack * (enemySkill != null ? Mathf.Max(0.1f, enemySkill.powerMultiplier) : 1f)));

                CombatantView combatant = CreateCombatant(
                    enemyRoot,
                    enemyName,
                    enemySprite,
                    GetElementColor(enemy.element),
                    CombatantSlotSize,
                    true,
                    enemyDisplayScale,
                    enemyVisualOffset,
                    enemyIdleFrames,
                    enemySkill != null ? enemySkill.attackFrames : null,
                    enemySkill != null ? enemySkill.projectileFrames : null,
                    enemySkill != null ? enemySkill.impactFrames : null,
                    enemyForm != null ? enemyForm.ultimateSkill : null,
                    enemyForm != null ? enemyForm.ultimateRageRequired : 100f,
                    enemyForm != null ? enemyForm.ultimateImpactEffectSize : 360f,
                    enemyOrbitSkill != null ? enemyOrbitSkill.orbitEffectFrames : null,
                    enemyOrbitSkill != null ? enemyOrbitSkill.orbitHitEffectFrames : null,
                    enemySkill != null ? enemySkill.range : CreatureSkillRange.Melee,
                    enemyCreatureData == null || enemyCreatureData.idleFramesFaceRight,
                    enemyCreatureData == null || enemyCreatureData.attackFramesFaceRight,
                    enemyCreatureData != null ? enemyCreatureData.speed : BaselineSpeed,
                    maxHealth,
                    attackPower,
                    enemyOrbitSkill != null ? enemyOrbitSkill.orbitEffectScaleMultiplier : 1f,
                    enemyOrbitSkill != null ? enemyOrbitSkill.orbitHitEffectSize : OrbitHitEffectSize,
                    enemyOrbitSkill != null ? enemyOrbitSkill.orbitOrbFrames : null,
                    enemyOrbitSkill != null ? enemyOrbitSkill.orbitOrbCount : 0,
                    enemyOrbitSkill != null ? enemyOrbitSkill.orbitOrbSize : DefaultOrbitOrbSize,
                    enemyOrbitSkill != null ? enemyOrbitSkill.orbitOrbRadius : DefaultOrbitOrbRadius,
                    enemyOrbitSkill != null ? enemyOrbitSkill.orbitOrbSpeedDegrees : DefaultOrbitOrbSpeedDegrees,
                    enemyOrbitSkill != null ? enemyOrbitSkill.orbitOrbHitRadius : DefaultOrbitOrbHitRadius,
                    enemyOrbitSkill != null ? enemyOrbitSkill.orbitOrbHitCooldown : DefaultOrbitOrbHitCooldown,
                    enemyOrbitSkill != null ? enemyOrbitSkill.orbitOrbDamageMultiplier : 0f);
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
            layout.preferredWidth = CombatantSlotSize;
            layout.preferredHeight = CombatantSlotSize;
            layout.flexibleWidth = 0f;
            layout.flexibleHeight = 0f;

            TMP_Text label = CreateText(slot.transform, "EmptyText", "Empty", 14f, TextAlignmentOptions.Center);
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
                    CreatureSkillData activeSkill = staticData != null ? staticData.GetActiveSkill(form) : null;
                    CreatureSkillData orbitSkill = staticData != null ? staticData.GetActiveOrbitSkill(form) : null;
                    string creatureName = staticData != null ? staticData.creatureName : selectedCreature.creatureId;
                    int maxHealth = Mathf.Max(1, staticData != null ? staticData.basePower * 3 + selectedCreature.level * 5 : 30);
                    int attackPower = Mathf.Max(1, Mathf.RoundToInt((staticData != null ? staticData.basePower : 10) * (activeSkill != null ? Mathf.Max(0.1f, activeSkill.powerMultiplier) : 1f)));

                    CombatantView combatant = CreateCombatant(
                        playerRoot,
                        creatureName,
                        form != null ? form.sprite : null,
                        new Color(1f, 0.82f, 0.38f, 0.48f),
                        CombatantSlotSize,
                        false,
                        form != null ? form.displayScale : 1f,
                        form != null ? form.battleVisualOffset : Vector2.zero,
                        form != null ? form.idleFrames : null,
                        activeSkill != null ? activeSkill.attackFrames : null,
                        activeSkill != null ? activeSkill.projectileFrames : null,
                        activeSkill != null ? activeSkill.impactFrames : null,
                        form != null ? form.ultimateSkill : null,
                        form != null ? form.ultimateRageRequired : 100f,
                        form != null ? form.ultimateImpactEffectSize : 360f,
                        orbitSkill != null ? orbitSkill.orbitEffectFrames : null,
                        orbitSkill != null ? orbitSkill.orbitHitEffectFrames : null,
                        activeSkill != null ? activeSkill.range : CreatureSkillRange.Melee,
                        staticData == null || staticData.idleFramesFaceRight,
                        staticData == null || staticData.attackFramesFaceRight,
                        staticData != null ? staticData.speed : BaselineSpeed,
                        maxHealth,
                        attackPower,
                        orbitSkill != null ? orbitSkill.orbitEffectScaleMultiplier : 1f,
                        orbitSkill != null ? orbitSkill.orbitHitEffectSize : OrbitHitEffectSize,
                        orbitSkill != null ? orbitSkill.orbitOrbFrames : null,
                        orbitSkill != null ? orbitSkill.orbitOrbCount : 0,
                        orbitSkill != null ? orbitSkill.orbitOrbSize : DefaultOrbitOrbSize,
                        orbitSkill != null ? orbitSkill.orbitOrbRadius : DefaultOrbitOrbRadius,
                        orbitSkill != null ? orbitSkill.orbitOrbSpeedDegrees : DefaultOrbitOrbSpeedDegrees,
                        orbitSkill != null ? orbitSkill.orbitOrbHitRadius : DefaultOrbitOrbHitRadius,
                        orbitSkill != null ? orbitSkill.orbitOrbHitCooldown : DefaultOrbitOrbHitCooldown,
                        orbitSkill != null ? orbitSkill.orbitOrbDamageMultiplier : 0f);
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
            layout.preferredWidth = CombatantSlotSize;
            layout.preferredHeight = CombatantSlotSize;
            layout.flexibleWidth = 0f;
            layout.flexibleHeight = 0f;

            TMP_Text label = CreateText(slot.transform, "EmptyText", "Empty", 14f, TextAlignmentOptions.Center);
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
            Vector2 visualOffset = default(Vector2),
            Sprite[] idleFrames = null,
            Sprite[] attackFrames = null,
            Sprite[] projectileFrames = null,
            Sprite[] impactFrames = null,
            CreatureSkillData ultimateSkill = null,
            float ultimateRageRequired = 100f,
            float ultimateImpactEffectSize = 360f,
            Sprite[] orbitEffectFrames = null,
            Sprite[] orbitHitEffectFrames = null,
            CreatureSkillRange skillRange = CreatureSkillRange.Melee,
            bool idleFramesFaceRight = true,
            bool attackFramesFaceRight = true,
            float speed = BaselineSpeed,
            int maxHealth = 1,
            int attackPower = 1,
            float orbitEffectScaleMultiplier = 1f,
            float orbitHitEffectSize = OrbitHitEffectSize,
            Sprite[] orbitOrbFrames = null,
            int orbitOrbCount = 0,
            float orbitOrbSize = DefaultOrbitOrbSize,
            float orbitOrbRadius = DefaultOrbitOrbRadius,
            float orbitOrbSpeedDegrees = DefaultOrbitOrbSpeedDegrees,
            float orbitOrbHitRadius = DefaultOrbitOrbHitRadius,
            float orbitOrbHitCooldown = DefaultOrbitOrbHitCooldown,
            float orbitOrbDamageMultiplier = 0f)
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

            GameObject visualRootObject = new GameObject("VisualRoot", typeof(RectTransform));
            visualRootObject.transform.SetParent(root.transform, false);
            RectTransform visualRoot = visualRootObject.GetComponent<RectTransform>();
            SetAnchored(visualRoot, new Vector2(0f, 0.24f), new Vector2(1f, 1f), visualOffset, Vector2.zero);

            Image spriteImage = CreateImage(visualRoot, "Sprite", fallbackColor);
            StretchFull(spriteImage.rectTransform);
            spriteImage.sprite = sprite;
            spriteImage.color = sprite != null ? Color.white : fallbackColor;
            spriteImage.preserveAspect = true;
            float spriteScale = Mathf.Max(0.1f, displayScale * CombatantSpriteScale);
            spriteImage.rectTransform.localScale = new Vector3(enemy ? -spriteScale : spriteScale, spriteScale, spriteScale);

            CreatureSpriteAnimationUI animation = spriteImage.gameObject.AddComponent<CreatureSpriteAnimationUI>();
            animation.Play(idleFrames, sprite);

            bool useRuntimeOrbitOrbs = orbitOrbFrames != null && orbitOrbFrames.Length > 0 && orbitOrbCount > 0;
            Image orbitEffectImage = null;
            CreatureSpriteAnimationUI orbitEffectAnimation = null;
            if (!useRuntimeOrbitOrbs && orbitEffectFrames != null && orbitEffectFrames.Length > 0)
            {
                orbitEffectImage = CreateImage(visualRoot, "OrbitEffect", Color.white);
                StretchFull(orbitEffectImage.rectTransform);
                orbitEffectImage.rectTransform.anchoredPosition = Vector2.zero;
                float orbitScale = spriteScale * OrbitEffectScale * Mathf.Max(0.1f, orbitEffectScaleMultiplier);
                orbitEffectImage.rectTransform.localScale = new Vector3(orbitScale, orbitScale, 1f);
                orbitEffectImage.preserveAspect = true;
                orbitEffectImage.raycastTarget = false;
                orbitEffectImage.transform.SetAsFirstSibling();

                orbitEffectAnimation = orbitEffectImage.gameObject.AddComponent<CreatureSpriteAnimationUI>();
                orbitEffectAnimation.Play(orbitEffectFrames, null);
            }

            TMP_Text label = CreateText(root.transform, "Name", labelText, 13f, TextAlignmentOptions.Center);
            label.fontSize = 12f;
            SetAnchored(label.rectTransform, new Vector2(0f, 0f), new Vector2(1f, 0.28f), Vector2.zero, Vector2.zero);

            CombatantView combatant = new CombatantView
            {
                root = root.GetComponent<RectTransform>(),
                visualRoot = visualRoot,
                spriteImage = spriteImage,
                animation = animation,
                orbitEffectImage = orbitEffectImage,
                orbitEffectAnimation = orbitEffectAnimation,
                nameLabel = label,
                displayName = labelText,
                idleFrames = idleFrames,
                attackFrames = attackFrames,
                projectileFrames = projectileFrames,
                impactFrames = impactFrames,
                ultimateSkill = HasSkillVisuals(ultimateSkill) ? ultimateSkill : null,
                ultimateRageRequired = Mathf.Max(1f, ultimateRageRequired),
                ultimateImpactEffectSize = Mathf.Max(1f, ultimateImpactEffectSize),
                orbitEffectFrames = orbitEffectFrames,
                orbitHitEffectFrames = orbitHitEffectFrames,
                fallbackSprite = sprite,
                isEnemy = enemy,
                baseSpriteScale = spriteScale,
                idleFramesFaceRight = idleFramesFaceRight,
                attackFramesFaceRight = attackFramesFaceRight,
                skillRange = skillRange,
                speed = Mathf.Max(1f, speed),
                maxHealth = Mathf.Max(1, maxHealth),
                currentHealth = Mathf.Max(1, maxHealth),
                attackPower = Mathf.Max(1, attackPower),
                orbitHitEffectSize = Mathf.Max(1f, orbitHitEffectSize),
                orbitOrbFrames = orbitOrbFrames,
                orbitOrbHitRadius = Mathf.Max(1f, orbitOrbHitRadius * Mathf.Max(0.1f, displayScale)),
                orbitOrbHitCooldown = Mathf.Max(0.05f, orbitOrbHitCooldown),
                orbitOrbDamageMultiplier = Mathf.Max(0f, orbitOrbDamageMultiplier),
                attackTimer = 0f
            };
            CreateOrbitOrbs(combatant, Mathf.Max(0, orbitOrbCount), orbitOrbSize, orbitOrbRadius, orbitOrbSpeedDegrees);
            ConfigureDebugUltimateClick(root, combatant);
            UpdateCombatantLabel(combatant);
            SetCombatantDefaultFacing(combatant);
            return combatant;
        }

        private void ConfigureDebugUltimateClick(GameObject root, CombatantView combatant)
        {
            if (!debugClickUltimate || root == null || combatant == null || !combatant.HasUltimate)
            {
                return;
            }

            Button button = root.GetComponent<Button>();
            if (button == null)
            {
                button = root.AddComponent<Button>();
            }

            button.transition = Selectable.Transition.None;
            button.onClick.AddListener(() => TryTriggerDebugUltimate(combatant));
        }

        private void TryTriggerDebugUltimate(CombatantView combatant)
        {
            if (!debugClickUltimate || combatant == null || !combatant.IsAlive || combatant.isAttacking || !combatant.HasUltimate)
            {
                return;
            }

            List<CombatantView> defenders = combatant.isEnemy ? playerCombatants : enemyCombatants;
            if (GetFirstLivingCombatant(defenders) == null)
            {
                return;
            }

            combatant.attackTimer = 0f;
            combatant.ultimateRage = combatant.ultimateRageRequired;
            UpdateCombatantLabel(combatant);
            StartCoroutine(PerformUltimate(combatant, defenders));
        }

        private void CreateOrbitOrbs(CombatantView combatant, int count, float size, float radius, float speedDegrees)
        {
            if (combatant == null || combatant.visualRoot == null || combatant.orbitOrbFrames == null || combatant.orbitOrbFrames.Length == 0 || count <= 0)
            {
                return;
            }

            combatant.orbitOrbs = new List<OrbitOrbView>();
            float formScale = Mathf.Max(0.1f, combatant.baseSpriteScale / CombatantSpriteScale);
            float clampedSize = Mathf.Max(1f, size * formScale);
            float clampedRadius = Mathf.Max(1f, radius * formScale);
            float clampedSpeed = Mathf.Max(1f, speedDegrees);

            for (int i = 0; i < count; i++)
            {
                Image orbImage = CreateImage(combatant.visualRoot, $"OrbitOrb{i + 1}", Color.white);
                RectTransform orbRect = orbImage.rectTransform;
                orbRect.anchorMin = new Vector2(0.5f, 0.5f);
                orbRect.anchorMax = new Vector2(0.5f, 0.5f);
                orbRect.pivot = new Vector2(0.5f, 0.5f);
                orbRect.sizeDelta = new Vector2(clampedSize, clampedSize);
                orbRect.localScale = Vector3.one;
                orbImage.preserveAspect = true;
                orbImage.raycastTarget = false;
                orbImage.transform.SetAsLastSibling();

                CreatureSpriteAnimationUI orbAnimation = orbImage.gameObject.AddComponent<CreatureSpriteAnimationUI>();
                orbAnimation.Play(combatant.orbitOrbFrames, null);

                combatant.orbitOrbs.Add(new OrbitOrbView
                {
                    image = orbImage,
                    rectTransform = orbRect,
                    animation = orbAnimation,
                    angleDegrees = i * 360f / count,
                    radius = clampedRadius,
                    speedDegrees = clampedSpeed,
                    hitCooldownRemaining = 0f
                });
            }
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

        private Sprite[] GetFormOverrideFrames(Sprite[] formFrames, Sprite[] fallbackFrames)
        {
            return formFrames != null && formFrames.Length > 0 ? formFrames : fallbackFrames;
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
                UpdateOrbitOrbs(playerCombatants, enemyCombatants);
                UpdateOrbitOrbs(enemyCombatants, playerCombatants);
                TickCombatants(playerCombatants, enemyCombatants);
                TickCombatants(enemyCombatants, playerCombatants);
                yield return null;
            }
        }

        private void UpdateOrbitOrbs(List<CombatantView> owners, List<CombatantView> defenders)
        {
            for (int i = 0; i < owners.Count; i++)
            {
                CombatantView owner = owners[i];
                if (owner == null || !owner.IsAlive || owner.orbitOrbs == null)
                {
                    continue;
                }

                for (int orbIndex = 0; orbIndex < owner.orbitOrbs.Count; orbIndex++)
                {
                    OrbitOrbView orb = owner.orbitOrbs[orbIndex];
                    if (orb == null || orb.rectTransform == null)
                    {
                        continue;
                    }

                    orb.angleDegrees = Mathf.Repeat(orb.angleDegrees + orb.speedDegrees * Time.deltaTime, 360f);
                    float radians = orb.angleDegrees * Mathf.Deg2Rad;
                    orb.rectTransform.anchoredPosition = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)) * orb.radius;
                    orb.hitCooldownRemaining = Mathf.Max(0f, orb.hitCooldownRemaining - Time.deltaTime);

                    if (orb.hitCooldownRemaining <= 0f && TryHitOrbitOrbTarget(owner, orb, defenders))
                    {
                        orb.hitCooldownRemaining = owner.orbitOrbHitCooldown;
                    }
                }
            }
        }

        private bool TryHitOrbitOrbTarget(CombatantView owner, OrbitOrbView orb, List<CombatantView> defenders)
        {
            if (owner == null || orb == null || orb.rectTransform == null || defenders == null)
            {
                return false;
            }

            Vector3 orbWorldPosition = orb.rectTransform.TransformPoint(orb.rectTransform.rect.center);
            float hitRadius = Mathf.Max(1f, owner.orbitOrbHitRadius);

            for (int i = 0; i < defenders.Count; i++)
            {
                CombatantView defender = defenders[i];
                if (defender == null || !defender.IsAlive || defender.spriteImage == null)
                {
                    continue;
                }

                Vector3 targetWorldPosition = GetSpriteWorldCenter(defender.spriteImage.rectTransform);
                if (Vector3.Distance(orbWorldPosition, targetWorldPosition) > hitRadius)
                {
                    continue;
                }

                int orbitDamage = Mathf.RoundToInt(owner.attackPower * owner.orbitOrbDamageMultiplier);
                if (orbitDamage > 0)
                {
                    ApplyDamage(defender, orbitDamage);
                    ChargeUltimate(owner, UltimateRageGainOnAttack * 0.4f);
                    ChargeUltimate(defender, UltimateRageGainWhenHit * 0.4f);
                }

                PlayOrbitHitEffect(owner, defender);
                return true;
            }

            return false;
        }

        private void TickCombatants(List<CombatantView> attackers, List<CombatantView> defenders)
        {
            for (int i = 0; i < attackers.Count; i++)
            {
                CombatantView attacker = attackers[i];
                if (attacker == null || !attacker.IsAlive || !attacker.HasCreatureVisual || attacker.isAttacking)
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
                if (attacker.CanUseUltimate)
                {
                    StartCoroutine(PerformUltimate(attacker, defenders));
                }
                else
                {
                    StartCoroutine(PerformAttack(attacker, target));
                }
            }
        }

        private IEnumerator PerformUltimate(CombatantView attacker, List<CombatantView> defenders)
        {
            CombatantView target = GetFirstLivingCombatant(defenders);
            if (attacker == null || target == null || attacker.ultimateSkill == null)
            {
                yield break;
            }

            attacker.isAttacking = true;
            attacker.ultimateRage = 0f;
            UpdateCombatantLabel(attacker);
            SetCombatantAttackFacing(attacker);

            yield return PlayCombatantFrames(attacker, attacker.ultimateSkill.attackFrames, UltimateFrameDuration, 0.18f);
            yield return PlayEffectFramesAtWorldPosition(
                GetTeamEffectWorldCenter(target),
                attacker.ultimateSkill.impactFrames,
                attacker.ultimateImpactEffectSize,
                UltimateFrameDuration);

            int damage = Mathf.Max(1, Mathf.RoundToInt(attacker.attackPower * Mathf.Max(0.1f, attacker.ultimateSkill.powerMultiplier)));
            for (int i = 0; defenders != null && i < defenders.Count; i++)
            {
                CombatantView defender = defenders[i];
                if (defender != null && defender.IsAlive)
                {
                    ApplyDamage(defender, damage);
                    ChargeUltimate(defender, UltimateRageGainWhenHit);
                }
            }

            attacker.animation.Play(attacker.idleFrames, attacker.fallbackSprite);
            SetCombatantDefaultFacing(attacker);
            attacker.isAttacking = false;
        }

        private IEnumerator PerformAttack(CombatantView attacker, CombatantView target)
        {
            attacker.isAttacking = true;

            if (attacker.skillRange == CreatureSkillRange.Melee)
            {
                SetCombatantAttackFacing(attacker);
                RectTransform attackerMoveRoot = attacker.visualRoot != null ? attacker.visualRoot : attacker.spriteImage.rectTransform;
                Vector3 currentPosition = attackerMoveRoot.localPosition;
                if (attacker.meleeContactTarget != target || !attacker.hasMeleeContactPosition)
                {
                    attacker.meleeContactPosition = GetMeleeContactPosition(attacker, target, currentPosition);
                    attacker.meleeContactTarget = target;
                    attacker.hasMeleeContactPosition = true;
                }

                if ((attacker.meleeContactPosition - currentPosition).sqrMagnitude > 1f)
                {
                    yield return MoveSprite(attackerMoveRoot, currentPosition, attacker.meleeContactPosition, MeleeLungeDuration);
                }

                yield return PlayAttackFrames(attacker);
                ApplyDamage(target, attacker.attackPower);
                ChargeUltimate(attacker, UltimateRageGainOnAttack);
                ChargeUltimate(target, UltimateRageGainWhenHit);
                yield return PlayImpactFrames(attacker, target);
                if (!attacker.HasRuntimeOrbitOrbs)
                {
                    PlayOrbitHitEffect(attacker, target);
                }
            }
            else
            {
                SetCombatantAttackFacing(attacker);
                yield return PlayAttackFrames(attacker);
                yield return PlayProjectileFrames(attacker, target);
                ApplyDamage(target, attacker.attackPower);
                ChargeUltimate(attacker, UltimateRageGainOnAttack);
                ChargeUltimate(target, UltimateRageGainWhenHit);
                yield return PlayImpactFrames(attacker, target);
                if (!attacker.HasRuntimeOrbitOrbs)
                {
                    PlayOrbitHitEffect(attacker, target);
                }
            }

            attacker.animation.Play(attacker.idleFrames, attacker.fallbackSprite);
            SetCombatantDefaultFacing(attacker);
            attacker.isAttacking = false;
        }

        private void SetCombatantDefaultFacing(CombatantView combatant)
        {
            if (combatant == null || combatant.spriteImage == null)
            {
                return;
            }

            SetCombatantFacing(combatant, combatant.idleFramesFaceRight);
        }

        private void SetCombatantAttackFacing(CombatantView combatant)
        {
            SetCombatantFacing(combatant, combatant.attackFramesFaceRight);
        }

        private void SetCombatantFacing(CombatantView combatant, bool sourceFacesRight)
        {
            if (combatant == null || combatant.spriteImage == null)
            {
                return;
            }

            float scale = Mathf.Max(0.1f, combatant.baseSpriteScale);
            bool shouldFaceRight = !combatant.isEnemy;
            float directionX = sourceFacesRight == shouldFaceRight ? 1f : -1f;
            combatant.spriteImage.rectTransform.localScale = new Vector3(scale * directionX, scale, scale);
        }

        private Vector3 GetSpriteWorldCenter(RectTransform spriteTransform)
        {
            if (spriteTransform == null)
            {
                return Vector3.zero;
            }

            return spriteTransform.TransformPoint(spriteTransform.rect.center);
        }

        private Vector3 GetMeleeContactPosition(CombatantView attacker, CombatantView target, Vector3 attackerStartLocalPosition)
        {
            if (attacker == null || target == null || attacker.spriteImage == null || target.spriteImage == null)
            {
                return attackerStartLocalPosition;
            }

            RectTransform attackerSprite = attacker.spriteImage.rectTransform;
            RectTransform attackerMoveRoot = attacker.visualRoot != null ? attacker.visualRoot : attackerSprite;
            RectTransform attackerParent = attackerMoveRoot.parent as RectTransform;
            RectTransform targetSprite = target.spriteImage.rectTransform;

            if (attackerParent == null)
            {
                return attackerStartLocalPosition;
            }

            Vector3 attackerWorldPosition = attackerParent.TransformPoint(attackerStartLocalPosition);
            Vector3 targetWorldPosition = GetCombatantWorldCenter(target);
            float horizontalDelta = targetWorldPosition.x - attackerWorldPosition.x;
            if (Mathf.Abs(horizontalDelta) <= 0.01f)
            {
                Vector3 targetLocalPosition = attackerParent.InverseTransformPoint(targetWorldPosition);
                return new Vector3(attackerStartLocalPosition.x, targetLocalPosition.y, attackerStartLocalPosition.z);
            }

            float directionX = Mathf.Sign(horizontalDelta);
            Vector3 direction = new Vector3(directionX, 0f, 0f);
            float stopDistance = CalculateMeleeStopDistance(attackerSprite, targetSprite, attackerParent, direction);
            Vector3 contactWorldPosition = new Vector3(
                targetWorldPosition.x - directionX * stopDistance,
                targetWorldPosition.y,
                attackerWorldPosition.z);

            Vector3 contactLocalPosition = attackerParent.InverseTransformPoint(contactWorldPosition);
            return new Vector3(contactLocalPosition.x, contactLocalPosition.y, attackerStartLocalPosition.z);
        }

        private Vector3 GetCombatantWorldCenter(CombatantView combatant)
        {
            if (combatant == null)
            {
                return Vector3.zero;
            }

            RectTransform root = combatant.root != null ? combatant.root : combatant.spriteImage != null ? combatant.spriteImage.rectTransform : null;
            if (root == null)
            {
                return Vector3.zero;
            }

            return root.TransformPoint(root.rect.center);
        }

        private float CalculateMeleeStopDistance(RectTransform attackerSprite, RectTransform targetSprite, RectTransform contactSpace, Vector3 direction)
        {
            if (attackerSprite == null || targetSprite == null || contactSpace == null)
            {
                return MeleeContactOffset;
            }

            float targetExtent = GetProjectedHalfExtent(targetSprite, contactSpace, direction);
            return Mathf.Max(MeleeContactOffset, targetExtent + MeleeContactOffset);
        }

        private float GetProjectedHalfExtent(RectTransform rectTransform, RectTransform projectionSpace, Vector3 direction)
        {
            if (rectTransform == null || projectionSpace == null || direction.sqrMagnitude <= 0.01f)
            {
                return 0f;
            }

            Vector3 center = projectionSpace.InverseTransformPoint(rectTransform.TransformPoint(rectTransform.rect.center));
            Vector3[] corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);

            float extent = 0f;
            for (int i = 0; i < corners.Length; i++)
            {
                Vector3 localCorner = projectionSpace.InverseTransformPoint(corners[i]);
                extent = Mathf.Max(extent, Mathf.Abs(Vector3.Dot(localCorner - center, direction)));
            }

            return extent;
        }

        private IEnumerator PlayAttackFrames(CombatantView attacker)
        {
            yield return PlayCombatantFrames(attacker, attacker != null ? attacker.attackFrames : null, 0.045f, 0.16f);
        }

        private IEnumerator PlayCombatantFrames(CombatantView attacker, Sprite[] frames, float frameDuration, float fallbackDelay)
        {
            if (attacker == null || frames == null || frames.Length == 0)
            {
                yield return new WaitForSeconds(fallbackDelay);
                yield break;
            }

            attacker.animation.Stop(attacker.fallbackSprite);

            for (int i = 0; i < frames.Length; i++)
            {
                if (frames[i] != null)
                {
                    attacker.spriteImage.sprite = frames[i];
                    attacker.spriteImage.enabled = true;
                }

                yield return new WaitForSeconds(frameDuration);
            }
        }

        private IEnumerator PlayImpactFrames(CombatantView attacker, CombatantView target)
        {
            yield return PlayEffectFrames(target, attacker != null ? attacker.impactFrames : null, ImpactEffectSize, ImpactFrameDuration);
        }

        private IEnumerator PlayEffectFrames(CombatantView target, Sprite[] frames, float effectSize, float frameDuration)
        {
            if (frames == null || frames.Length == 0 || target == null || target.spriteImage == null)
            {
                yield break;
            }

            yield return PlayEffectFramesAtWorldPosition(GetSpriteWorldCenter(target.spriteImage.rectTransform), frames, effectSize, frameDuration);
        }

        private IEnumerator PlayEffectFramesAtWorldPosition(Vector3 worldPosition, Sprite[] frames, float effectSize, float frameDuration)
        {
            if (frames == null || frames.Length == 0)
            {
                yield break;
            }

            RectTransform battleRect = transform as RectTransform;
            if (battleRect == null)
            {
                yield break;
            }

            GameObject effectRoot = new GameObject("ImpactEffect", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            effectRoot.transform.SetParent(transform, false);
            effectRoot.transform.SetAsLastSibling();

            RectTransform effectRect = effectRoot.GetComponent<RectTransform>();
            effectRect.anchorMin = new Vector2(0.5f, 0.5f);
            effectRect.anchorMax = new Vector2(0.5f, 0.5f);
            effectRect.pivot = new Vector2(0.5f, 0.5f);
            effectRect.localPosition = battleRect.InverseTransformPoint(worldPosition);
            float clampedEffectSize = Mathf.Max(1f, effectSize);
            effectRect.sizeDelta = new Vector2(clampedEffectSize, clampedEffectSize);
            effectRect.localScale = Vector3.one;

            Image effectImage = effectRoot.GetComponent<Image>();
            effectImage.color = Color.white;
            effectImage.preserveAspect = true;
            effectImage.raycastTarget = false;

            for (int i = 0; i < frames.Length; i++)
            {
                if (effectImage == null)
                {
                    yield break;
                }

                effectImage.sprite = frames[i];
                effectImage.enabled = frames[i] != null;
                yield return new WaitForSeconds(frameDuration);
            }

            if (effectRoot != null)
            {
                Destroy(effectRoot);
            }
        }

        private Vector3 GetTeamEffectWorldCenter(CombatantView target)
        {
            if (target == null)
            {
                return Vector3.zero;
            }

            RectTransform teamRoot = target.isEnemy ? enemyRoot as RectTransform : playerRoot as RectTransform;
            if (teamRoot != null)
            {
                return teamRoot.TransformPoint(teamRoot.rect.center);
            }

            return GetCombatantWorldCenter(target);
        }

        private void PlayOrbitHitEffect(CombatantView attacker, CombatantView target)
        {
            if (attacker == null || attacker.orbitHitEffectFrames == null || attacker.orbitHitEffectFrames.Length == 0 || target == null || target.spriteImage == null)
            {
                return;
            }

            StartCoroutine(PlayOrbitHitEffectFrames(attacker, target));
        }

        private IEnumerator PlayOrbitHitEffectFrames(CombatantView attacker, CombatantView target)
        {
            RectTransform battleRect = transform as RectTransform;
            if (battleRect == null)
            {
                yield break;
            }

            GameObject effectRoot = new GameObject("OrbitHitEffect", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            effectRoot.transform.SetParent(transform, false);
            effectRoot.transform.SetAsLastSibling();

            RectTransform effectRect = effectRoot.GetComponent<RectTransform>();
            effectRect.anchorMin = new Vector2(0.5f, 0.5f);
            effectRect.anchorMax = new Vector2(0.5f, 0.5f);
            effectRect.pivot = new Vector2(0.5f, 0.5f);
            Vector3 targetCenter = GetCombatantWorldCenter(target);
            effectRect.localPosition = battleRect.InverseTransformPoint(targetCenter);
            float hitEffectSize = Mathf.Max(1f, attacker.orbitHitEffectSize);
            effectRect.sizeDelta = new Vector2(hitEffectSize, hitEffectSize);
            effectRect.localScale = Vector3.one;

            Image effectImage = effectRoot.GetComponent<Image>();
            effectImage.color = Color.white;
            effectImage.preserveAspect = true;
            effectImage.raycastTarget = false;

            for (int i = 0; i < attacker.orbitHitEffectFrames.Length; i++)
            {
                if (effectImage == null)
                {
                    yield break;
                }

                effectImage.sprite = attacker.orbitHitEffectFrames[i];
                effectImage.enabled = attacker.orbitHitEffectFrames[i] != null;
                yield return new WaitForSeconds(OrbitHitEffectFrameDuration);
            }

            if (effectRoot != null)
            {
                Destroy(effectRoot);
            }
        }

        private IEnumerator PlayProjectileFrames(CombatantView attacker, CombatantView target)
        {
            if (attacker == null || attacker.projectileFrames == null || attacker.projectileFrames.Length == 0 || target == null || attacker.spriteImage == null || target.spriteImage == null)
            {
                yield break;
            }

            RectTransform battleRect = transform as RectTransform;
            if (battleRect == null)
            {
                yield break;
            }

            GameObject projectileRoot = new GameObject("ProjectileEffect", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            projectileRoot.transform.SetParent(transform, false);
            projectileRoot.transform.SetAsLastSibling();

            RectTransform projectileRect = projectileRoot.GetComponent<RectTransform>();
            projectileRect.anchorMin = new Vector2(0.5f, 0.5f);
            projectileRect.anchorMax = new Vector2(0.5f, 0.5f);
            projectileRect.pivot = new Vector2(0.5f, 0.5f);
            projectileRect.sizeDelta = new Vector2(ProjectileSize, ProjectileSize);
            projectileRect.localScale = Vector3.one;

            Image projectileImage = projectileRoot.GetComponent<Image>();
            projectileImage.color = Color.white;
            projectileImage.preserveAspect = true;
            projectileImage.raycastTarget = false;

            Vector3 startLocal = battleRect.InverseTransformPoint(GetSpriteWorldCenter(attacker.spriteImage.rectTransform));
            Vector3 endLocal = battleRect.InverseTransformPoint(GetSpriteWorldCenter(target.spriteImage.rectTransform));
            Vector3 direction = endLocal - startLocal;
            if (direction.sqrMagnitude > 0.01f)
            {
                startLocal += direction.normalized * ProjectileStartOffset;
            }

            float distance = Vector3.Distance(startLocal, endLocal);
            float duration = Mathf.Clamp(distance / ProjectileSpeed, 0.22f, 0.55f);
            float timer = 0f;

            while (timer < duration)
            {
                if (projectileImage == null || projectileRect == null)
                {
                    yield break;
                }

                if (target != null && target.spriteImage != null)
                {
                    endLocal = battleRect.InverseTransformPoint(GetSpriteWorldCenter(target.spriteImage.rectTransform));
                }

                Vector3 currentDirection = endLocal - startLocal;
                if (currentDirection.sqrMagnitude > 0.01f)
                {
                    float angle = Mathf.Atan2(currentDirection.y, currentDirection.x) * Mathf.Rad2Deg;
                    projectileRect.localRotation = Quaternion.Euler(0f, 0f, angle);
                }

                int frameIndex = Mathf.FloorToInt(timer / ProjectileFrameDuration) % attacker.projectileFrames.Length;
                projectileImage.sprite = attacker.projectileFrames[frameIndex];
                projectileImage.enabled = attacker.projectileFrames[frameIndex] != null;

                float t = Mathf.Clamp01(timer / Mathf.Max(0.01f, duration));
                projectileRect.localPosition = Vector3.Lerp(startLocal, endLocal, t);
                timer += Time.deltaTime;
                yield return null;
            }

            if (projectileRoot != null)
            {
                Destroy(projectileRoot);
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
                if (target.orbitEffectImage != null)
                {
                    target.orbitEffectImage.color = new Color(1f, 1f, 1f, 0.28f);
                }
                if (target.orbitOrbs != null)
                {
                    for (int i = 0; i < target.orbitOrbs.Count; i++)
                    {
                        if (target.orbitOrbs[i] != null && target.orbitOrbs[i].image != null)
                        {
                            target.orbitOrbs[i].image.color = new Color(1f, 1f, 1f, 0.28f);
                        }
                    }
                }
                target.animation.Stop(target.fallbackSprite);
            }
        }

        private CombatantView GetFirstLivingCombatant(List<CombatantView> combatants)
        {
            for (int i = 0; i < combatants.Count; i++)
            {
                if (combatants[i] != null && combatants[i].IsAlive && combatants[i].HasCreatureVisual)
                {
                    return combatants[i];
                }
            }

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

        private static bool HasSkillVisuals(CreatureSkillData skill)
        {
            if (skill == null)
            {
                return false;
            }

            bool hasAttack = skill.attackFrames != null && skill.attackFrames.Length > 0;
            bool hasImpact = skill.impactFrames != null && skill.impactFrames.Length > 0;
            return hasAttack || hasImpact;
        }

        private void ChargeUltimate(CombatantView combatant, float amount)
        {
            if (combatant == null || !combatant.HasUltimate || amount <= 0f)
            {
                return;
            }

            combatant.ultimateRage = Mathf.Min(combatant.ultimateRageRequired, combatant.ultimateRage + amount);
            UpdateCombatantLabel(combatant);
        }

        private void UpdateCombatantLabel(CombatantView combatant)
        {
            if (combatant == null || combatant.nameLabel == null)
            {
                return;
            }

            string rageLine = combatant.HasUltimate
                ? $"\nRage {Mathf.RoundToInt(combatant.ultimateRage / combatant.ultimateRageRequired * 100f)}%"
                : string.Empty;
            combatant.nameLabel.text = $"{combatant.displayName}\nHP {combatant.currentHealth}/{combatant.maxHealth}{rageLine}";
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
            public RectTransform visualRoot;
            public Image spriteImage;
            public CreatureSpriteAnimationUI animation;
            public Image orbitEffectImage;
            public CreatureSpriteAnimationUI orbitEffectAnimation;
            public TMP_Text nameLabel;
            public string displayName;
            public Sprite[] idleFrames;
            public Sprite[] attackFrames;
            public Sprite[] projectileFrames;
            public Sprite[] impactFrames;
            public CreatureSkillData ultimateSkill;
            public float ultimateRage;
            public float ultimateRageRequired;
            public float ultimateImpactEffectSize;
            public Sprite[] orbitEffectFrames;
            public Sprite[] orbitHitEffectFrames;
            public Sprite[] orbitOrbFrames;
            public List<OrbitOrbView> orbitOrbs;
            public Sprite fallbackSprite;
            public bool isEnemy;
            public float baseSpriteScale;
            public bool idleFramesFaceRight;
            public bool attackFramesFaceRight;
            public CreatureSkillRange skillRange;
            public float speed;
            public int maxHealth;
            public int currentHealth;
            public int attackPower;
            public float orbitHitEffectSize;
            public float orbitOrbHitRadius;
            public float orbitOrbHitCooldown;
            public float orbitOrbDamageMultiplier;
            public float attackTimer;
            public bool isAttacking;
            public bool hasMeleeContactPosition;
            public Vector3 meleeContactPosition;
            public CombatantView meleeContactTarget;

            public bool IsAlive => currentHealth > 0;
            public bool HasCreatureVisual => fallbackSprite != null || (idleFrames != null && idleFrames.Length > 0);
            public bool HasRuntimeOrbitOrbs => orbitOrbs != null && orbitOrbs.Count > 0;
            public bool HasUltimate => ultimateSkill != null;
            public bool CanUseUltimate => HasUltimate && ultimateRage >= ultimateRageRequired;
        }

        private class OrbitOrbView
        {
            public Image image;
            public RectTransform rectTransform;
            public CreatureSpriteAnimationUI animation;
            public float angleDegrees;
            public float radius;
            public float speedDegrees;
            public float hitCooldownRemaining;
        }
    }
}
