using UnityEngine;

namespace MergeGame.Items
{
    public class RandomItemSpawnPolicy : IItemSpawnPolicy
    {
        private readonly ItemCatalogSO catalog;

        public RandomItemSpawnPolicy(ItemCatalogSO catalog)
        {
            this.catalog = catalog;
        }

        public ItemStack ChooseItem()
        {
            int chainIndex = Random.Range(0, catalog.Chains.Count);
            var chain = catalog.Chains[chainIndex];
            return new ItemStack(chain.Type, 1);
        }
    }
}
