using System.Collections.Generic;
using MergeGame.LevelEditor;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MergeGame.EditorTools
{
    [InitializeOnLoad]
    public static class LevelEditorBootstrap
    {
        private static readonly Dictionary<LevelEditorView, LevelEditorController> controllers = new Dictionary<LevelEditorView, LevelEditorController>();

        static LevelEditorBootstrap()
        {
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
            AssemblyReloadEvents.beforeAssemblyReload += Clear;
            SceneManager.sceneLoaded += OnSceneLoaded;
            if (EditorApplication.isPlaying)
            {
                EditorApplication.delayCall += Attach;
            }
        }

        private static void OnPlayModeChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                Attach();
            }
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                Clear();
            }
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (EditorApplication.isPlaying)
            {
                EditorApplication.delayCall += Attach;
            }
        }

        private static void Attach()
        {
            if (!EditorApplication.isPlaying)
            {
                return;
            }
            var removed = new List<LevelEditorView>();
            foreach (var entry in controllers)
            {
                if (entry.Key == null)
                {
                    entry.Value.Dispose();
                    removed.Add(entry.Key);
                }
            }
            foreach (var key in removed)
            {
                controllers.Remove(key);
            }
            foreach (var view in Object.FindObjectsByType<LevelEditorView>(FindObjectsSortMode.None))
            {
                if (!controllers.ContainsKey(view))
                {
                    controllers.Add(view, new LevelEditorController(view));
                }
            }
        }

        private static void Clear()
        {
            foreach (var controller in controllers.Values)
            {
                controller.Dispose();
            }
            controllers.Clear();
        }
    }
}
