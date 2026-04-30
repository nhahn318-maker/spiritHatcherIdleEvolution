using UnityEngine;

namespace SpiritHatchers.Expedition
{
    [CreateAssetMenu(
        fileName = "ExpeditionData",
        menuName = "Spirit Hatchers/Expeditions/Expedition Data")]
    public class ExpeditionData : ScriptableObject
    {
        [Header("Identity")]
        public string expeditionId;
        public string expeditionName;
        [TextArea] public string description;

        [Header("Duration")]
        [Min(1)] public int durationSeconds = 300;

        [Header("Base Rewards")]
        [Min(0)] public int baseCoin;
        [Min(0)] public int baseFood;
        [Min(0)] public int baseCrystal;
    }
}
