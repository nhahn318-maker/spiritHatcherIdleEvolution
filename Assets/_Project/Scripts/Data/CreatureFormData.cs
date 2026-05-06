using System;
using System.Collections.Generic;
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

        [Header("Skill Unlocks")]
        public List<CreatureSkillData> unlockedSkills = new List<CreatureSkillData>();

        [Header("Ultimate Skill")]
        public CreatureSkillData ultimateSkill;
        [Min(1f)] public float ultimateRageRequired = 100f;
        [Min(1f)] public float ultimateImpactEffectSize = 360f;

        [Header("Legacy Form Skill Visual Overrides")]
        public Sprite[] attackFrames;
        public Sprite[] orbitEffectFrames;
        public Sprite[] orbitHitEffectFrames;
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
