using System;

namespace SpiritHatchers.Data
{
    [Serializable]
    public class PlayerCreatureData
    {
        public string uniqueInstanceId;
        public string creatureId;
        public int level = 1;
        public int exp;
        public int currentFormIndex;
        public bool isFavorite;

        public PlayerCreatureData()
        {
        }

        public PlayerCreatureData(string uniqueInstanceId, string creatureId)
        {
            this.uniqueInstanceId = uniqueInstanceId;
            this.creatureId = creatureId;
            level = 1;
            exp = 0;
            currentFormIndex = 0;
            isFavorite = false;
        }
    }
}
