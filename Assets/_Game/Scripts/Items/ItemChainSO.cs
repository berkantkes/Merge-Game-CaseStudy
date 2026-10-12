using System.Collections.Generic;
using UnityEngine;

namespace MergeGame.Items
{
    [CreateAssetMenu(fileName = "Item Chain", menuName = "Merge Game/Item Chain")]
    public class ItemChainSO : ScriptableObject
    {
        [SerializeField] private ItemType itemType;
        [SerializeField] private List<ItemLevelDefinition> levels = new List<ItemLevelDefinition>();
        public ItemType Type => itemType;
        public int MaxLevel => levels.Count;
        public IReadOnlyList<ItemLevelDefinition> Levels => levels;

        public bool TryGetLevel(int level, out ItemLevelDefinition definition)
        {
            definition = level >= 1 && level <= levels.Count ? levels[level - 1] : null;
            return definition != null;
        }
    }
}
