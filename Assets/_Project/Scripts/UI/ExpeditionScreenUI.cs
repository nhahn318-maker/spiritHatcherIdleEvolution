using System.Collections.Generic;
using TMPro;
using UnityEngine;
using SpiritHatchers.Expedition;
using SpiritHatchers.Save;

namespace SpiritHatchers.UI
{
    public class ExpeditionScreenUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ExpeditionManager expeditionManager;
        [SerializeField] private CreatureSelectPopupUI creatureSelectPopupUI;

        [Header("Cards")]
        [SerializeField] private Transform cardParent;
        [SerializeField] private ExpeditionCardUI expeditionCardPrefab;

        [Header("Feedback")]
        [SerializeField] private TMP_Text messageText;

        private readonly List<ExpeditionCardUI> spawnedCards = new List<ExpeditionCardUI>();
        private float timerRefreshDelay;

        private void OnEnable()
        {
            FindReferencesIfNeeded();

            if (expeditionManager != null)
            {
                expeditionManager.OnExpeditionsChanged += Refresh;
            }

            Refresh();
        }

        private void OnDisable()
        {
            if (expeditionManager != null)
            {
                expeditionManager.OnExpeditionsChanged -= Refresh;
            }
        }

        private void Update()
        {
            timerRefreshDelay -= Time.deltaTime;

            if (timerRefreshDelay > 0f)
            {
                return;
            }

            timerRefreshDelay = 1f;
            RefreshCardTimers();
        }

        public void Refresh()
        {
            FindReferencesIfNeeded();
            ClearMessage();
            ClearCards();

            if (expeditionManager == null)
            {
                ShowMessage("Expedition manager missing.");
                return;
            }

            if (cardParent == null || expeditionCardPrefab == null)
            {
                ShowMessage("Expedition card setup missing.");
                return;
            }

            List<ExpeditionData> expeditions = expeditionManager.GetAllExpeditions();

            for (int i = 0; i < expeditions.Count; i++)
            {
                ExpeditionData expedition = expeditions[i];

                if (expedition == null)
                {
                    continue;
                }

                PlayerExpeditionData active = expeditionManager.GetActiveExpeditionForExpedition(expedition.expeditionId);
                ExpeditionCardUI card = Instantiate(expeditionCardPrefab, cardParent);
                card.Setup(expedition, active, expeditionManager, OpenCreatureSelect, ClaimReward);
                spawnedCards.Add(card);
            }
        }

        private void RefreshCardTimers()
        {
            for (int i = 0; i < spawnedCards.Count; i++)
            {
                if (spawnedCards[i] != null)
                {
                    spawnedCards[i].Refresh();
                }
            }
        }

        private void OpenCreatureSelect(ExpeditionData expedition)
        {
            if (creatureSelectPopupUI == null)
            {
                ShowMessage("Creature select popup missing.");
                return;
            }

            creatureSelectPopupUI.Show(expedition);
        }

        private void ClaimReward(PlayerExpeditionData activeExpedition)
        {
            if (expeditionManager == null || activeExpedition == null)
            {
                return;
            }

            if (expeditionManager.ClaimReward(activeExpedition, out ExpeditionReward reward))
            {
                ShowMessage($"Claimed: {reward.coin} Coin, {reward.food} Food, {reward.crystal} Crystal");
                Refresh();
            }
        }

        private void ClearCards()
        {
            spawnedCards.Clear();

            if (cardParent == null)
            {
                return;
            }

            for (int i = cardParent.childCount - 1; i >= 0; i--)
            {
                Destroy(cardParent.GetChild(i).gameObject);
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

            if (creatureSelectPopupUI == null)
            {
                creatureSelectPopupUI = FindObjectOfType<CreatureSelectPopupUI>();
            }
        }

        private void ClearMessage()
        {
            if (messageText != null)
            {
                messageText.text = string.Empty;
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
