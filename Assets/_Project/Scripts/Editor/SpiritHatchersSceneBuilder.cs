using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using SpiritHatchers.Ads;
using SpiritHatchers.Core;
using SpiritHatchers.Creature;
using SpiritHatchers.DailyReward;
using SpiritHatchers.Data;
using SpiritHatchers.Expedition;
using SpiritHatchers.Hatch;
using SpiritHatchers.Resources;
using SpiritHatchers.Save;
using SpiritHatchers.UI;

namespace SpiritHatchers.EditorTools
{
    public static class SpiritHatchersSceneBuilder
    {
        private const string MenuPath = "Spirit Hatchers/Setup/Build MVP Scene UI";
        private const string FixResourceBarMenuPath = "Spirit Hatchers/Setup/Fix Resource Bar Icons";
        private const string PrefabUiPath = "Assets/_Project/Prefabs/UI";
        private const string PrefabCardsPath = "Assets/_Project/Prefabs/Cards";

        [MenuItem(MenuPath)]
        public static void BuildMvpSceneUi()
        {
            EnsureFolders();

            GameObject managers = GetOrCreateRoot("Managers");
            GameManager gameManager = GetOrCreateManager<GameManager>(managers.transform, "GameManager");
            SaveManager saveManager = GetOrCreateManager<SaveManager>(managers.transform, "SaveManager");
            ResourceManager resourceManager = GetOrCreateManager<ResourceManager>(managers.transform, "ResourceManager");
            HatchManager hatchManager = GetOrCreateManager<HatchManager>(managers.transform, "HatchManager");
            CreatureProgressionManager progressionManager = GetOrCreateManager<CreatureProgressionManager>(managers.transform, "CreatureProgressionManager");
            ExpeditionManager expeditionManager = GetOrCreateManager<ExpeditionManager>(managers.transform, "ExpeditionManager");
            DailyRewardManager dailyRewardManager = GetOrCreateManager<DailyRewardManager>(managers.transform, "DailyRewardManager");
            MockRewardedAdService adService = GetOrCreateManager<MockRewardedAdService>(managers.transform, "MockRewardedAdService");

            Canvas canvas = GetOrCreateCanvas();
            EnsureEventSystem();

            UIManager uiManager = GetOrCreateComponentOnChild<UIManager>(canvas.transform, "UIManager");
            GameObject homeScreen = CreateScreen(canvas.transform, "HomeScreen", true);
            GameObject hatchScreen = CreateScreen(canvas.transform, "HatchScreen", false);
            GameObject collectionScreen = CreateScreen(canvas.transform, "CollectionScreen", false);
            GameObject creatureDetailScreen = CreateScreen(canvas.transform, "CreatureDetailScreen", false);
            GameObject expeditionScreen = CreateScreen(canvas.transform, "ExpeditionScreen", false);
            GameObject popups = GetOrCreateChild(canvas.transform, "Popups");
            StretchFull(popups.GetComponent<RectTransform>() ?? popups.AddComponent<RectTransform>());

            CreatureDatabase creatureDatabase = FindFirstAsset<CreatureDatabase>();
            CreatureStaticData fallbackCreature = FindFirstAsset<CreatureStaticData>();
            ExpeditionData[] expeditionAssets = FindAllAssets<ExpeditionData>();

            BuildHomeScreen(homeScreen.transform, gameManager, uiManager, resourceManager, creatureDatabase, fallbackCreature);
            BuildHatchScreen(hatchScreen.transform, hatchManager, resourceManager, adService, uiManager);
            BuildCollectionScreen(collectionScreen.transform, gameManager, uiManager, hatchManager, creatureDatabase);
            BuildCreatureDetailScreen(creatureDetailScreen.transform, uiManager, progressionManager);
            SetObject(
                collectionScreen.GetComponent<CollectionScreenUI>(),
                "creatureDetailScreenUI",
                creatureDetailScreen.GetComponent<CreatureDetailScreenUI>());
            BuildPopups(popups.transform, hatchManager, expeditionManager, creatureDatabase, dailyRewardManager);
            BuildExpeditionScreen(expeditionScreen.transform, expeditionManager, creatureDatabase, uiManager);

            AssignManagers(gameManager, saveManager, uiManager, resourceManager, hatchManager, progressionManager, expeditionManager, dailyRewardManager, creatureDatabase, expeditionAssets);
            AssignUiManager(uiManager, homeScreen, hatchScreen, collectionScreen, creatureDetailScreen, expeditionScreen, popups.transform);

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Selection.activeGameObject = canvas.gameObject;

            Debug.Log("Spirit Hatchers MVP scene UI created. Check Inspector fields for any missing ScriptableObject assets.");
        }

        [MenuItem(FixResourceBarMenuPath)]
        public static void FixResourceBarIcons()
        {
            GameObject topBar = GameObject.Find("TopResourceBar");
            if (topBar == null)
            {
                Debug.LogWarning("Could not find TopResourceBar in the open scene.");
                return;
            }

            HorizontalLayoutGroup topLayout = topBar.GetOrAdd<HorizontalLayoutGroup>();
            topLayout.childAlignment = TextAnchor.MiddleCenter;
            topLayout.padding = new RectOffset(16, 16, 12, 12);
            topLayout.spacing = 12f;
            topLayout.childForceExpandWidth = true;
            topLayout.childForceExpandHeight = true;

            FixResourceGroup(topBar.transform, "CoinResource", "CoinIcon", FindSpriteByKeywords("coin", "gold"));
            FixResourceGroup(topBar.transform, "FoodResource", "FoodIcon", FindSpriteByKeywords("food", "drumstick", "berry"));
            FixResourceGroup(topBar.transform, "CrystalResource", "CrystalIcon", FindSpriteByKeywords("crystal", "gem"));
            FixResourceGroup(topBar.transform, "EggTicketResource", "TicketIcon", FindSpriteByKeywords("ticket", "egg"));

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Selection.activeGameObject = topBar;
            Debug.Log("Resource bar icons fixed. If an icon is still missing, assign its sprite manually on the matching Icon object.");
        }

