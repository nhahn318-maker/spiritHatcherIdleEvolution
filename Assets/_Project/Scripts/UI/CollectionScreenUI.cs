using System.Collections.Generic;
using TMPro;
using UnityEngine;
using SpiritHatchers.Core;
using SpiritHatchers.Data;
using SpiritHatchers.Hatch;
using SpiritHatchers.Save;

namespace SpiritHatchers.UI
{
    public class CollectionScreenUI : MonoBehaviour
    {
        [Header("Managers")]
        [SerializeField] private GameManager gameManager;
        [SerializeField] private UIManager uiManager;
        [SerializeField] private HatchManager hatchManager;

        [Header("Data")]
        [SerializeField] private CreatureDatabase creatureDatabase;

        [Header("Grid")]
        [SerializeField] private Transform cardParent;
        [SerializeField] private CreatureCardUI creatureCardPrefab;
        [SerializeField] private Sprite lockedPlaceholderSprite;

        [Header("Feedback")]
        [SerializeField] private TMP_Text messageText;

        [Header("Detail Screen")]
        [SerializeField] private CreatureDetailScreenUI creatureDetailScreenUI;

        private void OnEnable()
        {
            FindManagersIfNeeded();
            SubscribeToEvents();
            Refresh();
        }

        private void OnDisable()
        {
            UnsubscribeFromEvents();
        }

        public void Refresh()
        {
            FindManagersIfNeeded();

            if (messageText != null)
            {
                messageText.text = string.Empty;
            }

            ClearCards();

            if (creatureDatabase == null)
            {
                ShowMessage("Creature database missing.");
                return;
            }

            if (cardParent == null || creatureCardPrefab == null)
            {
                ShowMessage("Collection grid setup missing.");
                return;
            }

            List<CreatureStaticData> allCreatures = creatureDatabase.GetAllCreatures();
            Dictionary<string, PlayerCreatureData> ownedByCreatureId = BuildOwnedCreatureLookup();

            for (int i = 0; i < allCreatures.Count; i++)
            {
                CreatureStaticData creature = allCreatures[i];

                if (creature == null)
                {
                    continue;
                }

                ownedByCreatureId.TryGetValue(creature.creatureId, out PlayerCreatureData ownedCreature);

                CreatureCardUI card = Instantiate(creatureCardPrefab, cardParent);
                card.Setup(creature, ownedCreature, lockedPlaceholderSprite, HandleCardClicked);
            }
        }

        public void RefreshAfterEvolution()
        {
            Refresh();
        }

        private void HandleCardClicked(CreatureStaticData creature, PlayerCreatureData playerCreature)
        {
            if (creature == null)
            {
                return;
            }

            if (playerCreature == null)
            {
                ShowMessage("Not discovered yet");
                return;
            }

            if (creatureDetailScreenUI != null)
            {
                creatureDetailScreenUI.ShowCreature(creature, playerCreature);
            }

            if (uiManager != null)
            {
                uiManager.ShowCreatureDetailScreen();
            }
        }

        private Dictionary<string, PlayerCreatureData> BuildOwnedCreatureLookup()
        {
            Dictionary<string, PlayerCreatureData> ownedByCreatureId = new Dictionary<string, PlayerCreatureData>();

            if (gameManager == null || gameManager.CurrentSaveData == null)
            {
                return ownedByCreatureId;
            }

            PlayerSaveData saveData = gameManager.CurrentSaveData;
            saveData.EnsureListsAreValid();

            for (int i = 0; i < saveData.ownedCreatures.Count; i++)
            {
                PlayerCreatureData ownedCreature = saveData.ownedCreatures[i];

                if (ownedCreature == null || string.IsNullOrEmpty(ownedCreature.creatureId))
                {
                    continue;
                }

                if (!ownedByCreatureId.ContainsKey(ownedCreature.creatureId))
                {
                    ownedByCreatureId.Add(ownedCreature.creatureId, ownedCreature);
                }
            }

            return ownedByCreatureId;
        }

        private void ClearCards()
        {
            if (cardParent == null)
            {
                return;
            }

            for (int i = cardParent.childCount - 1; i >= 0; i--)
            {
                Destroy(cardParent.GetChild(i).gameObject);
            }
        }

        private void SubscribeToEvents()
        {
            if (hatchManager != null)
            {
                hatchManager.OnHatchCompleted += HandleHatchCompleted;
            }
        }

        private void UnsubscribeFromEvents()
        {
            if (hatchManager != null)
            {
                hatchManager.OnHatchCompleted -= HandleHatchCompleted;
            }
        }

        private void HandleHatchCompleted(HatchResult result)
        {
            Refresh();
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

            if (hatchManager == null)
            {
                hatchManager = HatchManager.Instance;
            }

            if (hatchManager == null)
            {
                hatchManager = FindObjectOfType<HatchManager>();
            }
        }

        private void ShowMessage(string message)
        {
            if (messageText != null)
            {
                messageText.text = message;
            }

            Debug.Log(message);
        }
    }
}
