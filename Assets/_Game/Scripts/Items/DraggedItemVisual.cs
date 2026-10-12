using System;
using DG.Tweening;
using MergeGame.Pooling;
using UnityEngine;
using UnityEngine.UI;

namespace MergeGame.Items
{
    public class DraggedItemVisual : PoolObject
    {
        [SerializeField] private RectTransform rect;
        [SerializeField] private Image icon;
        [SerializeField] private float settleDuration = 0.15f;

        private RectTransform layer;
        private Tween tween;

        public void Show(Sprite sprite, RectTransform source, RectTransform dragLayer)
        {
            layer = dragLayer;
            icon.sprite = sprite;
            rect.sizeDelta = new Vector2(
                source.rect.width * source.lossyScale.x / layer.lossyScale.x,
                source.rect.height * source.lossyScale.y / layer.lossyScale.y);
            Follow(source.position);
        }

        public void Follow(Vector3 worldPosition)
        {
            rect.localPosition = layer.InverseTransformPoint(worldPosition);
        }

        public void MoveToCell(RectTransform destination, Action completed)
        {
            var endPosition = layer.InverseTransformPoint(destination.position);
            tween = rect.DOLocalMove(endPosition, settleDuration).SetEase(Ease.OutQuad);
            tween.OnComplete(() =>
            {
                tween = null;
                ReturnToPool();
                completed?.Invoke();
            });
        }

        public override void ResetState()
        {
            StopTween();
            icon.sprite = null;
            layer = null;
        }

        private void StopTween()
        {
            if (tween != null)
            {
                tween.Kill();
                tween = null;
            }
        }

        private void OnDisable()
        {
            StopTween();
        }
    }
}