        private static void BuildHomeScreen(Transform parent, GameManager gameManager, UIManager uiManager, ResourceManager resourceManager, CreatureDatabase database, CreatureStaticData fallbackCreature)
        {
            Image background = CreateImage(parent, "Background", new Color(0.34f, 0.48f, 0.32f, 1f));
            StretchFull(background.rectTransform);
            background.raycastTarget = false;
            parent.GetChild(parent.childCount - 1).SetAsFirstSibling();

            GameObject topBar = CreatePanel(parent, "TopResourceBar", new Color(0.38f, 0.23f, 0.12f, 0.78f));
            SetAnchored(topBar.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -72f), new Vector2(980f, 96f));
            HorizontalLayoutGroup topLayout = topBar.GetOrAdd<HorizontalLayoutGroup>();
            topLayout.padding = new RectOffset(16, 16, 12, 12);
            topLayout.spacing = 12f;
            topLayout.childForceExpandWidth = true;
            topLayout.childForceExpandHeight = true;

            TMP_Text coinText = CreateResourceGroup(topBar.transform, "CoinResource", "Coin", FindSpriteByKeywords("coin", "gold"));
            TMP_Text foodText = CreateResourceGroup(topBar.transform, "FoodResource", "Food", FindSpriteByKeywords("food", "drumstick", "berry"));
            TMP_Text crystalText = CreateResourceGroup(topBar.transform, "CrystalResource", "Crystal", FindSpriteByKeywords("crystal", "gem"));
            TMP_Text ticketText = CreateResourceGroup(topBar.transform, "EggTicketResource", "Ticket", FindSpriteByKeywords("ticket", "egg"));

            ResourceDisplayUI resourceDisplay = topBar.GetOrAdd<ResourceDisplayUI>();
            SetObject(resourceDisplay, "resourceManager", resourceManager);
            SetObject(resourceDisplay, "coinText", coinText);
            SetObject(resourceDisplay, "foodText", foodText);
            SetObject(resourceDisplay, "crystalText", crystalText);
            SetObject(resourceDisplay, "eggTicketText", ticketText);

            Image featuredImage = CreateImage(parent, "FeaturedCreatureImage", Color.white);
            SetAnchored(featuredImage.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 180f), new Vector2(520f, 520f));
            featuredImage.preserveAspect = true;
            featuredImage.raycastTarget = false;

