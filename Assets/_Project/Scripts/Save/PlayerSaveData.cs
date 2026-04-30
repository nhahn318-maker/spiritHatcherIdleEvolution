using System;
using System.Collections.Generic;
using SpiritHatchers.Core;
using SpiritHatchers.Data;

namespace SpiritHatchers.Save
{
    [Serializable]
    public class PlayerSaveData
    {
        public int coin;
        public int food;
        public int crystal;
        public int eggTicket;

        public List<PlayerCreatureData> ownedCreatures = new List<PlayerCreatureData>();

        public string lastLoginTime;
        public string lastDailyRewardClaimDate;

        public List<PlayerExpeditionData> activeExpeditions = new List<PlayerExpeditionData>();

        public static PlayerSaveData CreateNewSave()
        {
            string now = DateTime.UtcNow.ToString("o");

            return new PlayerSaveData
            {
                coin = GameConstants.StartingCoins,
                food = GameConstants.StartingFood,
                crystal = GameConstants.StartingCrystals,
                eggTicket = GameConstants.StartingEggTickets,
                ownedCreatures = new List<PlayerCreatureData>(),
                lastLoginTime = now,
                lastDailyRewardClaimDate = string.Empty,
                activeExpeditions = new List<PlayerExpeditionData>()
            };
        }

        public void EnsureListsAreValid()
        {
            if (ownedCreatures == null)
            {
                ownedCreatures = new List<PlayerCreatureData>();
            }

            if (activeExpeditions == null)
            {
                activeExpeditions = new List<PlayerExpeditionData>();
            }
        }
    }
}
