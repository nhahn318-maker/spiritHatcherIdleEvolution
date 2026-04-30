using TMPro;
using UnityEngine;
using UnityEngine.UI;
using SpiritHatchers.Core;
using SpiritHatchers.Resources;

namespace SpiritHatchers.Hatch
{
    public class HatchButtonUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private HatchManager hatchManager;
        [SerializeField] private ResourceManager resourceManager;
        [SerializeField] private Button hatchButton;
        [SerializeField] private TMP_Text costText;

        private void Awake()
        {
            if (hatchButton == null)
            {
                hatchButton = GetComponent<Button>();
            }
        }

        private void OnEnable()
        {
            FindReferencesIfNeeded();

            if (hatchButton != null)
            {
                hatchButton.onClick.AddListener(OnHatchButtonClicked);
            }

            if (resourceManager != null)
            {
                resourceManager.OnAnyResourceChanged += Refresh;
            }

            Refresh();
        }

        private void OnDisable()
        {
            if (hatchButton != null)
            {
                hatchButton.onClick.RemoveListener(OnHatchButtonClicked);
            }

            if (resourceManager != null)
            {
                resourceManager.OnAnyResourceChanged -= Refresh;
            }
        }

        public void OnHatchButtonClicked()
        {
            FindReferencesIfNeeded();

            if (hatchManager == null)
            {
                Debug.LogWarning("Hatch button clicked, but HatchManager is missing.");
                return;
            }

            hatchManager.HatchCreature();
            Refresh();
        }

        public void Refresh()
        {
            FindReferencesIfNeeded();

            if (resourceManager == null)
            {
                SetButtonInteractable(false);
                SetCostText("No Resources");
                return;
            }

            if (resourceManager.GetResource(ResourceType.EggTicket) > 0)
            {
                SetCostText($"Hatch x{GameConstants.HatchTicketCost} Ticket");
                SetButtonInteractable(true);
                return;
            }

            bool canAffordCrystal = resourceManager.CanAfford(ResourceType.Crystal, GameConstants.HatchCrystalCost);
            SetCostText($"Hatch {GameConstants.HatchCrystalCost} Crystal");
            SetButtonInteractable(canAffordCrystal);
        }

        private void FindReferencesIfNeeded()
        {
            if (hatchManager == null)
            {
                hatchManager = HatchManager.Instance;
            }

            if (hatchManager == null)
            {
                hatchManager = FindObjectOfType<HatchManager>();
            }

            if (resourceManager == null)
            {
                resourceManager = ResourceManager.Instance;
            }

            if (resourceManager == null)
            {
                resourceManager = FindObjectOfType<ResourceManager>();
            }
        }

        private void SetButtonInteractable(bool isInteractable)
        {
            if (hatchButton != null)
            {
                hatchButton.interactable = isInteractable;
            }
        }

        private void SetCostText(string text)
        {
            if (costText != null)
            {
                costText.text = text;
            }
        }
    }
}
