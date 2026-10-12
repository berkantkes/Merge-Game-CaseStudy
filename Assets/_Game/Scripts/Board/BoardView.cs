using MergeGame.Items;
using MergeGame.Levels;
using MergeGame.Pooling;
using UnityEngine;
using UnityEngine.UI;

namespace MergeGame.Board
{
    public class BoardView : MonoBehaviour
    {
        [SerializeField] private RectTransform boardArea;
        [SerializeField] private RectTransform grid;
        [SerializeField] private BoardInput boardInput;
        [SerializeField] private GridLayoutGroup gridLayout;
        [SerializeField] private Vector2 maximumSize = new Vector2(960, 960);

        private PoolFactory poolFactory;
        private CellVisual[,] cells;
        private BoardState state;
        private ItemCatalogSO catalog;
        private bool inputEnabled;

        public void Initialize(PoolFactory factory)
        {
            poolFactory = factory;
        }

        public bool Build(LevelDataSO levelData, BoardState boardState, ItemCatalogSO itemCatalog)
        {
            if (grid == null || poolFactory == null)
            {
                Debug.LogError("Board references are missing.");
                return false;
            }

            Clear();
            SetData(boardState, itemCatalog);
            BoardLayout layout = ConfigureLayout(levelData);
            InitializeInput(layout);
            CreateCells();
            Fit();
            return true;
        }

        private void SetData(BoardState boardState, ItemCatalogSO itemCatalog)
        {
            state = boardState;
            catalog = itemCatalog;
        }

        private void InitializeInput(BoardLayout layout)
        {
            boardInput.Initialize(layout);
            boardInput.SetInputEnabled(inputEnabled);
        }

        private void CreateCells()
        {
            cells = new CellVisual[state.Rows, state.Columns];
            for (int row = 0; row < state.Rows; row++)
            {
                for (int column = 0; column < state.Columns; column++)
                {
                    CreateCell(row, column);
                }
            }
        }

        private void CreateCell(int row, int column)
        {
            var cell = poolFactory.Get<CellVisual>(PoolType.Cell, grid);
            cell.name = $"Cell {row}, {column}";
            cell.Initialize(row, column);
            cells[row, column] = cell;
            RefreshCell(row, column);
        }

        public void RefreshCell(int row, int column)
        {
            if (state == null || cells == null || !state.Contains(row, column))
            {
                return;
            }
            var item = state.GetCell(row, column);
            catalog.TryGet(item.Type, item.Level, out var definition);
            cells[row, column].ShowItem(definition, item.Level);
        }

        public void SetInputEnabled(bool enabled)
        {
            inputEnabled = enabled;
            if (boardInput != null)
            {
                boardInput.SetInputEnabled(enabled);
            }
        }

        private BoardLayout ConfigureLayout(LevelDataSO levelData)
        {
            gridLayout.constraintCount = levelData.Columns;
            gridLayout.spacing = new Vector2(levelData.ColumnSpacing, levelData.RowSpacing);
            var boardLayout = new BoardLayout(levelData.Rows, levelData.Columns, gridLayout.cellSize, gridLayout.spacing, gridLayout.padding);
            grid.sizeDelta = boardLayout.Size;
            return boardLayout;
        }

        private void Fit()
        {
            if (grid == null)
            {
                return;
            }
            var area = boardArea.rect.size;
            float width = Mathf.Min(maximumSize.x, area.x);
            float height = Mathf.Min(maximumSize.y, area.y);
            float scale = Mathf.Max(0, Mathf.Min(width / grid.sizeDelta.x, height / grid.sizeDelta.y));
            grid.localScale = new Vector3(scale, scale, 1);
        }

        public void Clear()
        {
            if (boardInput != null)
            {
                boardInput.SetInputEnabled(false);
            }
            if (cells == null)
            {
                return;
            }
            foreach (var cell in cells)
            {
                if (cell != null)
                {
                    cell.ReturnToPool();
                }
            }
            cells = null;
        }

        private void OnDestroy()
        {
            Clear();
        }

    }
}
