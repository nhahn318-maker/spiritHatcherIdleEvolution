using System.Collections;
using UnityEngine;

namespace SpiritHatchers.UI
{
    public enum GameScreen
    {
        Home,
        Hatch,
        Collection,
        CreatureDetail,
        Expedition
    }

    /// <summary>
    /// Simple UI screen switcher for the one-scene MVP.
    /// Assign each screen panel in the Inspector.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        [Header("Main Screen Panels")]
        [SerializeField] private GameObject homeScreen;
        [SerializeField] private GameObject hatchScreen;
        [SerializeField] private GameObject collectionScreen;
        [SerializeField] private GameObject creatureDetailScreen;
        [SerializeField] private GameObject expeditionScreen;

        [Header("Popups")]
        [SerializeField] private GameObject dailyRewardPopup;
        [SerializeField] private GameObject hatchResultPopup;

        [Header("Optional Fade Transition")]
        [SerializeField] private CanvasGroup fadeCanvasGroup;
        [SerializeField] private bool useFadeTransition;
        [SerializeField] private float fadeDuration = 0.15f;

        public GameScreen CurrentScreen { get; private set; } = GameScreen.Home;

        private Coroutine transitionCoroutine;

        private void Start()
        {
            if (fadeCanvasGroup != null)
            {
                fadeCanvasGroup.alpha = 0f;
                fadeCanvasGroup.blocksRaycasts = false;
            }

            ShowScreen(CurrentScreen);
            HideAllPopups();
        }

        public void ShowHomeScreen()
        {
            ShowScreen(GameScreen.Home);
        }

        public void ShowHatchScreen()
        {
            ShowScreen(GameScreen.Hatch);
        }

        public void ShowCollectionScreen()
        {
            ShowScreen(GameScreen.Collection);
        }

        public void ShowCreatureDetailScreen()
        {
            ShowScreen(GameScreen.CreatureDetail);
        }

        public void ShowExpeditionScreen()
        {
            ShowScreen(GameScreen.Expedition);
        }

        public void ShowDailyRewardPopup()
        {
            SetPopupActive(dailyRewardPopup, true);
        }

        public void HideDailyRewardPopup()
        {
            SetPopupActive(dailyRewardPopup, false);
        }

        public void ShowHatchResultPopup()
        {
            SetPopupActive(hatchResultPopup, true);
        }

        public void HideHatchResultPopup()
        {
            SetPopupActive(hatchResultPopup, false);
        }

        public void HideAllPopups()
        {
            SetPopupActive(dailyRewardPopup, false);
            SetPopupActive(hatchResultPopup, false);
        }

        public void ShowScreen(GameScreen screen)
        {
            if (useFadeTransition && fadeCanvasGroup != null && gameObject.activeInHierarchy)
            {
                if (transitionCoroutine != null)
                {
                    StopCoroutine(transitionCoroutine);
                }

                transitionCoroutine = StartCoroutine(ShowScreenWithFade(screen));
                return;
            }

            ShowScreenInstant(screen);
        }

        private IEnumerator ShowScreenWithFade(GameScreen screen)
        {
            yield return Fade(0f, 1f);
            ShowScreenInstant(screen);
            yield return Fade(1f, 0f);
            transitionCoroutine = null;
        }

        private IEnumerator Fade(float fromAlpha, float toAlpha)
        {
            float timer = 0f;
            float duration = Mathf.Max(0.01f, fadeDuration);

            fadeCanvasGroup.blocksRaycasts = true;

            while (timer < duration)
            {
                timer += Time.unscaledDeltaTime;
                fadeCanvasGroup.alpha = Mathf.Lerp(fromAlpha, toAlpha, timer / duration);
                yield return null;
            }

            fadeCanvasGroup.alpha = toAlpha;
            fadeCanvasGroup.blocksRaycasts = toAlpha > 0f;
        }

        private void ShowScreenInstant(GameScreen screen)
        {
            CurrentScreen = screen;

            SetScreenActive(homeScreen, screen == GameScreen.Home);
            SetScreenActive(hatchScreen, screen == GameScreen.Hatch);
            SetScreenActive(collectionScreen, screen == GameScreen.Collection);
            SetScreenActive(creatureDetailScreen, screen == GameScreen.CreatureDetail);
            SetScreenActive(expeditionScreen, screen == GameScreen.Expedition);

            Debug.Log($"Showing screen: {screen}");
        }

        private void SetScreenActive(GameObject screenObject, bool isActive)
        {
            if (screenObject == null)
            {
                return;
            }

            screenObject.SetActive(isActive);
        }

        private void SetPopupActive(GameObject popupObject, bool isActive)
        {
            if (popupObject == null)
            {
                return;
            }

            popupObject.SetActive(isActive);
        }
    }
}
