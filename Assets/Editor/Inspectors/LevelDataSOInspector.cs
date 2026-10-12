using MergeGame.Levels;
using UnityEditor;

namespace MergeGame.EditorTools
{
    [CustomEditor(typeof(LevelDataSO))]
    [CanEditMultipleObjects]
    public class LevelDataSOInspector : Editor
    {
        // Use IMGUI to avoid the default UI Toolkit property inspector text path.
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
        }
    }
}
