using DG.Tweening;
using MergeGame.Items;
using MergeGame.Pooling;
using UnityEngine;
using UnityEngine.UI;

namespace MergeGame.Board
{
    public class CellVisual : PoolObject
    {
        [SerializeField] private GameObject alternateBackground;
        [SerializeField] private Image icon;
        [SerializeField] private GameObject maxLevelBadge;
        [SerializeField] private GameObject selection;
        [SerializeField] private GameObject validTarget;
        [SerializeField] private GameObject invalidTarget;
        [SerializeField] private float bumpScale = 1.12f;
        [SerializeField] private float bumpDuration = 0.2f;
        [SerializeField] private float clickBumpScale = 1.08f;
        [SerializeField] private float clickBumpDuration = 0.12f;

        private Tween bumpTween;

        public RectTransform ItemTransform
        {
            get
            {
                return icon.rectTransform;
            }
        }

        public void Initialize(int row, int column)
        {
            alternateBackground.SetActive((row + column) % 2 != 0);
        }

        public override void ResetState()
        {
            StopBump();
            SetSelected(false);
            ClearTarget();
            icon.sprite = null;
            icon.gameObject.SetActive(false);
            maxLevelBadge.SetActive(false);
            alternateBackground.SetActive(false);
        }

        public void SetSelected(bool selected)
        {
            selection.SetActive(selected);
        }

        public void ShowTarget(DropResult result)
        {
            bool valid = result == DropResult.Move || result == DropResult.Merge;
            bool invalid = result == DropResult.DifferentType || result == DropResult.DifferentLevel ||
                result == DropResult.MaximumLevel;
            validTarget.SetActive(valid);
            invalidTarget.SetActive(invalid);
        }

        public void ClearTarget()
        {
            validTarget.SetActive(false);
            invalidTarget.SetActive(false);
        }

        public void SetItemVisible(bool visible)
        {
            StopBump();
            icon.gameObject.SetActive(visible);
        }

        public void ShowItem(ItemLevelDefinition definition, bool isMaxLevel)
        {
            StopBump();
            bool visible = definition != null;
            icon.gameObject.SetActive(visible);
            maxLevelBadge.SetActive(visible && isMaxLevel);
            if (!visible)
            {
                return;
            }
            icon.sprite = definition.Sprite;
        }

        public void PlayBump()
        {
            PlayBump(bumpScale, bumpDuration);
        }

        public void PlayClickBump()
        {
            PlayBump(clickBumpScale, clickBumpDuration);
        }

        private void PlayBump(float scale, float duration)
        {
            StopBump();
            var sequence = DOTween.Sequence();
            sequence.Append(icon.rectTransform.DOScale(scale, duration * 0.4f).SetEase(Ease.OutQuad));
            sequence.Append(icon.rectTransform.DOScale(1f, duration * 0.6f).SetEase(Ease.InOutQuad));
            sequence.OnComplete(() =>
            {
                bumpTween = null;
            });
            bumpTween = sequence;
        }

        private void StopBump()
        {
            if (bumpTween != null)
            {
                bumpTween.Kill();
                bumpTween = null;
            }
            icon.rectTransform.localScale = Vector3.one;
        }

        private void OnDisable()
        {
            StopBump();
        }
    }
}
