using System.IO;
using MergeGame.Items;
using MergeGame.Levels;
using UnityEditor;
using UnityEngine;

namespace MergeGame.EditorTools
{
    public static class LevelAssetStore
    {
        public static bool Save(LevelDataSO draft, ItemCatalogSO catalog, string path, out LevelDataSO saved, out string error)
        {
            saved = null;
            error = null;
            if (draft == null)
            {
                error = "Level draft is missing.";
                return false;
            }
            if (!LevelDataValidator.TryValidate(draft, catalog, out error))
            {
                return false;
            }
            path = path.Replace('\\', '/');
            var full = Path.GetFullPath(path);
            var assets = Path.GetFullPath(Application.dataPath) + Path.DirectorySeparatorChar;
            if (!path.StartsWith("Assets/") || !full.StartsWith(assets, System.StringComparison.OrdinalIgnoreCase) || Path.GetExtension(path) != ".asset" || !AssetDatabase.IsValidFolder(Path.GetDirectoryName(path).Replace('\\', '/')))
            {
                error = "Choose an asset path inside Assets.";
                return false;
            }
            var existing = AssetDatabase.LoadMainAssetAtPath(path);
            if (existing != null && !(existing is LevelDataSO))
            {
                error = "This path contains another asset type.";
                return false;
            }
            saved = existing as LevelDataSO;
            if (saved == null)
            {
                saved = ScriptableObject.CreateInstance<LevelDataSO>();
                LevelDataEditor.CopyFrom(saved, draft);
                AssetDatabase.CreateAsset(saved, path);
            }
            else
            {
                LevelDataEditor.CopyFrom(saved, draft);
                EditorUtility.SetDirty(saved);
            }
            AssetDatabase.SaveAssetIfDirty(saved);
            return true;
        }
    }
}
