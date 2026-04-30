using System;

namespace SpiritHatchers.Save
{
    [Serializable]
    public class PlayerExpeditionData
    {
        public string expeditionId;
        public string creatureInstanceId;
        public string startTime;
        public string endTime;
    }
}
