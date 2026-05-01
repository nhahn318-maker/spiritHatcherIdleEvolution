using System.Collections.Generic;
using UnityEngine;

namespace SpiritHatchers.Data
{
    [CreateAssetMenu(
        fileName = "CreatureStaticData",
        menuName = "Spirit Hatchers/Creatures/Creature Static Data")]
    public class CreatureStaticData : ScriptableObject
    {
        [Header("Identity")]
        public string creatureId;
        public string creatureName;

        [Header("Classification")]
        public CreatureElement element;
        public CreatureRarity rarity;

        [Header("Stats")]
        [Min(1)] public int basePower = 10;
        [Min(1)] public int speed = 50;

        [Header("Default Skill")]
        public string skillName = "Spirit Strike";
        public CreatureSkillRange skillRange = CreatureSkillRange.Melee;
        [Min(0.1f)] public float skillPowerMultiplier = 1f;

        [Header("Animation Facing")]
        public bool idleFramesFaceRight = true;
        public bool attackFramesFaceRight = true;

        [Header("Skill Visuals")]
        public Sprite[] attackFrames;
        public Sprite[] projectileFrames;
        public Sprite[] impactFrames;

        [Header("Evolution Forms")]
        public List<CreatureFormData> forms = new List<CreatureFormData>();

        public CreatureFormData GetFormByIndex(int formIndex)
        {
            for (int i = 0; i < forms.Count; i++)
            {
                CreatureFormData form = forms[i];

                if (form != null && form.formIndex == formIndex)
                {
                    return form;
                }
            }

            return null;
        }
    }
}
