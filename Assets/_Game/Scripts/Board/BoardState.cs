using MergeGame.Items;
using MergeGame.Levels;
using UnityEngine;

namespace MergeGame.Board
{
    public class BoardState
    {
        private readonly ItemStack[,] cells;
        public int Rows
        {
            get
            {
                return cells.GetLength(0);
            }
        }

        public int Columns
        {
            get
            {
                return cells.GetLength(1);
            }
        }

        private BoardState(int rows, int columns)
        {
            cells = new ItemStack[rows, columns];
        }

        public static BoardState Create(LevelDataSO data, ItemCatalogSO catalog)
        {
            if (!LevelDataValidator.TryValidate(data, catalog, out var error))
            {
                Debug.LogError(error);
                return null;
            }
            var state = new BoardState(data.Rows, data.Columns);
            foreach (var placement in data.Placements)
            {
                state.cells[placement.Row, placement.Column] = placement.Item;
            }
            return state;
        }

        public bool Contains(int row, int column)
        {
            if (row < 0 || row >= Rows)
            {
                return false;
            }
            if (column < 0 || column >= Columns)
            {
                return false;
            }
            return true;
        }

        public ItemStack GetCell(int row, int column)
        {
            if (!Contains(row, column))
            {
                return default;
            }
            return cells[row, column];
        }

        public bool SetCell(int row, int column, ItemStack item, ItemCatalogSO catalog)
        {
            if (!Contains(row, column))
            {
                return false;
            }
            if (!item.IsEmpty && (catalog == null || !catalog.TryGet(item.Type, item.Level, out _)))
            {
                return false;
            }
            cells[row, column] = item;
            return true;
        }
    }
}
