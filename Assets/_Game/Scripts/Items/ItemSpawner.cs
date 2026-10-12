using MergeGame.Board;
using UnityEngine;

namespace MergeGame.Items
{
    public class ItemSpawner
    {
        private readonly BoardState boardState;
        private readonly ItemCatalogSO catalog;
        private readonly IItemSpawnPolicy spawnPolicy;

        public ItemSpawner(BoardState boardState, ItemCatalogSO catalog, IItemSpawnPolicy spawnPolicy)
        {
            this.boardState = boardState;
            this.catalog = catalog;
            this.spawnPolicy = spawnPolicy;
        }

        public Vector2Int? TrySpawn()
        {
            var emptyCells = boardState.GetEmptyCells();
            if (emptyCells.Count == 0)
            {
                return null;
            }

            int cellIndex = Random.Range(0, emptyCells.Count);
            var position = emptyCells[cellIndex];
            var item = spawnPolicy.ChooseItem();

            bool placed = boardState.SetCell(position.y, position.x, item, catalog);
            if (!placed)
            {
                return null;
            }
            return position;
        }
    }
}
