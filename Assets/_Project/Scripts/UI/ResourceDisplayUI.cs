using TMPro;
using UnityEngine;
using SpiritHatchers.Resources;

namespace SpiritHatchers.UI
{
    public class ResourceDisplayUI : MonoBehaviour
    {
        [Header("Manager")]
        [SerializeField] private ResourceManager resourceManager;

        [Header("Top Resource Bar Text")]
        [SerializeField] private TMP_Text coinText;
        [SerializeField] private TMP_Text foodText;
        [SerializeField] private TMP_Text crystalText;
        [SerializeField] private TMP_Text eggTicketText;

        private void OnEnable()
        {
            FindResourceManagerIfNeeded();

            if (resourceManager != null)
            {
                resourceManager.OnAnyResourceChanged += RefreshAll;
            }

            RefreshAll();
        }

        private void OnDisable()
        {
            if (resourceManager != null)
            {
                resourceManager.OnAnyResourceChanged -= RefreshAll;
            }
        }

        public void RefreshAll()
        {
            FindResourceManagerIfNeeded();

            if (resourceManager == null)
            {
                Debug.LogWarning("ResourceDisplayUI cannot refresh because ResourceManager is missing.");
                return;
            }

            SetText(coinText, resourceManager.GetResource(ResourceType.Coin));
            SetText(foodText, resourceManager.GetResource(ResourceType.Food));
            SetText(crystalText, resourceManager.GetResource(ResourceType.Crystal));
            SetText(eggTicketText, resourceManager.GetResource(ResourceType.EggTicket));
        }

        private void FindResourceManagerIfNeeded()
        {
            if (resourceManager == null)
            {
                resourceManager = ResourceManager.Instance;
            }

            if (resourceManager == null)
            {
                resourceManager = FindObjectOfType<ResourceManager>();
            }
        }

        private void SetText(TMP_Text targetText, int value)
        {
            if (targetText == null)
            {
                return;
            }

            targetText.text = value.ToString("N0");
        }
    }
}