            GameObject infoCard = CreatePanel(parent, "CreatureInfoCard", new Color(0.95f, 0.76f, 0.42f, 0.92f));
            SetAnchored(infoCard.GetComponent<RectTransform>(), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -270f), new Vector2(760f, 230f));
            Image portrait = CreateImage(infoCard.transform, "CreaturePortraitImage", Color.white);
            SetAnchored(portrait.rectTransform, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(105f, 0f), new Vector2(170f, 170f));
            portrait.preserveAspect = true;

            TMP_Text nameText = CreateText(infoCard.transform, "CreatureNameText", "No Creature Yet", 42, TextAlignmentOptions.Left);
            SetAnchored(nameText.rectTransform, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(450f, -55f), new Vector2(460f, 60f));
            TMP_Text rarityText = CreateText(infoCard.transform, "RarityText", "Common", 30, TextAlignmentOptions.Left);
            SetAnchored(rarityText.rectTransform, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(450f, -108f), new Vector2(460f, 48f));
            TMP_Text levelText = CreateText(infoCard.transform, "LevelText", "Lv. 1", 30, TextAlignmentOptions.Left);
            SetAnchored(levelText.rectTransform, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(450f, -155f), new Vector2(220f, 48f));
            TMP_Text powerText = CreateText(infoCard.transform, "PowerText", "0", 30, TextAlignmentOptions.Left);
            SetAnchored(powerText.rectTransform, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(650f, -155f), new Vector2(220f, 48f));

            Button dailyButton = CreateButton(parent, "DailyRewardBanner", "Daily Reward");
            SetAnchored(dailyButton.GetComponent<RectTransform>(), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 250f), new Vector2(820f, 120f));
            TMP_Text dailyTitle = CreateText(dailyButton.transform, "TitleText", "Daily Reward", 34, TextAlignmentOptions.Center);
            SetAnchored(dailyTitle.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 22f), new Vector2(760f, 44f));
            TMP_Text dailyBody = CreateText(dailyButton.transform, "BodyText", "Claim your free rewards!", 26, TextAlignmentOptions.Center);
            SetAnchored(dailyBody.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -24f), new Vector2(760f, 40f));
            DailyRewardButtonUI dailyRewardButtonUI = dailyButton.gameObject.GetOrAdd<DailyRewardButtonUI>();
            SetObject(dailyRewardButtonUI, "dailyRewardManager", Object.FindObjectOfType<DailyRewardManager>());
            SetObject(dailyRewardButtonUI, "claimButton", dailyButton);
            SetObject(dailyRewardButtonUI, "titleText", dailyTitle);
            SetObject(dailyRewardButtonUI, "bodyText", dailyBody);
            SetObject(dailyRewardButtonUI, "buttonText", dailyTitle);

            GameObject nav = CreatePanel(parent, "BottomNavigation", new Color(0.2f, 0.12f, 0.08f, 0.72f));
            SetAnchored(nav.GetComponent<RectTransform>(), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 80f), new Vector2(1000f, 145f));
            HorizontalLayoutGroup navLayout = nav.GetOrAdd<HorizontalLayoutGroup>();
            navLayout.padding = new RectOffset(24, 24, 18, 18);
            navLayout.spacing = 18f;
            navLayout.childForceExpandWidth = true;
            navLayout.childForceExpandHeight = true;

            Button hatchButton = CreateButton(nav.transform, "HatchButton", "Hatch");
            Button collectionButton = CreateButton(nav.transform, "CollectionButton", "Collection");
            Button expeditionButton = CreateButton(nav.transform, "ExpeditionButton", "Expedition");

            HomeScreenUI homeUi = parent.gameObject.GetOrAdd<HomeScreenUI>();
            SetObject(homeUi, "gameManager", gameManager);
            SetObject(homeUi, "uiManager", uiManager);
            SetObject(homeUi, "creatureDatabase", database);
            SetObject(homeUi, "topResourceBar", topBar);
            SetObject(homeUi, "featuredCreatureImage", featuredImage);
            SetObject(homeUi, "placeholderCreatureSprite", GetFirstSprite());
            SetObject(homeUi, "fallbackFeaturedCreature", fallbackCreature);
            SetObject(homeUi, "creaturePortraitImage", portrait);
            SetObject(homeUi, "creatureNameText", nameText);
            SetObject(homeUi, "rarityText", rarityText);
            SetObject(homeUi, "levelText", levelText);
            SetObject(homeUi, "powerText", powerText);
            SetObject(homeUi, "dailyRewardBanner", dailyButton.gameObject);
            SetObject(homeUi, "dailyRewardButton", dailyButton);
            SetObject(homeUi, "dailyRewardTitleText", dailyTitle);
            SetObject(homeUi, "dailyRewardBodyText", dailyBody);
            SetObject(homeUi, "hatchButton", hatchButton);
            SetObject(homeUi, "collectionButton", collectionButton);
            SetObject(homeUi, "expeditionButton", expeditionButton);
            SetObject(homeUi, "hatchButtonText", hatchButton.GetComponentInChildren<TMP_Text>());
            SetObject(homeUi, "collectionButtonText", collectionButton.GetComponentInChildren<TMP_Text>());
            SetObject(homeUi, "expeditionButtonText", expeditionButton.GetComponentInChildren<TMP_Text>());
        }

        private static void BuildHatchScreen(Transform parent, HatchManager hatchManager, ResourceManager resourceManager, MockRewardedAdService adService, UIManager uiManager)
        {
            AddBackground(parent, new Color(0.48f, 0.28f, 0.18f, 1f));
            Image eggPreview = CreateImage(parent, "EggPreviewImage", new Color(1f, 0.82f, 0.42f, 0.85f));
            SetAnchored(eggPreview.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 150f), new Vector2(520f, 520f));
            eggPreview.raycastTarget = false;

            Button hatchButton = CreateButton(parent, "HatchButton", "Hatch");
            SetAnchored(hatchButton.GetComponent<RectTransform>(), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 260f), new Vector2(520f, 120f));
            HatchButtonUI hatchUi = hatchButton.gameObject.GetOrAdd<HatchButtonUI>();
            SetObject(hatchUi, "hatchManager", hatchManager);
            SetObject(hatchUi, "resourceManager", resourceManager);
            SetObject(hatchUi, "hatchButton", hatchButton);
            SetObject(hatchUi, "costText", hatchButton.GetComponentInChildren<TMP_Text>());

            Button adButton = CreateButton(parent, "WatchAdButton", "Watch Ad");
            SetAnchored(adButton.GetComponent<RectTransform>(), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 140f), new Vector2(520f, 90f));
            RewardedAdButtonUI adUi = adButton.gameObject.GetOrAdd<RewardedAdButtonUI>();
            SetObject(adUi, "rewardedAdService", adService);
            SetObject(adUi, "resourceManager", resourceManager);
            SetObject(adUi, "button", adButton);
            SetObject(adUi, "buttonText", adButton.GetComponentInChildren<TMP_Text>());

            CreateBackButton(parent, uiManager, GameScreen.Home);
        }

        private static void BuildCollectionScreen(Transform parent, GameManager gameManager, UIManager uiManager, HatchManager hatchManager, CreatureDatabase database)
        {
            AddBackground(parent, new Color(0.27f, 0.43f, 0.33f, 1f));
            CreateBackButton(parent, uiManager, GameScreen.Home);

            ScrollRect scrollRect = CreateScrollView(parent, "CreatureScrollView", new Vector2(0f, -80f), new Vector2(900f, 1300f), out Transform content);
            GridLayoutGroup grid = content.gameObject.GetOrAdd<GridLayoutGroup>();
            grid.cellSize = new Vector2(260f, 310f);
            grid.spacing = new Vector2(24f, 24f);
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = 3;

            TMP_Text messageText = CreateText(parent, "MessageText", "", 30, TextAlignmentOptions.Center);
            SetAnchored(messageText.rectTransform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 220f), new Vector2(800f, 80f));

            CreatureCardUI cardPrefab = CreateCreatureCardPrefab();
            CollectionScreenUI collectionUi = parent.gameObject.GetOrAdd<CollectionScreenUI>();
            SetObject(collectionUi, "gameManager", gameManager);
            SetObject(collectionUi, "uiManager", uiManager);
            SetObject(collectionUi, "hatchManager", hatchManager);
            SetObject(collectionUi, "creatureDatabase", database);
            SetObject(collectionUi, "cardParent", content);
            SetObject(collectionUi, "creatureCardPrefab", cardPrefab);
            SetObject(collectionUi, "lockedPlaceholderSprite", GetFirstSprite());
            SetObject(collectionUi, "messageText", messageText);
        }

        private static void BuildCreatureDetailScreen(Transform parent, UIManager uiManager, CreatureProgressionManager progressionManager)
        {
            AddBackground(parent, new Color(0.23f, 0.38f, 0.45f, 1f));
            CreateBackButton(parent, uiManager, GameScreen.Collection);

            Image creatureImage = CreateImage(parent, "CreatureImage", Color.white);
            SetAnchored(creatureImage.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 390f), new Vector2(560f, 560f));
            creatureImage.preserveAspect = true;

            TMP_Text nameText = CreateText(parent, "NameText", "Creature", 48, TextAlignmentOptions.Center);
            SetAnchored(nameText.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 70f), new Vector2(840f, 80f));
            TMP_Text elementText = CreateText(parent, "ElementText", "Element", 30, TextAlignmentOptions.Center);
            SetAnchored(elementText.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 10f), new Vector2(840f, 52f));
            TMP_Text rarityText = CreateText(parent, "RarityText", "Rarity", 30, TextAlignmentOptions.Center);
            SetAnchored(rarityText.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -45f), new Vector2(840f, 52f));
            TMP_Text formText = CreateText(parent, "FormNameText", "Form", 30, TextAlignmentOptions.Center);
            SetAnchored(formText.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -100f), new Vector2(840f, 52f));
            TMP_Text levelText = CreateText(parent, "LevelText", "Lv. 1", 30, TextAlignmentOptions.Center);
            SetAnchored(levelText.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(-160f, -155f), new Vector2(300f, 52f));
            TMP_Text powerText = CreateText(parent, "PowerText", "0", 30, TextAlignmentOptions.Center);
            SetAnchored(powerText.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(160f, -155f), new Vector2(300f, 52f));
            TMP_Text messageText = CreateText(parent, "MessageText", "", 28, TextAlignmentOptions.Center);
            SetAnchored(messageText.rectTransform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 390f), new Vector2(820f, 90f));

            Button upgradeButton = CreateButton(parent, "UpgradeButton", "Upgrade");
            SetAnchored(upgradeButton.GetComponent<RectTransform>(), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(-230f, 210f), new Vector2(360f, 110f));
            Button evolveButton = CreateButton(parent, "EvolveButton", "Evolve");
            SetAnchored(evolveButton.GetComponent<RectTransform>(), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(230f, 210f), new Vector2(360f, 110f));

            CreatureDetailScreenUI detailUi = parent.gameObject.GetOrAdd<CreatureDetailScreenUI>();
            SetObject(detailUi, "uiManager", uiManager);
            SetObject(detailUi, "progressionManager", progressionManager);
            SetObject(detailUi, "creatureImage", creatureImage);
            SetObject(detailUi, "placeholderSprite", GetFirstSprite());
            SetObject(detailUi, "creatureNameText", nameText);
            SetObject(detailUi, "elementText", elementText);
            SetObject(detailUi, "rarityText", rarityText);
            SetObject(detailUi, "formNameText", formText);
            SetObject(detailUi, "levelText", levelText);
            SetObject(detailUi, "powerText", powerText);
            SetObject(detailUi, "messageText", messageText);
            SetObject(detailUi, "upgradeButton", upgradeButton);
            SetObject(detailUi, "evolveButton", evolveButton);
            SetObject(detailUi, "upgradeButtonText", upgradeButton.GetComponentInChildren<TMP_Text>());
            SetObject(detailUi, "evolveButtonText", evolveButton.GetComponentInChildren<TMP_Text>());
        }

        private static void BuildExpeditionScreen(Transform parent, ExpeditionManager expeditionManager, CreatureDatabase database, UIManager uiManager)
        {
            AddBackground(parent, new Color(0.25f, 0.26f, 0.42f, 1f));
            CreateBackButton(parent, uiManager, GameScreen.Home);

            GameObject list = CreatePanel(parent, "ExpeditionList", new Color(0f, 0f, 0f, 0f));
            SetAnchored(list.GetComponent<RectTransform>(), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -70f), new Vector2(900f, 1180f));
            VerticalLayoutGroup layout = list.GetOrAdd<VerticalLayoutGroup>();
            layout.padding = new RectOffset(0, 0, 20, 20);
            layout.spacing = 22f;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            TMP_Text messageText = CreateText(parent, "MessageText", "", 30, TextAlignmentOptions.Center);
            SetAnchored(messageText.rectTransform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 220f), new Vector2(840f, 80f));

            CreatureSelectPopupUI selectPopup = Object.FindObjectOfType<CreatureSelectPopupUI>();
            ExpeditionScreenUI expeditionUi = parent.gameObject.GetOrAdd<ExpeditionScreenUI>();
            SetObject(expeditionUi, "expeditionManager", expeditionManager);
            SetObject(expeditionUi, "creatureSelectPopupUI", selectPopup);
            SetObject(expeditionUi, "cardParent", list.transform);
            SetObject(expeditionUi, "expeditionCardPrefab", CreateExpeditionCardPrefab());
            SetObject(expeditionUi, "messageText", messageText);
        }

        private static void BuildPopups(Transform parent, HatchManager hatchManager, ExpeditionManager expeditionManager, CreatureDatabase database, DailyRewardManager dailyRewardManager)
        {
            GameObject hatchController = GetOrCreateChild(parent, "HatchResultPopupController");
            HatchResultPopupUI hatchPopup = hatchController.GetOrAdd<HatchResultPopupUI>();
            GameObject hatchPanel = CreatePanel(hatchController.transform, "HatchResultPopupPanel", new Color(0.94f, 0.72f, 0.38f, 0.96f));
            SetAnchored(hatchPanel.GetComponent<RectTransform>(), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(760f, 900f));
            TMP_Text creatureName = CreateText(hatchPanel.transform, "CreatureNameText", "Creature Name", 48, TextAlignmentOptions.Center);
            SetAnchored(creatureName.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -80f), new Vector2(650f, 90f));
            TMP_Text rarity = CreateText(hatchPanel.transform, "RarityText", "Common", 36, TextAlignmentOptions.Center);
            SetAnchored(rarity.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -160f), new Vector2(600f, 70f));
            Image creatureImage = CreateImage(hatchPanel.transform, "CreatureImage", Color.white);
            SetAnchored(creatureImage.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 60f), new Vector2(480f, 480f));
            TMP_Text duplicateText = CreateText(hatchPanel.transform, "DuplicateRewardText", "Duplicate Reward", 30, TextAlignmentOptions.Center);
            SetAnchored(duplicateText.rectTransform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 210f), new Vector2(650f, 80f));
            Button closeButton = CreateButton(hatchPanel.transform, "CloseButton", "Close");
            SetAnchored(closeButton.GetComponent<RectTransform>(), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 90f), new Vector2(360f, 90f));
            SetObject(hatchPopup, "hatchManager", hatchManager);
            SetObject(hatchPopup, "popupRoot", hatchPanel);
            SetObject(hatchPopup, "creatureNameText", creatureName);
            SetObject(hatchPopup, "rarityText", rarity);
            SetObject(hatchPopup, "creatureImage", creatureImage);
            SetObject(hatchPopup, "duplicateRewardText", duplicateText);
            SetObject(hatchPopup, "closeButton", closeButton);
            hatchPanel.SetActive(false);

            GameObject selectController = GetOrCreateChild(parent, "CreatureSelectPopupController");
            CreatureSelectPopupUI selectPopup = selectController.GetOrAdd<CreatureSelectPopupUI>();
            GameObject selectPanel = CreatePanel(selectController.transform, "CreatureSelectPopupPanel", new Color(0.21f, 0.31f, 0.22f, 0.96f));
            SetAnchored(selectPanel.GetComponent<RectTransform>(), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(820f, 1020f));
            TMP_Text title = CreateText(selectPanel.transform, "TitleText", "Select Creature", 42, TextAlignmentOptions.Center);
            SetAnchored(title.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -70f), new Vector2(740f, 80f));
            TMP_Text message = CreateText(selectPanel.transform, "MessageText", "", 28, TextAlignmentOptions.Center);
            SetAnchored(message.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -140f), new Vector2(740f, 70f));
            GameObject buttonList = CreatePanel(selectPanel.transform, "CreatureButtonList", new Color(0f, 0f, 0f, 0f));
            SetAnchored(buttonList.GetComponent<RectTransform>(), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -30f), new Vector2(700f, 640f));
            VerticalLayoutGroup listLayout = buttonList.GetOrAdd<VerticalLayoutGroup>();
            listLayout.spacing = 14f;
            listLayout.childForceExpandWidth = true;
            Button creatureButtonPrefab = CreateCreatureSelectButtonPrefab();
            Button selectCloseButton = CreateButton(selectPanel.transform, "CloseButton", "Close");
            SetAnchored(selectCloseButton.GetComponent<RectTransform>(), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 90f), new Vector2(360f, 90f));
            SetObject(selectPopup, "expeditionManager", expeditionManager);
            SetObject(selectPopup, "creatureDatabase", database);
            SetObject(selectPopup, "popupRoot", selectPanel);
            SetObject(selectPopup, "titleText", title);
            SetObject(selectPopup, "messageText", message);
            SetObject(selectPopup, "creatureButtonParent", buttonList.transform);
            SetObject(selectPopup, "creatureButtonPrefab", creatureButtonPrefab);
            SetObject(selectPopup, "closeButton", selectCloseButton);
            selectPanel.SetActive(false);
        }

        private static void AssignManagers(GameManager gameManager, SaveManager saveManager, UIManager uiManager, ResourceManager resourceManager, HatchManager hatchManager, CreatureProgressionManager progressionManager, ExpeditionManager expeditionManager, DailyRewardManager dailyRewardManager, CreatureDatabase database, ExpeditionData[] expeditionAssets)
        {
            SetObject(gameManager, "uiManager", uiManager);
            SetObject(gameManager, "saveManager", saveManager);
            SetObject(resourceManager, "gameManager", gameManager);
            SetObject(hatchManager, "creatureDatabase", database);
            SetObject(hatchManager, "resourceManager", resourceManager);
            SetObject(hatchManager, "gameManager", gameManager);
            SetObject(hatchManager, "placeholderSprite", GetFirstSprite());
            SetObject(progressionManager, "gameManager", gameManager);
            SetObject(progressionManager, "resourceManager", resourceManager);
            SetObject(progressionManager, "creatureDatabase", database);
            SetObject(expeditionManager, "gameManager", gameManager);
            SetObject(expeditionManager, "resourceManager", resourceManager);
            SetObject(expeditionManager, "creatureDatabase", database);
            SetObject(expeditionManager, "progressionManager", progressionManager);
            SetObject(dailyRewardManager, "gameManager", gameManager);
            SetObject(dailyRewardManager, "resourceManager", resourceManager);
            SetObjectArray(expeditionManager, "expeditions", expeditionAssets);
        }

        private static void AssignUiManager(UIManager uiManager, GameObject home, GameObject hatch, GameObject collection, GameObject detail, GameObject expedition, Transform popups)
        {
            SetObject(uiManager, "homeScreen", home);
            SetObject(uiManager, "hatchScreen", hatch);
            SetObject(uiManager, "collectionScreen", collection);
            SetObject(uiManager, "creatureDetailScreen", detail);
            SetObject(uiManager, "expeditionScreen", expedition);
            Transform hatchPopup = popups.Find("HatchResultPopupController/HatchResultPopupPanel");
            SetObject(uiManager, "hatchResultPopup", hatchPopup != null ? hatchPopup.gameObject : null);
        }

        private static CreatureCardUI CreateCreatureCardPrefab()
        {
            string path = $"{PrefabCardsPath}/CreatureCardUI.prefab";
            CreatureCardUI existing = AssetDatabase.LoadAssetAtPath<CreatureCardUI>(path);
            if (existing != null)
            {
                return existing;
            }

            GameObject root = CreatePanel(null, "CreatureCardUI", new Color(0.95f, 0.76f, 0.42f, 0.95f));
            root.AddComponent<Button>();
            Image image = CreateImage(root.transform, "CreatureImage", Color.white);
            SetAnchored(image.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -100f), new Vector2(180f, 180f));
            Image locked = CreateImage(root.transform, "LockedOverlay", new Color(0f, 0f, 0f, 0.45f));
            StretchFull(locked.rectTransform);
            TMP_Text name = CreateText(root.transform, "NameText", "???", 28, TextAlignmentOptions.Center);
            SetAnchored(name.rectTransform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 82f), new Vector2(230f, 44f));
            TMP_Text rarity = CreateText(root.transform, "RarityText", "", 22, TextAlignmentOptions.Center);
            SetAnchored(rarity.rectTransform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 46f), new Vector2(230f, 36f));
            TMP_Text level = CreateText(root.transform, "LevelText", "", 22, TextAlignmentOptions.Center);
            SetAnchored(level.rectTransform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 16f), new Vector2(230f, 34f));
            CreatureCardUI card = root.GetOrAdd<CreatureCardUI>();
            SetObject(card, "button", root.GetComponent<Button>());
            SetObject(card, "creatureImage", image);
            SetObject(card, "lockedOverlayImage", locked);
            SetObject(card, "creatureNameText", name);
            SetObject(card, "rarityText", rarity);
            SetObject(card, "levelText", level);

            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
            Object.DestroyImmediate(root);
            return prefab.GetComponent<CreatureCardUI>();
        }

        private static ExpeditionCardUI CreateExpeditionCardPrefab()
        {
            string path = $"{PrefabCardsPath}/ExpeditionCardUI.prefab";
            ExpeditionCardUI existing = AssetDatabase.LoadAssetAtPath<ExpeditionCardUI>(path);
            if (existing != null)
            {
                return existing;
            }

            GameObject root = CreatePanel(null, "ExpeditionCardUI", new Color(0.92f, 0.71f, 0.38f, 0.95f));
            LayoutElement layout = root.GetOrAdd<LayoutElement>();
            layout.preferredHeight = 250f;
            TMP_Text name = CreateText(root.transform, "NameText", "Expedition", 34, TextAlignmentOptions.Left);
            SetAnchored(name.rectTransform, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(240f, -40f), new Vector2(420f, 52f));
            TMP_Text duration = CreateText(root.transform, "DurationText", "Duration", 24, TextAlignmentOptions.Left);
            SetAnchored(duration.rectTransform, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(240f, -88f), new Vector2(500f, 40f));
            TMP_Text reward = CreateText(root.transform, "RewardText", "Rewards", 22, TextAlignmentOptions.Left);
            SetAnchored(reward.rectTransform, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(240f, -130f), new Vector2(580f, 42f));
            TMP_Text status = CreateText(root.transform, "StatusText", "Ready", 24, TextAlignmentOptions.Left);
            SetAnchored(status.rectTransform, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(240f, -178f), new Vector2(360f, 40f));
            Button start = CreateButton(root.transform, "StartButton", "Start");
            SetAnchored(start.GetComponent<RectTransform>(), new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(-125f, 40f), new Vector2(190f, 72f));
            Button claim = CreateButton(root.transform, "ClaimButton", "Claim");
            SetAnchored(claim.GetComponent<RectTransform>(), new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(-125f, -48f), new Vector2(190f, 72f));
            ExpeditionCardUI card = root.GetOrAdd<ExpeditionCardUI>();
            SetObject(card, "nameText", name);
            SetObject(card, "durationText", duration);
            SetObject(card, "rewardText", reward);
            SetObject(card, "statusText", status);
            SetObject(card, "startButton", start);
            SetObject(card, "claimButton", claim);

            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
            Object.DestroyImmediate(root);
            return prefab.GetComponent<ExpeditionCardUI>();
        }

        private static Button CreateCreatureSelectButtonPrefab()
        {
            string path = $"{PrefabUiPath}/CreatureSelectButton.prefab";
            Button existing = AssetDatabase.LoadAssetAtPath<Button>(path);
            if (existing != null)
            {
                return existing;
            }

            Button button = CreateButton(null, "CreatureSelectButton", "Creature Lv. 1");
            LayoutElement layout = button.gameObject.GetOrAdd<LayoutElement>();
            layout.preferredHeight = 90f;
            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(button.gameObject, path);
            Object.DestroyImmediate(button.gameObject);
            return prefab.GetComponent<Button>();
        }

        private static void CreateBackButton(Transform parent, UIManager uiManager, GameScreen target)
        {
            Button back = CreateButton(parent, "BackButton", "Back");
            SetAnchored(back.GetComponent<RectTransform>(), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(120f, -110f), new Vector2(180f, 70f));
            BackButtonUI backUi = back.gameObject.GetOrAdd<BackButtonUI>();
            SetObject(backUi, "uiManager", uiManager);
            SetObject(backUi, "backButton", back);
            SetObject(backUi, "buttonText", back.GetComponentInChildren<TMP_Text>());
            SetEnum(backUi, "targetScreen", (int)target);
        }

        private static TMP_Text CreateResourceGroup(Transform parent, string name, string label, Sprite iconSprite)
        {
            GameObject group = CreatePanel(parent, name, new Color(0.98f, 0.8f, 0.4f, 0.82f));
            HorizontalLayoutGroup layout = group.GetOrAdd<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(12, 12, 8, 8);
            layout.spacing = 8f;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = true;

            Image icon = CreateImage(group.transform, "Icon", new Color(1f, 0.88f, 0.28f, 1f));
            icon.sprite = iconSprite;
            icon.color = iconSprite != null ? Color.white : new Color(1f, 1f, 1f, 0f);
            icon.preserveAspect = true;
            icon.raycastTarget = false;
            LayoutElement iconLayout = icon.gameObject.GetOrAdd<LayoutElement>();
            iconLayout.preferredWidth = 48f;
            iconLayout.preferredHeight = 48f;
            TMP_Text amount = CreateText(group.transform, "AmountText", "0", 30, TextAlignmentOptions.MidlineLeft);
            LayoutElement textLayout = amount.gameObject.GetOrAdd<LayoutElement>();
            textLayout.flexibleWidth = 1f;
            group.name = name;
            icon.name = $"{label}Icon";
            return amount;
        }

        private static void FixResourceGroup(Transform topBar, string groupName, string iconName, Sprite iconSprite)
        {
            Transform groupTransform = topBar.Find(groupName);
            if (groupTransform == null)
            {
                Debug.LogWarning($"{groupName} was not found under TopResourceBar.");
                return;
            }

            GameObject group = groupTransform.gameObject;
            Image groupImage = group.GetComponent<Image>();
            if (groupImage != null)
            {
                groupImage.enabled = true;
                groupImage.sprite = null;
                groupImage.color = new Color(0.28f, 0.16f, 0.08f, 0.72f);
                groupImage.raycastTarget = false;
            }

            LayoutElement groupLayout = group.GetOrAdd<LayoutElement>();
            groupLayout.preferredWidth = 220f;
            groupLayout.preferredHeight = 70f;
            groupLayout.flexibleWidth = 1f;
            groupLayout.flexibleHeight = 0f;

            HorizontalLayoutGroup groupHorizontalLayout = group.GetOrAdd<HorizontalLayoutGroup>();
            groupHorizontalLayout.childAlignment = TextAnchor.MiddleCenter;
            groupHorizontalLayout.padding = new RectOffset(12, 12, 8, 8);
            groupHorizontalLayout.spacing = 8f;
            groupHorizontalLayout.childForceExpandWidth = false;
            groupHorizontalLayout.childForceExpandHeight = false;

            Transform iconTransform = groupTransform.Find(iconName);
            if (iconTransform == null)
            {
                iconTransform = groupTransform.Find("Icon");
            }

            if (iconTransform == null)
            {
                GameObject iconObject = new GameObject(iconName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(LayoutElement));
                iconObject.transform.SetParent(groupTransform, false);
                iconTransform = iconObject.transform;
            }

            iconTransform.name = iconName;
            iconTransform.SetSiblingIndex(0);

            Image iconImage = iconTransform.GetComponent<Image>() ?? iconTransform.gameObject.AddComponent<Image>();
            iconImage.sprite = iconSprite;
            iconImage.color = iconSprite != null ? Color.white : new Color(1f, 1f, 1f, 0f);
            iconImage.preserveAspect = true;
            iconImage.raycastTarget = false;
            iconImage.enabled = true;

            LayoutElement iconLayout = iconTransform.GetComponent<LayoutElement>() ?? iconTransform.gameObject.AddComponent<LayoutElement>();
            iconLayout.preferredWidth = 48f;
            iconLayout.preferredHeight = 48f;
            iconLayout.flexibleWidth = 0f;
            iconLayout.flexibleHeight = 0f;

            RectTransform iconRect = iconTransform.GetComponent<RectTransform>();
            iconRect.localScale = Vector3.one;
            iconRect.sizeDelta = new Vector2(48f, 48f);

            TMP_Text amountText = groupTransform.GetComponentInChildren<TMP_Text>(true);
            if (amountText != null)
            {
                amountText.alignment = TextAlignmentOptions.MidlineLeft;
                amountText.fontSize = 30f;
                amountText.color = new Color(0.2f, 0.11f, 0.06f, 1f);
                amountText.transform.SetAsLastSibling();

                LayoutElement textLayout = amountText.GetComponent<LayoutElement>() ?? amountText.gameObject.AddComponent<LayoutElement>();
                textLayout.flexibleWidth = 1f;
                textLayout.preferredHeight = 48f;
            }
        }

        private static ScrollRect CreateScrollView(Transform parent, string name, Vector2 position, Vector2 size, out Transform content)
        {
            GameObject scrollObject = CreatePanel(parent, name, new Color(0f, 0f, 0f, 0.12f));
            SetAnchored(scrollObject.GetComponent<RectTransform>(), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), position, size);
            ScrollRect scroll = scrollObject.GetOrAdd<ScrollRect>();

            GameObject viewport = CreatePanel(scrollObject.transform, "Viewport", new Color(0f, 0f, 0f, 0f));
            StretchFull(viewport.GetComponent<RectTransform>());
            Mask mask = viewport.GetOrAdd<Mask>();
            mask.showMaskGraphic = false;
            Image viewportImage = viewport.GetComponent<Image>();
            viewportImage.raycastTarget = true;

            GameObject contentObject = CreatePanel(viewport.transform, "Content", new Color(0f, 0f, 0f, 0f));
            RectTransform contentRect = contentObject.GetComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0f, 1f);
            contentRect.anchorMax = new Vector2(1f, 1f);
            contentRect.pivot = new Vector2(0.5f, 1f);
            contentRect.anchoredPosition = Vector2.zero;
            contentRect.sizeDelta = new Vector2(0f, 1200f);
            ContentSizeFitter fitter = contentObject.GetOrAdd<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            scroll.viewport = viewport.GetComponent<RectTransform>();
            scroll.content = contentRect;
            scroll.horizontal = false;
            scroll.vertical = true;
            content = contentObject.transform;
            return scroll;
        }

        private static void AddBackground(Transform parent, Color color)
        {
            Image background = CreateImage(parent, "Background", color);
            StretchFull(background.rectTransform);
            background.raycastTarget = false;
            background.transform.SetAsFirstSibling();
        }

        private static Canvas GetOrCreateCanvas()
        {
            Canvas canvas = Object.FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                GameObject canvasObject = new GameObject("MainCanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
                canvas = canvasObject.GetComponent<Canvas>();
            }

            canvas.name = "MainCanvas";
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvas.GetComponent<CanvasScaler>() ?? canvas.gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = GameConstants.ReferenceResolution;
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
            if (canvas.GetComponent<GraphicRaycaster>() == null)
            {
                canvas.gameObject.AddComponent<GraphicRaycaster>();
            }

            return canvas;
        }

        private static void EnsureEventSystem()
        {
            if (Object.FindObjectOfType<EventSystem>() != null)
            {
                return;
            }

            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        }

        private static GameObject CreateScreen(Transform canvas, string name, bool active)
        {
            GameObject screen = GetOrCreateChild(canvas, name);
            RectTransform rectTransform = screen.GetComponent<RectTransform>() ?? screen.AddComponent<RectTransform>();
            StretchFull(rectTransform);
            screen.SetActive(active);
            return screen;
        }

        private static Image CreateImage(Transform parent, string name, Color color)
        {
            GameObject obj = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            if (parent != null)
            {
                obj.transform.SetParent(parent, false);
            }

            Image image = obj.GetComponent<Image>();
            image.color = color;
            return image;
        }

        private static GameObject CreatePanel(Transform parent, string name, Color color)
        {
            Image image = CreateImage(parent, name, color);
            return image.gameObject;
        }

        private static Button CreateButton(Transform parent, string name, string label)
        {
            GameObject buttonObject = CreatePanel(parent, name, new Color(0.93f, 0.58f, 0.22f, 1f));
            Button button = buttonObject.GetOrAdd<Button>();
            TMP_Text text = CreateText(buttonObject.transform, "Text (TMP)", label, 32, TextAlignmentOptions.Center);
            StretchFull(text.rectTransform);
            text.raycastTarget = false;
            return button;
        }

        private static TMP_Text CreateText(Transform parent, string name, string value, float fontSize, TextAlignmentOptions alignment)
        {
            GameObject textObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
            if (parent != null)
            {
                textObject.transform.SetParent(parent, false);
            }

            TMP_Text text = textObject.GetComponent<TMP_Text>();
            text.text = value;
            text.fontSize = fontSize;
            text.alignment = alignment;
            text.color = new Color(0.22f, 0.12f, 0.07f, 1f);
            text.enableWordWrapping = true;
            return text;
        }

        private static T GetOrCreateManager<T>(Transform parent, string name) where T : Component
        {
            GameObject obj = GetOrCreateChild(parent, name);
            return obj.GetOrAdd<T>();
        }

        private static T GetOrCreateComponentOnChild<T>(Transform parent, string name) where T : Component
        {
            GameObject obj = GetOrCreateChild(parent, name);
            return obj.GetOrAdd<T>();
        }

        private static GameObject GetOrCreateRoot(string name)
        {
            GameObject existing = GameObject.Find(name);
            return existing != null ? existing : new GameObject(name);
        }

        private static GameObject GetOrCreateChild(Transform parent, string name)
        {
            Transform existing = parent.Find(name);
            if (existing != null)
            {
                return existing.gameObject;
            }

            GameObject child = new GameObject(name, typeof(RectTransform));
            child.transform.SetParent(parent, false);
            return child;
        }

        private static void StretchFull(RectTransform rectTransform)
        {
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
        }

        private static void SetAnchored(RectTransform rectTransform, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition, Vector2 sizeDelta)
        {
            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = anchoredPosition;
            rectTransform.sizeDelta = sizeDelta;
        }

        private static void SetObject(Object target, string propertyName, Object value)
        {
            if (target == null)
            {
                return;
            }

            SerializedObject serializedObject = new SerializedObject(target);
            SerializedProperty property = serializedObject.FindProperty(propertyName);
            if (property == null)
            {
                return;
            }

            property.objectReferenceValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(target);
        }

        private static void SetEnum(Object target, string propertyName, int value)
        {
            if (target == null)
            {
                return;
            }

            SerializedObject serializedObject = new SerializedObject(target);
            SerializedProperty property = serializedObject.FindProperty(propertyName);
            if (property == null)
            {
                return;
            }

            property.enumValueIndex = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(target);
        }

        private static void SetObjectArray<T>(Object target, string propertyName, T[] values) where T : Object
        {
            if (target == null || values == null)
            {
                return;
            }

            SerializedObject serializedObject = new SerializedObject(target);
            SerializedProperty property = serializedObject.FindProperty(propertyName);
            if (property == null || !property.isArray)
            {
                return;
            }

            property.arraySize = values.Length;
            for (int i = 0; i < values.Length; i++)
            {
                property.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
            }

            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(target);
        }

        private static T FindFirstAsset<T>() where T : Object
        {
            string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
            if (guids.Length == 0)
            {
                return null;
            }

            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<T>(path);
        }

        private static T[] FindAllAssets<T>() where T : Object
        {
            string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
            T[] assets = new T[guids.Length];
            for (int i = 0; i < guids.Length; i++)
            {
                assets[i] = AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guids[i]));
            }

            return assets;
        }

        private static Sprite GetFirstSprite()
        {
            return FindFirstAsset<Sprite>();
        }

        private static Sprite FindSpriteByKeywords(params string[] keywords)
        {
            string[] guids = AssetDatabase.FindAssets("t:Sprite", new[] { "Assets/_Project/Sprites", "Assets/_Project/Art" });

            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                string lowerPath = path.ToLowerInvariant();
                bool matches = true;

                for (int keywordIndex = 0; keywordIndex < keywords.Length; keywordIndex++)
                {
                    if (!lowerPath.Contains(keywords[keywordIndex].ToLowerInvariant()))
                    {
                        matches = false;
                        break;
                    }
                }

                if (matches)
                {
                    return AssetDatabase.LoadAssetAtPath<Sprite>(path);
                }
            }

            return null;
        }

        private static void EnsureFolders()
        {
            CreateFolderIfMissing("Assets/_Project", "Prefabs");
            CreateFolderIfMissing("Assets/_Project/Scripts", "Editor");
            CreateFolderIfMissing("Assets/_Project/Prefabs", "UI");
            CreateFolderIfMissing("Assets/_Project/Prefabs", "Cards");
        }

        private static void CreateFolderIfMissing(string parent, string child)
        {
            string path = $"{parent}/{child}";
            if (!AssetDatabase.IsValidFolder(path))
            {
                AssetDatabase.CreateFolder(parent, child);
            }
        }
    }

    public static class GameObjectEditorExtensions
    {
        public static T GetOrAdd<T>(this GameObject gameObject) where T : Component
        {
            T component = gameObject.GetComponent<T>();
            return component != null ? component : gameObject.AddComponent<T>();
        }
    }
}
