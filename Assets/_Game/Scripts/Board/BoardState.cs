using System.Collections.Generic;
using MergeGame.Items;
using MergeGame.Levels;
using UnityEngine;

namespace MergeGame.Board
{
    public class BoardState
    {
        private readonly ItemStack[,] cells;
        private readonly IBoardDropRules dropRules;
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

        private BoardState(int rows, int columns, IBoardDropRules dropRules)
        {
            cells = new ItemStack[rows, columns];
            this.dropRules = dropRules;
        }

        public static BoardState Create(LevelDataSO data, ItemCatalogSO catalog, IBoardDropRules dropRules)
        {
            if (!LevelDataValidator.TryValidate(data, catalog, out var error))
            {
                Debug.LogError(error);
                return null;
            }
            var state = new BoardState(data.Rows, data.Columns, dropRules);
            foreach (var placement in data.Placements)
            {
                state.cells[placement.Row, placement.Column] = placement.Item;
            }
            return state;
        }

        public List<Vector2Int> GetEmptyCells()
        {
            var emptyCells = new List<Vector2Int>();
            for (int row = 0; row < Rows; row++)
            {
                for (int column = 0; column < Columns; column++)
                {
                    if (cells[row, column].IsEmpty)
                    {
                        emptyCells.Add(new Vector2Int(column, row));
                    }
                }
            }
            return emptyCells;
        }

        public bool HasEmptyCell()
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int column = 0; column < Columns; column++)
                {
                    if (cells[row, column].IsEmpty)
                    {
                        return true;
                    }
                }
            }
            return false;
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

        public DropResult EvaluateDrop(Vector2Int source, Vector2Int? target)
        {
            return dropRules.Evaluate(this, source, target);
        }

        public DropResult TryDrop(Vector2Int source, Vector2Int? target)
        {
            var result = EvaluateDrop(source, target);
            if (result == DropResult.Move)
            {
                MoveItem(source, target.Value);
            }
            else if (result == DropResult.Merge)
            {
                MergeItems(source, target.Value);
            }
            return result;
        }

        private void MoveItem(Vector2Int source, Vector2Int target)
        {
            cells[target.y, target.x] = cells[source.y, source.x];
            cells[source.y, source.x] = default;
        }

        private void MergeItems(Vector2Int source, Vector2Int target)
        {
            var item = cells[source.y, source.x];
            cells[target.y, target.x] = new ItemStack(item.Type, item.Level + 1);
            cells[source.y, source.x] = default;
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
