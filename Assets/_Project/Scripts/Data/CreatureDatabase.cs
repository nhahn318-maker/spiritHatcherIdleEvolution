using System.Collections.Generic;
using UnityEngine;

namespace SpiritHatchers.Data
{
    [CreateAssetMenu(
        fileName = "CreatureDatabase",
        menuName = "Spirit Hatchers/Creatures/Creature Database")]
    public class CreatureDatabase : ScriptableObject
    {
        [SerializeField] private List<CreatureStaticData> creatures = new List<CreatureStaticData>();

        public CreatureStaticData GetCreatureById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                Debug.LogWarning("CreatureDatabase.GetCreatureById was called with an empty id.");
                return null;
            }

            for (int i = 0; i < creatures.Count; i++)
            {
                CreatureStaticData creature = creatures[i];

                if (creature != null && creature.creatureId == id)
                {
                    return creature;
                }
            }

            Debug.LogWarning($"Creature with id '{id}' was not found in the database.");
            return null;
        }

        public List<CreatureStaticData> GetCreaturesByRarity(CreatureRarity rarity)
        {
            List<CreatureStaticData> matchingCreatures = new List<CreatureStaticData>();

            for (int i = 0; i < creatures.Count; i++)
            {
                CreatureStaticData creature = creatures[i];

                if (creature != null && creature.rarity == rarity)
                {
                    matchingCreatures.Add(creature);
                }
            }

            return matchingCreatures;
        }

        public List<CreatureStaticData> GetAllCreatures()
        {
            return new List<CreatureStaticData>(creatures);
        }
    }
}
