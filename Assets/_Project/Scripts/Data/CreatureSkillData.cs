using System;
using UnityEngine;

namespace SpiritHatchers.Data
{
    [Serializable]
    public class CreatureSkillData
    {
        public string skillId;
        public string skillName = "Spirit Strike";
        public CreatureSkillRange range = CreatureSkillRange.Melee;
        [Min(0.1f)] public float powerMultiplier = 1f;

        [Header("Skill Visuals")]
        public Sprite[] attackFrames;
        public Sprite[] projectileFrames;
        public Sprite[] impactFrames;
        public Sprite[] orbitEffectFrames;
        public Sprite[] orbitHitEffectFrames;
        [Min(0.1f)] public float orbitEffectScaleMultiplier = 1f;
        [Min(1f)] public float orbitHitEffectSize = 180f;

        [Header("Runtime Orbit Orbs")]
        public Sprite[] orbitOrbFrames;
        [Min(0)] public int orbitOrbCount = 0;
        [Min(1f)] public float orbitOrbSize = 34f;
        [Min(1f)] public float orbitOrbRadius = 44f;
        [Min(1f)] public float orbitOrbSpeedDegrees = 180f;
        [Min(1f)] public float orbitOrbHitRadius = 30f;
        [Min(0.05f)] public float orbitOrbHitCooldown = 0.55f;
        [Min(0f)] public float orbitOrbDamageMultiplier = 0f;
    }
}
