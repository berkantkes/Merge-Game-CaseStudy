using System;
using DG.Tweening;
using MergeGame.Pooling;
using UnityEngine;
using UnityEngine.UI;

namespace MergeGame.Items
{
    public class SpawnItemVisual : PoolObject
    {
        [SerializeField] private RectTransform rect;
        [SerializeField] private Image icon;
        [SerializeField, Min(1f)] private float speed = 5000f;
        [SerializeField] private float flightScale = 1.4f;

        private Tween tween;
        private RectTransform layer;
        private RectTransform target;
        private Vector3 startPosition;
        private Vector2 startSize;
        private Action completed;
        private float progress;

        public void Play(Sprite sprite, RectTransform source, RectTransform destination,
            RectTransform flightLayer, Action onCompleted)
        {
            layer = flightLayer;
            target = destination;
            completed = onCompleted;
            icon.sprite = sprite;
            startPosition = layer.InverseTransformPoint(source.position);
            startSize = rect.sizeDelta;
            progress = 0;
            Move(0);
            var endPosition = layer.InverseTransformPoint(target.position);
            float distance = Vector3.Distance(startPosition, endPosition);
            float duration = distance / speed;
            tween = DOTween.To(() => progress, Move, 1f, duration);
            tween.SetEase(Ease.Linear);
            tween.OnComplete(Complete);
        }

        private void Move(float value)
        {
            progress = value;
            var endPosition = layer.InverseTransformPoint(target.position);
            rect.localPosition = Vector3.Lerp(startPosition, endPosition, progress);

            var endSize = new Vector2(
                target.rect.width * target.lossyScale.x / layer.lossyScale.x,
                target.rect.height * target.lossyScale.y / layer.lossyScale.y);
            rect.sizeDelta = Vector2.Lerp(startSize, endSize, progress);
            float scaleProgress = 1f - Mathf.Abs(progress * 2f - 1f);
            float scale = Mathf.Lerp(1f, flightScale, Mathf.SmoothStep(0f, 1f, scaleProgress));
            rect.localScale = Vector3.one * scale;
        }

        private void Complete()
        {
            var callback = completed;
            tween = null;
            ReturnToPool();
            callback?.Invoke();
        }

        public override void ResetState()
        {
            StopTween();
            rect.sizeDelta = startSize;
            rect.localScale = Vector3.one;
            icon.sprite = null;
            completed = null;
            target = null;
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
