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
                error = "Catalog is empty.";
            }
            var types = new HashSet<ItemType>();
            foreach (var chain in chains)
            {
                if (chain == null)
                {
                    error = "Item chain is missing.";
                    break;
                }

                if (chain.Type == ItemType.None || !Enum.IsDefined(typeof(ItemType), chain.Type))
                {
                    error = "Item type is invalid.";
                    break;
                }

                if (!types.Add(chain.Type))
                {
                    error = "Item type is duplicated.";
                    break;
                }

                if (chain.MaxLevel == 0)
                {
                    error = "Item chain is empty.";
                    break;
                }

                foreach (var level in chain.Levels)
                {
                    if (level == null || string.IsNullOrWhiteSpace(level.DisplayName))
                    {
                        error = "Item name is missing.";
                        break;
                    }

                    if (level.Sprite == null)
                    {
                        error = "Item sprite is missing.";
                        break;
                    }
                }

                if (error != null)
                {
                    break;
                }
            }
            return error == null;
        }
    }
}
