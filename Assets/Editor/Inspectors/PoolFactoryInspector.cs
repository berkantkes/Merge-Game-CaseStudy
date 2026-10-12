using MergeGame.Pooling;
using UnityEditor;

namespace MergeGame.EditorTools
{
    [CustomEditor(typeof(PoolFactory))]
    [CanEditMultipleObjects]
    public class PoolFactoryInspector : Editor
    {
        // Use IMGUI to avoid the default UI Toolkit property inspector text path.
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
        }
    }
}
