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
        [SerializeField] private RectTransform gridRect;
        [SerializeField] private BoardInput boardInput;
        [SerializeField] private GridLayoutGroup gridLayout;
        [SerializeField] private Vector2 maxBoardSize = new Vector2(960, 960);

        private PoolFactory poolFactory;
        private CellVisual[,] cellVisuals;
        private BoardState boardState;
        private ItemCatalogSO itemCatalog;
        private bool isInputEnabled;
        private CellVisual selectedCellVisual;
        private CellVisual dropTargetVisual;

        public void Initialize(PoolFactory factory)
        {
            poolFactory = factory;
        }

        public bool Build(LevelDataSO levelData, BoardState boardState, ItemCatalogSO itemCatalog)
        {
            if (gridRect == null || poolFactory == null)
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
            this.boardState = boardState;
            this.itemCatalog = itemCatalog;
        }

        private void InitializeInput(BoardLayout layout)
        {
            boardInput.Initialize(layout);
            boardInput.SetInputEnabled(isInputEnabled);
        }

        private void CreateCells()
        {
            cellVisuals = new CellVisual[boardState.Rows, boardState.Columns];
            for (int row = 0; row < boardState.Rows; row++)
            {
                for (int column = 0; column < boardState.Columns; column++)
                {
                    CreateCell(row, column);
                }
            }
        }

        private void CreateCell(int row, int column)
        {
            var cell = poolFactory.Get<CellVisual>(PoolType.Cell, gridRect);
            cell.name = $"Cell {row}, {column}";
            cell.Initialize(row, column);
            cellVisuals[row, column] = cell;
            RefreshCell(row, column);
        }

        public void SelectCell(Vector2Int? position)
        {
            if (selectedCellVisual != null)
            {
                selectedCellVisual.SetSelected(false);
                selectedCellVisual = null;
            }
            if (position.HasValue && cellVisuals != null)
            {
                selectedCellVisual = cellVisuals[position.Value.y, position.Value.x];
                selectedCellVisual.SetSelected(true);
            }
        }

        public void ShowDropTarget(Vector2Int? position, DropResult result)
        {
            ClearDropTarget();
            if (position.HasValue && cellVisuals != null)
            {
                dropTargetVisual = cellVisuals[position.Value.y, position.Value.x];
                dropTargetVisual.ShowTarget(result);
            }
        }

        public void ClearDropTarget()
        {
            if (dropTargetVisual != null)
            {
                dropTargetVisual.ClearTarget();
                dropTargetVisual = null;
            }
        }

        public void SetItemVisible(Vector2Int position, bool visible)
        {
            cellVisuals[position.y, position.x].SetItemVisible(visible);
        }

        public RectTransform GetItemTransform(int row, int column)
        {
            return cellVisuals[row, column].ItemTransform;
        }

        public void RefreshCell(int row, int column)
        {
            if (boardState == null || cellVisuals == null || !boardState.Contains(row, column))
            {
                return;
            }
            var item = boardState.GetCell(row, column);
            itemCatalog.TryGet(item.Type, item.Level, out var definition);
            cellVisuals[row, column].ShowItem(definition);
        }

        public void PlayItemArrival(int row, int column)
        {
            RefreshCell(row, column);
            cellVisuals[row, column].PlayBump();
        }

        public void PlayItemClick(int row, int column)
        {
            cellVisuals[row, column].PlayClickBump();
        }

        public void SetInputEnabled(bool enabled)
        {
            isInputEnabled = enabled;
            if (boardInput != null)
            {
                boardInput.SetInputEnabled(enabled);
            }
        }

        public void SetInteractionLocked(bool locked)
        {
            boardInput.SetInteractionLocked(locked);
        }

        private BoardLayout ConfigureLayout(LevelDataSO levelData)
        {
            gridLayout.constraintCount = levelData.Columns;
            gridLayout.spacing = new Vector2(levelData.ColumnSpacing, levelData.RowSpacing);
            var boardLayout = new BoardLayout(levelData.Rows, levelData.Columns, gridLayout.cellSize, gridLayout.spacing, gridLayout.padding);
            gridRect.sizeDelta = boardLayout.Size;
            return boardLayout;
        }

        private void Fit()
        {
            if (gridRect == null)
            {
                return;
            }
            var area = boardArea.rect.size;
            float width = Mathf.Min(maxBoardSize.x, area.x);
            float height = Mathf.Min(maxBoardSize.y, area.y);
            float scale = Mathf.Max(0, Mathf.Min(width / gridRect.sizeDelta.x, height / gridRect.sizeDelta.y));
            gridRect.localScale = new Vector3(scale, scale, 1);
        }

        public void Clear()
        {
            SelectCell(null);
            ClearDropTarget();
            if (boardInput != null)
            {
                boardInput.SetInputEnabled(false);
            }
            if (cellVisuals == null)
            {
                return;
            }
            foreach (var cell in cellVisuals)
            {
                if (cell != null)
                {
                    cell.ReturnToPool();
                }
            }
            cellVisuals = null;
        }

        private void OnDestroy()
        {
            Clear();
        }

    }
}
