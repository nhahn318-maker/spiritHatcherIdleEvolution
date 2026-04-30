using UnityEngine;
using SpiritHatchers.Save;
using SpiritHatchers.UI;

namespace SpiritHatchers.Core
{
    /// <summary>
    /// First project-level bootstrap for the MVP.
    /// Add this to one GameObject in MainScene.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Scene References")]
        [SerializeField] private UIManager uiManager;
        [SerializeField] private SaveManager saveManager;

        [Header("Startup")]
        [SerializeField] private bool forcePortraitMode = true;
        [SerializeField] private bool showHomeOnStart = true;

        public PlayerSaveData CurrentSaveData { get; private set; }
        public bool IsInitialized { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning("Duplicate GameManager found. Destroying the new one.");
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            ApplyAppSettings();
        }

        private void Start()
        {
            InitializeGame();
        }

        private void ApplyAppSettings()
        {
            Application.targetFrameRate = GameConstants.TargetFrameRate;

            if (!forcePortraitMode)
            {
                return;
            }

            Screen.orientation = ScreenOrientation.Portrait;
            Screen.autorotateToPortrait = true;
            Screen.autorotateToPortraitUpsideDown = false;
            Screen.autorotateToLandscapeLeft = false;
            Screen.autorotateToLandscapeRight = false;
        }

        public void InitializeGame()
        {
            if (IsInitialized)
            {
                return;
            }

            if (uiManager == null)
            {
                uiManager = FindObjectOfType<UIManager>();
            }

            if (saveManager == null)
            {
                saveManager = FindObjectOfType<SaveManager>();
            }

            if (saveManager == null)
            {
                saveManager = gameObject.AddComponent<SaveManager>();
                Debug.Log("SaveManager was missing, so GameManager added one automatically.");
            }

            CurrentSaveData = saveManager.LoadGame();

            IsInitialized = true;
            Debug.Log($"{GameConstants.GameTitle} initialized.");

            if (showHomeOnStart && uiManager != null)
            {
                uiManager.ShowHomeScreen();
            }
        }

        public void SaveGame()
        {
            if (saveManager == null)
            {
                Debug.LogWarning("Cannot save because SaveManager is missing.");
                return;
            }

            if (CurrentSaveData == null)
            {
                Debug.LogWarning("Cannot save because CurrentSaveData is null.");
                return;
            }

            saveManager.SaveGame(CurrentSaveData);
        }
    }
}
