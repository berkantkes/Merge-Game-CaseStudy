using MergeGame.Events;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MergeGame.Board
{
    public class BoardInput : MonoBehaviour, IPointerDownHandler, IPointerUpHandler,
        IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private RectTransform surface;
        [SerializeField] private Image raycastSurface;

        private BoardLayout layout;
        private int? activePointer;
        private bool inputEnabled;
        private bool dragging;
        private bool interactionLocked;

        public void SetInteractionLocked(bool locked)
        {
            interactionLocked = locked;
        }

        public void Initialize(BoardLayout layout)
        {
            this.layout = layout;
            activePointer = null;
        }

        public void SetInputEnabled(bool enabled)
        {
            if (!enabled)
            {
                CancelInteraction();
            }
            inputEnabled = enabled;
            raycastSurface.raycastTarget = enabled;
            activePointer = null;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!inputEnabled || layout == null || activePointer.HasValue || interactionLocked)
            {
                return;
            }
            var pointer = ReadPointer(eventData);
            if (!pointer.HasValue || !pointer.Value.Cell.HasValue)
            {
                return;
            }
            var cell = pointer.Value.Cell.Value;
            activePointer = eventData.pointerId;
            EventManager.CellPressed?.Invoke(cell.y, cell.x);
        }

        private BoardPointer? ReadPointer(PointerEventData eventData)
        {
            bool converted = RectTransformUtility.ScreenPointToLocalPointInRectangle(
                surface, eventData.position, eventData.pressEventCamera, out var localPoint);

            if (!converted)
            {
                return null;
            }

            var position = new Vector2(localPoint.x - surface.rect.xMin, localPoint.y - surface.rect.yMin);
            var cell = layout.GetCell(position);
            var worldPosition = surface.TransformPoint(localPoint);
            return new BoardPointer(cell, worldPosition);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (activePointer == eventData.pointerId && !dragging)
            {
                activePointer = null;
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!inputEnabled || activePointer != eventData.pointerId)
            {
                return;
            }
            var pointer = ReadPointer(eventData);
            if (!pointer.HasValue)
            {
                return;
            }
            dragging = true;
            EventManager.BoardDragStarted?.Invoke(pointer.Value);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!dragging || activePointer != eventData.pointerId)
            {
                return;
            }
            var pointer = ReadPointer(eventData);
            if (pointer.HasValue)
            {
                EventManager.BoardDragMoved?.Invoke(pointer.Value);
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!dragging || activePointer != eventData.pointerId)
            {
                return;
            }
            var pointer = ReadPointer(eventData);
            dragging = false;
            activePointer = null;
            if (pointer.HasValue)
            {
                EventManager.BoardDragEnded?.Invoke(pointer.Value);
            }
            else
            {
                EventManager.BoardInteractionCancelled?.Invoke();
            }
        }

        private void CancelInteraction()
        {
            activePointer = null;
            dragging = false;
            EventManager.BoardInteractionCancelled?.Invoke();
        }

        private void OnDisable()
        {
            CancelInteraction();
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus)
            {
                CancelInteraction();
            }
        }

        private void OnApplicationPause(bool paused)
        {
            if (paused)
            {
                CancelInteraction();
            }
        }

    }
}
