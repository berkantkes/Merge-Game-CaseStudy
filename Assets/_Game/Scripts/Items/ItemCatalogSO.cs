using System;
using System.Collections.Generic;
using UnityEngine;

namespace MergeGame.Items
{
    [CreateAssetMenu(fileName = "Item Catalog", menuName = "Merge Game/Item Catalog")]
    public class ItemCatalogSO : ScriptableObject
    {
        [SerializeField] private List<ItemChainSO> chains = new List<ItemChainSO>();
        public IReadOnlyList<ItemChainSO> Chains => chains;

        public int GetMaxLevel(ItemType type)
        {
            foreach (var chain in chains)
            {
                if (chain != null && chain.Type == type)
                {
                    return chain.MaxLevel;
                }
            }
            return 0;
        }

        public bool TryGet(ItemType type, int level, out ItemLevelDefinition definition)
        {
            foreach (var chain in chains)
            {
                if (chain != null && chain.Type == type)
                {
                    return chain.TryGetLevel(level, out definition);
                }
            }
            definition = null;
            return false;
        }

        public bool TryValidate(out string error)
        {
            error = null;
            if (chains.Count == 0)
            {
                error = "empty catalog";
            }
            var types = new HashSet<ItemType>();
            foreach (var chain in chains)
            {
                if (chain == null)
                {
                    error = "missing chain";
                    break;
                }

                if (chain.Type == ItemType.None || !Enum.IsDefined(typeof(ItemType), chain.Type))
                {
                    error = "invalid item type";
                    break;
                }

                if (!types.Add(chain.Type))
                {
                    error = "duplicate item type";
                    break;
                }

                if (chain.MaxLevel == 0)
                {
                    error = "empty chain";
                    break;
                }

                foreach (var level in chain.Levels)
                {
                    if (level == null || string.IsNullOrWhiteSpace(level.DisplayName))
                    {
                        error = "missing item name";
                        break;
                    }

                    if (level.Sprite == null)
                    {
                        error = "missing item sprite";
                        break;
                    }
                }

                if (error != null)
                {
                    break;
                }
                bool validScores = ValidateMergeScores(chain);
                if (!validScores)
                {
                    error = "invalid merge scores";
                    break;
                }
            }
            return error == null;
        }

        private bool ValidateMergeScores(ItemChainSO chain)
        {
            if (chain.Levels[0].MergeScore != 0)
            {
                return false;
            }
            for (int index = 1; index < chain.Levels.Count; index++)
            {
                if (chain.Levels[index].MergeScore <= chain.Levels[index - 1].MergeScore)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
