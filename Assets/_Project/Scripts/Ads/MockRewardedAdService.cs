using System;
using System.Collections;
using UnityEngine;

namespace SpiritHatchers.Ads
{
    public class MockRewardedAdService : MonoBehaviour, IRewardedAdService
    {
        public static MockRewardedAdService Instance { get; private set; }

        public event Action<string> OnRewardedAdCompleted;

        [SerializeField] private float simulatedAdDurationSeconds = 1f;

        private bool isShowingAd;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning("Duplicate MockRewardedAdService found. Destroying the new one.");
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public bool IsAdReady()
        {
            return !isShowingAd;
        }

        public void ShowRewardedAd(string placementId, Action onCompleted)
        {
            if (isShowingAd)
            {
                Debug.Log("Mock rewarded ad is already showing.");
                return;
            }

            StartCoroutine(SimulateRewardedAd(placementId, onCompleted));
        }

        private IEnumerator SimulateRewardedAd(string placementId, Action onCompleted)
        {
            isShowingAd = true;
            Debug.Log($"Mock rewarded ad started. Placement: {placementId}");

            yield return new WaitForSeconds(simulatedAdDurationSeconds);

            Debug.Log($"Mock rewarded ad completed. Placement: {placementId}");
            onCompleted?.Invoke();
            OnRewardedAdCompleted?.Invoke(placementId);

            isShowingAd = false;
        }
    }
}
