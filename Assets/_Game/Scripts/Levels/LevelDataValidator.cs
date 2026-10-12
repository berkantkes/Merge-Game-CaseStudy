using System.Collections.Generic;
using MergeGame.Items;
using UnityEngine;

namespace MergeGame.Levels
{
    public static class LevelDataValidator
    {
        public static bool TryValidate(LevelDataSO data, ItemCatalogSO catalog, out string error)
        {
            if (data == null)
            {
                error = "missing level data";
                return false;
            }
            if (!ValidateSize(data.Rows, data.Columns, data.RowSpacing, data.ColumnSpacing, out error))
            {
                return false;
            }
            if (catalog == null)
            {
                error = "missing catalog";
                return false;
            }
            if (!catalog.TryValidate(out error))
            {
                return false;
            }
            var occupied = new HashSet<Vector2Int>();
            foreach (var placement in data.Placements)
            {
                if (!Contains(data, placement.Row, placement.Column))
                {
                    error = "item outside board";
                    return false;
                }

                if (!occupied.Add(new Vector2Int(placement.Row, placement.Column)))
                {
                    error = "multiple items in cell";
                    return false;
                }

                if (placement.Item.IsEmpty || !catalog.TryGet(placement.Item.Type, placement.Item.Level, out _))
                {
                    error = "invalid item type or level";
                    return false;
                }
            }
            return true;
        }

        public static bool ValidateSize(int rows, int columns, float rowSpacing, float columnSpacing, out string error)
        {
            error = null;
            if (rows < 1 || columns < 1)
            {
                error = "invalid board size";
            }
            else if (float.IsNaN(rowSpacing) || float.IsInfinity(rowSpacing) || rowSpacing < 0 || float.IsNaN(columnSpacing) || float.IsInfinity(columnSpacing) || columnSpacing < 0)
            {
                error = "invalid spacing";
            }
            return error == null;
        }

        public static bool Contains(LevelDataSO data, int row, int column)
        {
            return row >= 0 && row < data.Rows && column >= 0 && column < data.Columns;
        }
    }
}
