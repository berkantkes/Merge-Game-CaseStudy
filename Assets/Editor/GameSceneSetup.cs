using System.IO;
using MergeGame.Board;
using MergeGame.Game;
using MergeGame.Items;
using MergeGame.Levels;
using MergeGame.Pooling;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSceneSetup
{
    [InitializeOnLoadMethod]
    private static void ApplyPendingSetup()
    {
        if (!File.Exists("Temp/GameBoardSetup.request") && !File.Exists("Temp/LevelDataSetup.request"))
        {
            return;
        }
        EditorApplication.delayCall += () =>
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                Debug.LogError("Stop Play Mode before board setup.");
                return;
            }

            if (File.Exists("Temp/LevelDataSetup.request"))
            {
                ConfigureLevel();
                File.Delete("Temp/LevelDataSetup.request");
                return;
            }

            Create();
            File.Delete("Temp/GameBoardSetup.request");
        };
    }

    [MenuItem("Tools/Merge Game/Create Game Board")]
    public static void Create()
    {
        var scene = SceneManager.GetSceneByPath("Assets/Scenes/Game.unity");
        if (!scene.isLoaded)
        {
            scene = EditorSceneManager.OpenScene("Assets/Scenes/Game.unity", OpenSceneMode.Additive);
        }
        foreach (var root in scene.GetRootGameObjects())
        {
            if (root.name != "Game Canvas")
            {
                continue;
            }

            Debug.LogError("Game board already exists.");
            return;
        }
        var canvasObject = new GameObject("Game Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler));
        SceneManager.MoveGameObjectToScene(canvasObject, scene);
        canvasObject.layer = 5;
        canvasObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
        var background = Rect("Background", canvasObject.transform);
        Stretch(background);
        var image = background.gameObject.AddComponent<Image>();
        image.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Game/Art/background.png");
        image.raycastTarget = false;
        var aspect = background.gameObject.AddComponent<AspectRatioFitter>();
        aspect.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
        aspect.aspectRatio = image.sprite.rect.width / image.sprite.rect.height;

        var safeArea = Rect("Safe Area", canvasObject.transform);
        Stretch(safeArea);
        safeArea.gameObject.AddComponent<SafeArea>();
        var boardArea = Rect("Board Area", safeArea);
        boardArea.anchorMin = new Vector2(.05f, .06f);
        boardArea.anchorMax = new Vector2(.95f, .64f);
        boardArea.offsetMin = boardArea.offsetMax = Vector2.zero;
        var board = boardArea.gameObject.AddComponent<BoardView>();
        ConnectLevel(scene, board);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("GAME_BOARD_SETUP_OK");
    }

    public static void ConfigureLevel()
    {
        var scene = SceneManager.GetSceneByPath("Assets/Scenes/Game.unity");
        if (!scene.isLoaded)
        {
            scene = EditorSceneManager.OpenScene("Assets/Scenes/Game.unity", OpenSceneMode.Additive);
        }
        foreach (var root in scene.GetRootGameObjects())
        {
            var board = root.GetComponentInChildren<BoardView>(true);
            if (board == null)
            {
                continue;
            }

            ConnectLevel(scene, board);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            Debug.Log("LEVEL_DATA_SETUP_OK");
            return;
        }
        Debug.LogError("Board view is missing.");
    }

    private static void ConnectLevel(Scene scene, BoardView board)
    {
        const string folder = "Assets/_Game/Levels";
        if (!AssetDatabase.IsValidFolder(folder))
        {
            AssetDatabase.CreateFolder("Assets/_Game", "Levels");
        }
        var level = AssetDatabase.LoadAssetAtPath<LevelDataSO>(folder + "/Level 01.asset");
        if (level == null)
        {
            level = ScriptableObject.CreateInstance<LevelDataSO>();
            AssetDatabase.CreateAsset(level, folder + "/Level 01.asset");
        }
        GameController controller = null;
        foreach (var root in scene.GetRootGameObjects())
        {
            controller = root.GetComponentInChildren<GameController>(true);
            if (controller != null)
            {
                break;
            }
        }
        if (controller == null)
        {
            controller = new GameObject("Game Controller").AddComponent<GameController>();
            SceneManager.MoveGameObjectToScene(controller.gameObject, scene);
        }
        var serialized = new SerializedObject(controller);
        serialized.FindProperty("levelData").objectReferenceValue = level;
        serialized.FindProperty("catalog").objectReferenceValue = AssetDatabase.LoadAssetAtPath<ItemCatalogSO>("Assets/_Game/Items/Item Catalog.asset");
        serialized.FindProperty("boardView").objectReferenceValue = board;
        serialized.ApplyModifiedPropertiesWithoutUndo();
        var boardData = new SerializedObject(board);
        boardData.FindProperty("boardArea").objectReferenceValue = board.GetComponent<RectTransform>();
        var gridProperty = boardData.FindProperty("gridRect");
        if (gridProperty.objectReferenceValue == null)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Game/Prefabs/Board Grid.prefab");
            var grid = (GameObject)PrefabUtility.InstantiatePrefab(prefab, board.transform);
            grid.name = "Grid";
            gridProperty.objectReferenceValue = grid.GetComponent<RectTransform>();
        }
        var gridTransform = (RectTransform)gridProperty.objectReferenceValue;
        boardData.FindProperty("boardInput").objectReferenceValue = gridTransform.GetComponent<BoardInput>();
        boardData.FindProperty("gridLayout").objectReferenceValue = gridTransform.GetComponent<GridLayoutGroup>();
        boardData.ApplyModifiedPropertiesWithoutUndo();
        foreach (var root in scene.GetRootGameObjects())
        {
            if (root.GetComponent<PoolFactory>() != null)
            {
                return;
            }
        }
        var factoryPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Game/Prefabs/Pool Factory.prefab");
        PrefabUtility.InstantiatePrefab(factoryPrefab, scene);
    }

    private static RectTransform Rect(string name, Transform parent)
    {
        var rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        rect.gameObject.layer = 5;
        return rect;
    }

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = rect.offsetMax = Vector2.zero;
    }
}
