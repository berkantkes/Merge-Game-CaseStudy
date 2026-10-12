using System;
using MergeGame.Pooling;
using UnityEngine;

namespace MergeGame.Items
{
    public class ItemDragView : MonoBehaviour
    {
        [SerializeField] private RectTransform dragLayer;

        private PoolFactory poolFactory;
        private DraggedItemVisual draggedItem;

        public void Initialize(PoolFactory factory)
        {
            poolFactory = factory;
        }

        public void Show(Sprite sprite, RectTransform source)
        {
            Clear();
            draggedItem = poolFactory.Get<DraggedItemVisual>(PoolType.DraggedItemVisual, dragLayer);
            draggedItem.Show(sprite, source, dragLayer);
        }

        public void Follow(Vector3 worldPosition)
        {
            draggedItem.Follow(worldPosition);
        }

        public void MoveToCell(RectTransform target, Action completed)
        {
            draggedItem.MoveToCell(target, () =>
            {
                draggedItem = null;
                completed?.Invoke();
            });
        }

        public void Clear()
        {
            if (draggedItem != null)
            {
                draggedItem.ReturnToPool();
                draggedItem = null;
            }
        }

        private void OnDisable()
        {
            Clear();
        }
    }
}
