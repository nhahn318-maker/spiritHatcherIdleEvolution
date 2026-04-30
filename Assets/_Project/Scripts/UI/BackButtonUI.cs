using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SpiritHatchers.UI
{
    public class BackButtonUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private UIManager uiManager;
        [SerializeField] private Button backButton;
        [SerializeField] private TMP_Text buttonText;

        [Header("Settings")]
        [SerializeField] private GameScreen targetScreen = GameScreen.Home;
        [SerializeField] private string label = "Back";

        private void Awake()
        {
            if (backButton == null)
            {
                backButton = GetComponent<Button>();
            }
        }

        private void OnEnable()
        {
            FindUIManagerIfNeeded();

            if (backButton != null)
            {
                backButton.onClick.AddListener(OnBackClicked);
            }

            if (buttonText != null)
            {
                buttonText.text = label;
            }
        }

        private void OnDisable()
        {
            if (backButton != null)
            {
                backButton.onClick.RemoveListener(OnBackClicked);
            }
        }

        public void OnBackClicked()
        {
            FindUIManagerIfNeeded();

            if (uiManager == null)
            {
                Debug.LogWarning("Back button clicked, but UIManager is missing.");
                return;
            }

            uiManager.ShowScreen(targetScreen);
        }

        private void FindUIManagerIfNeeded()
        {
            if (uiManager == null)
            {
                uiManager = FindObjectOfType<UIManager>();
            }
        }
    }
}
