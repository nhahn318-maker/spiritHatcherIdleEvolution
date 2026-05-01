using System;
using UnityEngine;

namespace SpiritHatchers.Data
{
    [Serializable]
    public class CreatureFormData
    {
        [Min(0)] public int formIndex;
        public string formName;
        public Sprite sprite;
        public Sprite[] idleFrames;
        public Sprite[] attackFrames;
        [Min(0.1f)] public float displayScale = 1f;
        public Vector2 battleVisualOffset;

        [Header("Evolution Requirements")]
        [Min(1)] public int requiredLevel;
        [Min(0)] public int requiredFood;
        [Min(0)] public int requiredCrystal;

        [Header("Power")]
        [Min(0f)] public float powerMultiplier = 1f;
    }
}
