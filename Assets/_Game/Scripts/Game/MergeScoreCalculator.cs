using MergeGame.Items;
using UnityEngine;

namespace MergeGame.Game
{
    public class MergeScoreCalculator : IMergeScoreCalculator
    {
        private readonly ItemCatalogSO catalog;

        public MergeScoreCalculator(ItemCatalogSO catalog)
        {
            this.catalog = catalog;
        }

        public int Calculate(ItemStack mergedItem)
        {
            bool found = catalog.TryGet(mergedItem.Type, mergedItem.Level, out var definition);
            if (!found)
            {
                Debug.LogError("missing merge item");
                return 0;
            }
            return definition.MergeScore;
        }
    }
}
