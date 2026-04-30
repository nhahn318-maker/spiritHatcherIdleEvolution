using System;

namespace SpiritHatchers.Ads
{
    public interface IRewardedAdService
    {
        event Action<string> OnRewardedAdCompleted;

        bool IsAdReady();
        void ShowRewardedAd(string placementId, Action onCompleted);
    }
}
