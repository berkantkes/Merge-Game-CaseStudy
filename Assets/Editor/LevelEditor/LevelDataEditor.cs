using MergeGame.Items;
using MergeGame.Levels;

namespace MergeGame.EditorTools
{
    public static class LevelDataEditor
    {
        public static bool TryResize(LevelDataSO data, int newRows, int newColumns, float newRowSpacing, float newColumnSpacing, out string error)
        {
            if (!LevelDataValidator.ValidateSize(newRows, newColumns, newRowSpacing, newColumnSpacing, out error))
            {
                return false;
            }
            foreach (var placement in data.Placements)
            {
                if (placement.Row >= newRows || placement.Column >= newColumns)
                {
                    error = "Resize would remove items.";
                    return false;
                }
            }
            data.Rows = newRows;
            data.Columns = newColumns;
            data.RowSpacing = newRowSpacing;
            data.ColumnSpacing = newColumnSpacing;
            return true;
        }

        public static bool SetCell(LevelDataSO data, int row, int column, ItemStack item, ItemCatalogSO catalog, out string error)
        {
            error = null;
            if (!LevelDataValidator.Contains(data, row, column))
            {
                error = "Cell is outside the board.";
                return false;
            }
            if (!item.IsEmpty && (catalog == null || !catalog.TryGet(item.Type, item.Level, out _)))
            {
                error = "Item type or level is invalid.";
                return false;
            }
            data.Placements.RemoveAll(p => p.Row == row && p.Column == column);
            if (!item.IsEmpty)
            {
                data.Placements.Add(new CellPlacement(row, column, item));
            }
            return true;
        }

        public static void ClearPlacements(LevelDataSO data)
        {
            data.Placements.Clear();
        }

        public static void CopyFrom(LevelDataSO target, LevelDataSO source)
        {
            if (target == source)
            {
                return;
            }
            target.Rows = source.Rows;
            target.Columns = source.Columns;
            target.RowSpacing = source.RowSpacing;
            target.ColumnSpacing = source.ColumnSpacing;
            target.Placements.Clear();
            target.Placements.AddRange(source.Placements);
        }
    }
}
