using UnityEngine;

namespace SpiritHatchers.Core
{
    /// <summary>
    /// Shared values used by the MVP. Keeping these in one place makes the
    /// beginner-facing gameplay scripts easier to read and tune.
    /// </summary>
    public static class GameConstants
    {
        public const string GameTitle = "Spirit Hatchers: Idle Evolution";

        public const int TargetFrameRate = 60;
        public const string SaveFileName = "save_data.json";

        public const int StartingCoins = 100;
        public const int StartingFood = 50;
        public const int StartingCrystals = 20;
        public const int StartingEggTickets = 1;

        public const int HatchCrystalCost = 10;
        public const int HatchTicketCost = 1;

        public const int CommonRarityRate = 70;
        public const int RareRarityRate = 25;
        public const int EpicRarityRate = 5;

        public const int FormTwoRequiredLevel = 10;
        public const int FormTwoFoodCost = 100;

        public const int FormThreeRequiredLevel = 25;
        public const int FormThreeFoodCost = 300;
        public const int FormThreeCrystalCost = 20;

        public const int DailyRewardCoins = 100;
        public const int DailyRewardFood = 50;
        public const int DailyRewardEggTickets = 1;

        public static readonly Vector2 ReferenceResolution = new Vector2(1080, 1920);
    }
}
