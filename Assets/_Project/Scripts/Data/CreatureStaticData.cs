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
