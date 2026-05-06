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

        public CreatureSkillData GetActiveSkill(CreatureFormData form)
        {
            CreatureSkillData formSkill = GetLastSkill(GetUnlockedSkills(form));
            return formSkill ?? CreateLegacySkill(form);
        }

        public CreatureSkillData GetActiveOrbitSkill(CreatureFormData form)
        {
            List<CreatureSkillData> skills = GetUnlockedSkills(form);
            for (int i = skills.Count - 1; i >= 0; i--)
            {
                CreatureSkillData skill = skills[i];
                if (skill == null)
                {
                    continue;
                }

                bool hasOrbitEffect = skill.orbitEffectFrames != null && skill.orbitEffectFrames.Length > 0;
                bool hasOrbitHitEffect = skill.orbitHitEffectFrames != null && skill.orbitHitEffectFrames.Length > 0;
                bool hasRuntimeOrbitOrb = skill.orbitOrbFrames != null && skill.orbitOrbFrames.Length > 0 && skill.orbitOrbCount > 0;
                if (hasOrbitEffect || hasOrbitHitEffect || hasRuntimeOrbitOrb)
                {
                    return skill;
                }
            }

            CreatureSkillData legacySkill = CreateLegacySkill(form);
            bool legacyHasOrbit = legacySkill.orbitEffectFrames != null && legacySkill.orbitEffectFrames.Length > 0;
            bool legacyHasOrbitHit = legacySkill.orbitHitEffectFrames != null && legacySkill.orbitHitEffectFrames.Length > 0;
            return legacyHasOrbit || legacyHasOrbitHit ? legacySkill : null;
        }

        public string GetActiveSkillName(CreatureFormData form)
        {
            CreatureSkillData skill = GetActiveSkill(form);
            return skill != null && !string.IsNullOrEmpty(skill.skillName)
                ? skill.skillName
                : "Spirit Strike";
        }

        public List<CreatureSkillData> GetUnlockedSkills(CreatureFormData form)
        {
            List<CreatureSkillData> unlocked = new List<CreatureSkillData>();
            int formIndex = form != null ? form.formIndex : 0;

            if (forms != null)
            {
                for (int i = 0; i < forms.Count; i++)
                {
                    CreatureFormData candidate = forms[i];
                    if (candidate == null || candidate.formIndex > formIndex || candidate.unlockedSkills == null)
                    {
                        continue;
                    }

                    for (int skillIndex = 0; skillIndex < candidate.unlockedSkills.Count; skillIndex++)
                    {
                        CreatureSkillData skill = candidate.unlockedSkills[skillIndex];
                        if (skill != null)
                        {
                            unlocked.Add(skill);
                        }
                    }
                }
            }

            return unlocked;
        }

        public string GetUnlockedSkillNames(CreatureFormData form)
        {
            List<CreatureSkillData> skills = GetUnlockedSkills(form);
            if (skills.Count == 0)
            {
                return GetActiveSkillName(form);
            }

            List<string> names = new List<string>();
            for (int i = 0; i < skills.Count; i++)
            {
                CreatureSkillData skill = skills[i];
                if (skill != null && !string.IsNullOrEmpty(skill.skillName))
                {
                    names.Add(skill.skillName);
                }
            }

            return names.Count > 0 ? string.Join(", ", names) : "Spirit Strike";
        }

        private static CreatureSkillData GetLastSkill(List<CreatureSkillData> skills)
        {
            if (skills == null)
            {
                return null;
            }

            for (int i = skills.Count - 1; i >= 0; i--)
            {
                if (skills[i] != null)
                {
                    return skills[i];
                }
            }

            return null;
        }

        private CreatureSkillData CreateLegacySkill(CreatureFormData form)
        {
            return new CreatureSkillData
            {
                skillId = string.IsNullOrEmpty(creatureId) ? "legacy_skill" : $"{creatureId}_legacy_skill",
                skillName = string.IsNullOrEmpty(skillName) ? "Spirit Strike" : skillName,
                range = skillRange,
                powerMultiplier = Mathf.Max(0.1f, skillPowerMultiplier),
                attackFrames = ResolveFrames(form != null ? form.attackFrames : null, attackFrames),
                projectileFrames = projectileFrames,
                impactFrames = impactFrames,
                orbitEffectFrames = form != null ? form.orbitEffectFrames : null,
                orbitHitEffectFrames = form != null ? form.orbitHitEffectFrames : null,
                orbitEffectScaleMultiplier = 1f,
                orbitHitEffectSize = 180f,
                orbitOrbCount = 0
            };
        }

        private static Sprite[] ResolveFrames(Sprite[] primary, Sprite[] fallback)
        {
            return primary != null && primary.Length > 0 ? primary : fallback;
        }
    }
}
