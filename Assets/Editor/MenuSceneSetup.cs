using System.IO;
using MergeGame.Menu;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuSceneSetup
{
    [InitializeOnLoadMethod]
    private static void ApplyPendingSetup()
    {
        if (Application.isBatchMode || (!File.Exists("Temp/MenuSetup.request") && !File.Exists("Temp/MenuTypography.request")))
        {
            return;
        }

        EditorApplication.delayCall += () =>
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                Debug.LogError("Stop Play Mode before menu setup.");
                return;
            }

            if (File.Exists("Temp/MenuTypography.request"))
            {
                UpdateTypography();
                File.Delete("Temp/MenuTypography.request");
                return;
            }

            Create();
            File.Delete("Temp/MenuSetup.request");
        };
    }

    public static void Create()
    {
        const string art = "Assets/_Game/Art/";
        var importer = (TextureImporter)AssetImporter.GetAtPath(art + "menu-play-button.png");
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.alphaIsTransparency = true;
        importer.mipmapEnabled = false;
        importer.maxTextureSize = 2048;
        importer.SaveAndReimport();

        var font = LoadMenuFont();
        var scene = SceneManager.GetSceneByPath("Assets/Scenes/Menu.unity");
        if (!scene.isLoaded)
        {
            scene = EditorSceneManager.OpenScene("Assets/Scenes/Menu.unity", OpenSceneMode.Additive);
        }
        SceneManager.SetActiveScene(scene);
        if (System.Array.Exists(scene.GetRootGameObjects(), root => root.name == "Menu Canvas"))
        {
            Debug.LogError("Menu already exists.");
            return;
        }

        var canvasObject = new GameObject("Menu Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvasObject.layer = 5;
        canvasObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

        var background = Rect("Background", canvasObject.transform);
        Stretch(background);
        var backgroundImage = background.gameObject.AddComponent<Image>();
        backgroundImage.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(art + "background.png");
        backgroundImage.raycastTarget = false;
        var fitter = background.gameObject.AddComponent<AspectRatioFitter>();
        fitter.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
        fitter.aspectRatio = backgroundImage.sprite.rect.width / backgroundImage.sprite.rect.height;

        var title = Label("Title", canvasObject.transform, "SEASIDE\nWORKSHOP", font, 100);
        Place(title.rectTransform, new Vector2(.5f, .76f), new Vector2(950, 260));
        title.color = new Color32(24, 89, 96, 255);

        var controls = Rect("Play Area", canvasObject.transform);
        Place(controls, new Vector2(.5f, .28f), new Vector2(780, 390));
        var level = Label("Level", controls, "LEVEL 1", font, 44);
        Place(level.rectTransform, new Vector2(.5f, .86f), new Vector2(600, 75));
        level.color = new Color32(24, 89, 96, 255);
        level.fontStyle = FontStyles.Normal;

        var buttonRect = Rect("Play Button", controls);
        Place(buttonRect, new Vector2(.5f, .45f), new Vector2(780, 280));
        var buttonImage = buttonRect.gameObject.AddComponent<Image>();
        buttonImage.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(art + "menu-play-button.png");
        buttonImage.preserveAspect = true;
        var button = buttonRect.gameObject.AddComponent<Button>();
        button.targetGraphic = buttonImage;
        var colors = button.colors;
        colors.highlightedColor = new Color(1, 1, .9f);
        colors.pressedColor = new Color(.7f, .87f, .87f);
        button.colors = colors;

        var label = Label("Label", buttonRect, "PLAY", font, 72);
        Stretch(label.rectTransform);
        label.fontStyle = FontStyles.Normal;
        label.color = new Color32(255, 249, 230, 255);

        var controller = new GameObject("Menu Controller").AddComponent<MenuController>();
        var serialized = new SerializedObject(controller);
        serialized.FindProperty("levelText").objectReferenceValue = level;
        serialized.ApplyModifiedPropertiesWithoutUndo();
        UnityEventTools.AddPersistentListener(button.onClick, controller.Play);

        new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        Debug.Log("MENU_SETUP_OK");
    }

    private static TMP_FontAsset LoadMenuFont()
    {
        const string path = "Assets/_Game/Art/Fonts/LilitaOne SDF.asset";
        var font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path);
        if (font != null)
        {
            return font;
        }

        var source = AssetDatabase.LoadAssetAtPath<Font>("Assets/_Game/Art/Fonts/LilitaOne-Regular.ttf");
        font = TMP_FontAsset.CreateFontAsset(source);
        font.name = "LilitaOne SDF";
        font.TryAddCharacters("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789 .,!?:;'-+()/");
        font.atlasPopulationMode = AtlasPopulationMode.Static;
        AssetDatabase.CreateAsset(font, path);
        AssetDatabase.AddObjectToAsset(font.material, font);
        foreach (var texture in font.atlasTextures)
        {
            AssetDatabase.AddObjectToAsset(texture, font);
        }
        EditorUtility.SetDirty(font);
        AssetDatabase.SaveAssets();
        return font;
    }

    public static void UpdateTypography()
    {
        var font = LoadMenuFont();
        var scene = SceneManager.GetSceneByPath("Assets/Scenes/Menu.unity");
        if (!scene.isLoaded)
        {
            scene = EditorSceneManager.OpenScene("Assets/Scenes/Menu.unity", OpenSceneMode.Additive);
        }
        foreach (var root in scene.GetRootGameObjects())
        {
            foreach (var label in root.GetComponentsInChildren<TextMeshProUGUI>(true))
            {
                label.font = font;
                label.fontSharedMaterial = font.material;
                label.fontStyle = FontStyles.Normal;
                if (label.name == "Title")
                {
                    label.text = "SEASIDE\nWORKSHOP";
                    label.color = new Color32(24, 89, 96, 255);
                }

                label.ForceMeshUpdate();
                EditorUtility.SetDirty(label);
            }
        }
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("MENU_TYPOGRAPHY_OK");
    }

    private static RectTransform Rect(string name, Transform parent)
    {
        var rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        rect.gameObject.layer = 5;
        return rect;
    }

    private static void Place(RectTransform rect, Vector2 anchor, Vector2 size)
    {
        rect.anchorMin = rect.anchorMax = anchor;
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = size;
    }

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = rect.offsetMax = Vector2.zero;
    }

    private static TextMeshProUGUI Label(string name, Transform parent, string text, TMP_FontAsset font, int size)
    {
        var label = Rect(name, parent).gameObject.AddComponent<TextMeshProUGUI>();
        label.font = font;
        label.text = text;
        label.fontSize = size;
        label.alignment = TextAlignmentOptions.Center;
        label.raycastTarget = false;
        return label;
    }
}
