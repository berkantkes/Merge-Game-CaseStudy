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
                error = "Level data is missing.";
                return false;
            }
            if (!ValidateSize(data.Rows, data.Columns, data.RowSpacing, data.ColumnSpacing, out error))
            {
                return false;
            }
            if (catalog == null)
            {
                error = "Item catalog is missing.";
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
                    error = "Item is outside the board.";
                    return false;
                }

                if (!occupied.Add(new Vector2Int(placement.Row, placement.Column)))
                {
                    error = "Cell has multiple items.";
                    return false;
                }

                if (placement.Item.IsEmpty || !catalog.TryGet(placement.Item.Type, placement.Item.Level, out _))
                {
                    error = "Item type or level is invalid.";
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
                error = "Rows and columns must be at least 1.";
            }
            else if (float.IsNaN(rowSpacing) || float.IsInfinity(rowSpacing) || rowSpacing < 0 || float.IsNaN(columnSpacing) || float.IsInfinity(columnSpacing) || columnSpacing < 0)
            {
                error = "Spacing must be a finite positive value or zero.";
            }
            return error == null;
        }

        public static bool Contains(LevelDataSO data, int row, int column)
        {
            return row >= 0 && row < data.Rows && column >= 0 && column < data.Columns;
        }
    }
}
