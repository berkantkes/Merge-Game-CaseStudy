using UnityEngine;

namespace MergeGame.Board
{
    [RequireComponent(typeof(RectTransform))]
    public class SafeArea : MonoBehaviour
    {
        private bool isApplying;

        private void OnEnable()
        {
            Apply();
        }

        private void OnRectTransformDimensionsChange()
        {
            Apply();
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus)
            {
                Apply();
            }
        }

        private void Apply()
        {
            if (isApplying || Screen.width <= 0 || Screen.height <= 0)
            {
                return;
            }
            isApplying = true;
            var area = Screen.safeArea;
            var rect = (RectTransform)transform;
            rect.anchorMin = new Vector2(area.xMin / Screen.width, area.yMin / Screen.height);
            rect.anchorMax = new Vector2(area.xMax / Screen.width, area.yMax / Screen.height);
            rect.offsetMin = rect.offsetMax = Vector2.zero;
            isApplying = false;
        }
    }
}
