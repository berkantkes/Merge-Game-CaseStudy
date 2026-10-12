using MergeGame.Items;
using UnityEditor;

namespace MergeGame.EditorTools
{
    [CustomEditor(typeof(ItemCatalogSO))]
    [CanEditMultipleObjects]
    public class ItemCatalogSOInspector : Editor
    {
        // Use IMGUI to avoid the default UI Toolkit property inspector text path.
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
        }
    }
}
