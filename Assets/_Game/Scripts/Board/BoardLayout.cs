using UnityEngine;

namespace MergeGame.Board
{
    public class BoardLayout
    {
        private readonly int rows;
        private readonly int columns;
        private readonly Vector2 cellSize;
        private readonly Vector2 spacing;
        private readonly Vector2 padding;
        private readonly Vector2 size;

        public Vector2 Size
        {
            get
            {
                return size;
            }
        }

        public BoardLayout(int rows, int columns, Vector2 cellSize, Vector2 spacing, RectOffset padding)
        {
            this.rows = rows;
            this.columns = columns;
            this.cellSize = cellSize;
            this.spacing = spacing;
            this.padding = new Vector2(padding.left, padding.bottom);
            size = new Vector2(
                columns * cellSize.x + (columns - 1) * spacing.x + padding.horizontal,
                rows * cellSize.y + (rows - 1) * spacing.y + padding.vertical);
        }

        public Vector2Int? GetCell(Vector2 position)
        {
            if (position.x < 0 || position.y < 0 || position.x > size.x || position.y > size.y)
            {
                return null;
            }

            int column = Mathf.FloorToInt((position.x - padding.x + spacing.x * 0.5f) / (cellSize.x + spacing.x));
            int row = Mathf.FloorToInt((position.y - padding.y + spacing.y * 0.5f) / (cellSize.y + spacing.y));
            column = Mathf.Clamp(column, 0, columns - 1);
            row = Mathf.Clamp(row, 0, rows - 1);
            return new Vector2Int(column, row);
        }
    }
}
