using MergeGame.Board;
using MergeGame.Events;
using MergeGame.Game;
using MergeGame.Pooling;
using UnityEngine;

namespace MergeGame.Items
{
    public class ItemDragController : MonoBehaviour
    {
        [SerializeField] private BoardView boardView;
        [SerializeField] private ItemDragView dragView;

        private BoardState boardState;
        private BoardActions boardActions;
        private ItemCatalogSO itemCatalog;
        private DragState dragState;
        private Vector2Int? selectedCellPosition;
        private Vector2Int startCellPosition;
        private Vector2Int endCellPosition;
        private DropResult dropResult;

        public void Initialize(BoardActions actions, ItemCatalogSO itemCatalog, PoolFactory factory)
        {
            boardActions = actions;
            boardState = boardActions.Board;
            this.itemCatalog = itemCatalog;
            dragView.Initialize(factory);
        }

        private void OnEnable()
        {
            EventManager.CellPressed += SelectCell;
            EventManager.BoardDragStarted += BeginDrag;
            EventManager.BoardDragMoved += MoveDrag;
            EventManager.BoardDragEnded += EndDrag;
            EventManager.BoardInteractionCancelled += Cancel;
        }

        private void OnDisable()
        {
            EventManager.CellPressed -= SelectCell;
            EventManager.BoardDragStarted -= BeginDrag;
            EventManager.BoardDragMoved -= MoveDrag;
            EventManager.BoardDragEnded -= EndDrag;
            EventManager.BoardInteractionCancelled -= Cancel;
            Cancel();
        }

        private void SelectCell(int row, int column)
        {
            if (boardState == null || dragState != DragState.Idle)
            {
                return;
            }
            selectedCellPosition = new Vector2Int(column, row);
            boardView.SelectCell(selectedCellPosition);
            var item = boardState.GetCell(row, column);
            if (!item.IsEmpty)
            {
                boardView.PlayItemClick(row, column);
            }
        }

        private void BeginDrag(BoardPointer pointer)
        {
            if (boardState == null || dragState != DragState.Idle || !selectedCellPosition.HasValue)
            {
                return;
            }
            startCellPosition = selectedCellPosition.Value;
            endCellPosition = startCellPosition;
            var item = boardState.GetCell(startCellPosition.y, startCellPosition.x);
            if (item.IsEmpty)
            {
                return;
            }
            bool found = itemCatalog.TryGet(item.Type, item.Level, out var definition);
            if (!found)
            {
                return;
            }
            StartDrag(definition);
            MoveDrag(pointer);
        }

        private void StartDrag(ItemLevelDefinition definition)
        {
            dragState = DragState.Dragging;
            EventManager.BoardInteractionBusyChanged?.Invoke(true);
            boardView.SetItemVisible(startCellPosition, false);
            var startItemRect = boardView.GetItemTransform(startCellPosition.y, startCellPosition.x);
            dragView.Show(definition.Sprite, startItemRect);
        }

        private void MoveDrag(BoardPointer pointer)
        {
            if (dragState != DragState.Dragging)
            {
                return;
            }
            dragView.Follow(pointer.WorldPosition);
            var previewResult = boardState.EvaluateDrop(startCellPosition, pointer.Cell);
            boardView.ShowDropTarget(pointer.Cell, previewResult);
        }

        private void EndDrag(BoardPointer pointer)
        {
            if (dragState != DragState.Dragging)
            {
                return;
            }
            dragState = DragState.Resolving;
            dropResult = boardActions.TryDrop(startCellPosition, pointer.Cell);
            if (dropResult == DropResult.Merge)
            {
                EventManager.ScoreChanged?.Invoke(boardActions.Score);
            }
            endCellPosition = startCellPosition;
            if (dropResult == DropResult.Move || dropResult == DropResult.Merge)
            {
                endCellPosition = pointer.Cell.Value;
            }
            boardView.ShowDropTarget(pointer.Cell, dropResult);
            var endItemRect = boardView.GetItemTransform(endCellPosition.y, endCellPosition.x);
            dragView.MoveToCell(endItemRect, CompleteDrop);
        }

        private void CompleteDrop()
        {
            RefreshAffectedCells();
            boardView.ClearDropTarget();
            selectedCellPosition = endCellPosition;
            if (dropResult == DropResult.Move || dropResult == DropResult.Merge)
            {
                boardView.PlayItemArrival(endCellPosition.y, endCellPosition.x);
            }
            boardView.SelectCell(selectedCellPosition);
            dragState = DragState.Idle;
            EventManager.BoardInteractionBusyChanged?.Invoke(false);
        }

        private void RefreshAffectedCells()
        {
            boardView.RefreshCell(startCellPosition.y, startCellPosition.x);
            if (endCellPosition != startCellPosition)
            {
                boardView.RefreshCell(endCellPosition.y, endCellPosition.x);
            }
        }

        private void Cancel()
        {
            bool wasBusy = dragState != DragState.Idle;
            if (dragView != null)
            {
                dragView.Clear();
            }
            if (boardView != null)
            {
                if (wasBusy)
                {
                    RefreshAffectedCells();
                }
                boardView.SelectCell(null);
                boardView.ClearDropTarget();
            }
            selectedCellPosition = null;
            dragState = DragState.Idle;
            if (wasBusy)
            {
                EventManager.BoardInteractionBusyChanged?.Invoke(false);
            }
        }
    }
}
