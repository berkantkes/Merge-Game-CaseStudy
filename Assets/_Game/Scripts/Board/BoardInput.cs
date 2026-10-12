using MergeGame.Events;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MergeGame.Board
{
    public class BoardInput : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private RectTransform surface;
        [SerializeField] private Image raycastSurface;

        private BoardLayout layout;
        private int? activePointer;
        private bool inputEnabled;

        public void Initialize(BoardLayout layout)
        {
            this.layout = layout;
            activePointer = null;
        }

        public void SetInputEnabled(bool enabled)
        {
            inputEnabled = enabled;
            raycastSurface.raycastTarget = enabled;
            activePointer = null;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!inputEnabled || layout == null || activePointer.HasValue)
            {
                return;
            }
            bool converted = RectTransformUtility.ScreenPointToLocalPointInRectangle(
                surface, eventData.position, eventData.pressEventCamera, out var localPoint);

            if (!converted)
            {
                return;
            }

            var position = new Vector2(localPoint.x - surface.rect.xMin, localPoint.y - surface.rect.yMin);
            var cell = layout.GetCell(position);
            if (!cell.HasValue)
            {
                return;
            }
            activePointer = eventData.pointerId;
            EventManager.CellPressed?.Invoke(cell.Value.y, cell.Value.x);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (activePointer == eventData.pointerId)
            {
                activePointer = null;
            }
        }

        private void OnDisable()
        {
            activePointer = null;
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus)
            {
                activePointer = null;
            }
        }

    }
}
