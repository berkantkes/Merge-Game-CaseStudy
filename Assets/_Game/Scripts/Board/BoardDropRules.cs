using MergeGame.Items;
using UnityEngine;

namespace MergeGame.Board
{
    public class BoardDropRules : IBoardDropRules
    {
        private readonly ItemCatalogSO catalog;

        public BoardDropRules(ItemCatalogSO catalog)
        {
            this.catalog = catalog;
        }

        public DropResult Evaluate(BoardState state, Vector2Int source, Vector2Int? target)
        {
            bool sourceExists = state.Contains(source.y, source.x);
            if (!sourceExists || state.GetCell(source.y, source.x).IsEmpty)
            {
                return DropResult.EmptySource;
            }
            if (!target.HasValue)
            {
                return DropResult.OutsideBoard;
            }
            var destination = target.Value;
            bool targetExists = state.Contains(destination.y, destination.x);
            if (!targetExists)
            {
                return DropResult.OutsideBoard;
            }
            if (source == destination)
            {
                return DropResult.SameCell;
            }
            var targetItem = state.GetCell(destination.y, destination.x);
            if (targetItem.IsEmpty)
            {
                return DropResult.Move;
            }
            var sourceItem = state.GetCell(source.y, source.x);
            return EvaluateMerge(sourceItem, targetItem);
        }

        private DropResult EvaluateMerge(ItemStack source, ItemStack target)
        {
            if (source.Type != target.Type)
            {
                return DropResult.DifferentType;
            }
            if (source.Level != target.Level)
            {
                return DropResult.DifferentLevel;
            }
            int maxLevel = catalog.GetMaxLevel(source.Type);
            if (source.Level >= maxLevel)
            {
                return DropResult.MaximumLevel;
            }
            return DropResult.Merge;
        }
    }
}
