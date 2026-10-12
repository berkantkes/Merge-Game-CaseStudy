using UnityEngine;

namespace MergeGame.LevelEditor
{
    [RequireComponent(typeof(RectTransform))]
    public class LevelEditorLayout : MonoBehaviour
    {
        [SerializeField] private RectTransform content;
        private void Start() => Fit();
        private void OnRectTransformDimensionsChange() => Fit();
        private void Fit()
        {
            if (content == null)
            {
                return;
            }
            var area = ((RectTransform)transform).rect.size;
            float scale = Mathf.Max(0, Mathf.Min(area.x / 1080, area.y / 1920));
            content.localScale = new Vector3(scale, scale, 1);
        }
    }
}
