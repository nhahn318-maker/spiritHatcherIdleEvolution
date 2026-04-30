using System;
using System.Collections.Generic;
using UnityEngine;
using SpiritHatchers.Data;

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

        [Header("Scene")]
        public Sprite backgroundSprite;
        public Color sceneTintColor = new Color(0.21f, 0.31f, 0.22f, 1f);

        [Header("Battle")]
        [Min(1)] public int recommendedPower = 50;
        public List<ExpeditionEnemyData> enemies = new List<ExpeditionEnemyData>();

        [Header("Base Rewards")]
        [Min(0)] public int baseCoin;
        [Min(0)] public int baseFood;
        [Min(0)] public int baseCrystal;
    }

    [Serializable]
    public class ExpeditionEnemyData
    {
        public string enemyId;
        public string enemyName;
        public CreatureElement element;
        [Min(1)] public int maxHealth = 30;
        [Min(0)] public int attack = 5;
        [Min(0)] public int defense = 0;
        public Sprite sprite;
    }
}
