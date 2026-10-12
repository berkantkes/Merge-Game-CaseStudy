using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace MergeGame.EditorTools
{
    public static class LevelEditorSetup
    {
        public const string CatalogPath = "Assets/_Game/Items/Item Catalog.asset";
        public const string ScenePath = "Assets/Scenes/LevelEditor.unity";

        [MenuItem("Tools/Merge Game/Open Level Editor")]
        public static void Open()
        {
            if (EditorApplication.isPlaying)
            {
                Debug.LogError("Stop Play Mode first.");
                return;
            }
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return;
            }
            EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        }
    }
}
