using System;
using System.Collections.Generic;
using System.IO;
using MergeGame.Board;
using MergeGame.Events;
using MergeGame.Items;
using MergeGame.LevelEditor;
using MergeGame.Levels;
using UnityEditor;
using UnityEngine;

namespace MergeGame.EditorTools
{
    // Plain editor controller: never attached to a scene or included in a player build.
    public class LevelEditorController : IDisposable
    {
        private readonly LevelEditorView view;
        private readonly List<LevelDataSO> levels = new List<LevelDataSO>();
        private LevelDataSO source;
        private LevelDataSO draft;
        private BoardState state;
        private ItemStack selected;
        private string savedSnapshot;
        private bool disposed;
        private bool IsDirty => source == null || JsonUtility.ToJson(draft) != savedSnapshot;

        public LevelEditorController(LevelEditorView view)
        {
            this.view = view;
            EventManager.EditorActionRequested += HandleAction;
            EventManager.ItemSelected += Select;
            EventManager.CellPressed += Place;
            view.Closed += Dispose;
            Undo.undoRedoPerformed += OnUndoRedo;
            if (view.Catalog == null || !view.Catalog.TryValidate(out _))
            {
                view.ShowStatus("Item catalog is invalid.", true);
                return;
            }
            view.BuildPalette();
            ReloadLevels();
            if (view.InitialLevel != null)
            {
                Load(view.InitialLevel);
            }
            else
            {
                New();
            }
            Select(default);
        }

        private void HandleAction(LevelEditorAction action)
        {
            if (draft == null && action != LevelEditorAction.New)
            {
                return;
            }
            switch (action)
            {
                case LevelEditorAction.New:
                    if (draft == null || CanDiscard())
                    {
                        New();
                    }

                    break;
                case LevelEditorAction.Load:
                    if (levels.Count > 0 && CanDiscard())
                    {
                        Load(levels[view.SelectedLevelIndex]);
                    }

                    break;
                case LevelEditorAction.ApplySize:
                    Resize();
                    break;
                case LevelEditorAction.Save:
                    Save(false);
                    break;
                case LevelEditorAction.SaveAs:
                    Save(true);
                    break;
                case LevelEditorAction.ClearBoard:
                    ClearBoard();
                    break;
                case LevelEditorAction.Eraser:
                    Select(default);
                    break;
                case LevelEditorAction.Undo:
                    Undo.PerformUndo();
                    break;
                case LevelEditorAction.Redo:
                    Undo.PerformRedo();
                    break;
            }
        }

        private bool CanDiscard() => !IsDirty || EditorUtility.DisplayDialog(
            "Unsaved level", "Discard unsaved changes?", "Discard", "Cancel");

        private void New()
        {
            ReplaceDraft(ScriptableObject.CreateInstance<LevelDataSO>(), null);
            view.ShowStatus("New draft. Save before leaving Play Mode.");
        }

        private void Load(LevelDataSO level)
        {
            if (!LevelDataValidator.TryValidate(level, view.Catalog, out var error))
            {
                view.ShowStatus(error, true);
                return;
            }
            ReplaceDraft(UnityEngine.Object.Instantiate(level), level);
            view.ShowStatus("Level loaded. Select an item and click a cell.");
        }

        private void ReplaceDraft(LevelDataSO next, LevelDataSO original)
        {
            if (draft != null)
            {
                Undo.ClearUndo(draft);
                UnityEngine.Object.DestroyImmediate(draft);
            }
            source = original;
            draft = next;
            draft.hideFlags = HideFlags.DontSave;
            savedSnapshot = JsonUtility.ToJson(draft);
            ReloadLevels();
            RefreshBoard();
            RefreshStatus();
        }

        private void Select(ItemStack item) { selected = item; view.SelectItem(item); }

        private void Place(int row, int column)
        {
            if (draft == null || state == null)
            {
                return;
            }
            var previous = state.GetCell(row, column);
            if (previous.Type == selected.Type && previous.Level == selected.Level)
            {
                return;
            }
            Record("Paint cell");
            if (!LevelDataEditor.SetCell(draft, row, column, selected, view.Catalog, out var error))
            {
                view.ShowStatus(error, true);
                return;
            }
            state.SetCell(row, column, selected, view.Catalog);
            view.Board.RefreshCell(row, column);
            Commit(false);
        }

        private void Resize()
        {
            if (!view.TryReadSize(out int rows, out int columns, out float rowGap, out float columnGap))
            {
                view.ShowStatus("Enter valid numbers. Use a dot for decimals.", true);
                return;
            }
            Record("Resize board");
            if (!LevelDataEditor.TryResize(draft, rows, columns, rowGap, columnGap, out var error))
            {
                view.ShowStatus(error, true);
                return;
            }
            Commit(true);
        }

        private void ClearBoard()
        {
            Record("Clear board");
            foreach (var placement in draft.Placements)
            {
                state.SetCell(placement.Row, placement.Column, default, view.Catalog);
                view.Board.RefreshCell(placement.Row, placement.Column);
            }
            LevelDataEditor.ClearPlacements(draft);
            Commit(false);
        }

        private void Record(string name)
        {
            Undo.IncrementCurrentGroup();
            Undo.RegisterCompleteObjectUndo(draft, name);
        }

        private void Commit(bool rebuild)
        {
            EditorUtility.SetDirty(draft);
            Undo.FlushUndoRecordObjects();
            if (rebuild)
            {
                RefreshBoard();
            }
            RefreshStatus();
            view.ShowStatus("Draft changed. Save before leaving Play Mode.");
        }

        private void OnUndoRedo()
        {
            if (disposed || draft == null)
            {
                return;
            }
            var next = BoardState.Create(draft, view.Catalog);
            if (next == null)
            {
                view.ShowStatus("Board could not be created. Check the Console.", true);
                return;
            }
            bool sameLayout = state != null && state.Rows == next.Rows && state.Columns == next.Columns;
            // Spacing can change without a row/column change, so compare the view's last layout too.
            sameLayout &= lastRowGap == draft.RowSpacing && lastColumnGap == draft.ColumnSpacing;
            if (!sameLayout)
            {
                RefreshBoard();
            }
            else
            {
                for (int row = 0; row < state.Rows; row++)
                {
                    for (int column = 0; column < state.Columns; column++)
                    {
                        var oldItem = state.GetCell(row, column);
                        var newItem = next.GetCell(row, column);
                        if (oldItem.Type == newItem.Type && oldItem.Level == newItem.Level)
                        {
                            continue;
                        }

                        state.SetCell(row, column, newItem, view.Catalog);
                        view.Board.RefreshCell(row, column);
                    }
                }

                view.ShowSize(draft);
            }
            RefreshStatus();
            view.ShowStatus("Undo / Redo applied.");
        }

        private float lastRowGap, lastColumnGap;
        private void RefreshBoard()
        {
            state = BoardState.Create(draft, view.Catalog);
            if (state == null)
            {
                view.ShowStatus("Board could not be created. Check the Console.", true);
                return;
            }
            Canvas.ForceUpdateCanvases();
            view.Board.Build(draft, state, view.Catalog);
            view.Board.SetInputEnabled(true);
            lastRowGap = draft.RowSpacing;
            lastColumnGap = draft.ColumnSpacing;
            view.ShowSize(draft);
        }

        private void Save(bool saveAs)
        {
            if (!LevelDataValidator.TryValidate(draft, view.Catalog, out var error))
            {
                view.ShowStatus(error, true);
                return;
            }
            string path = source != null && !saveAs ? AssetDatabase.GetAssetPath(source) :
                EditorUtility.SaveFilePanelInProject("Save Level", "New Level", "asset", "Choose a level asset location.", "Assets/_Game/Levels");
            if (string.IsNullOrEmpty(path))
            {
                return;
            }
            if (!LevelAssetStore.Save(draft, view.Catalog, path, out var saved, out error))
            {
                view.ShowStatus(error, true);
                return;
            }
            source = saved;
            savedSnapshot = JsonUtility.ToJson(draft);
            ReloadLevels();
            RefreshStatus();
            view.ShowStatus("Saved: " + path);
        }

        private void ReloadLevels()
        {
            levels.Clear();
            var names = new List<string>();
            var paths = new List<string>();
            foreach (string guid in AssetDatabase.FindAssets("t:LevelDataSO", new[] { "Assets" }))
            {
                paths.Add(AssetDatabase.GUIDToAssetPath(guid));
            }
            paths.Sort(StringComparer.Ordinal);
            foreach (string path in paths)
            {
                levels.Add(AssetDatabase.LoadAssetAtPath<LevelDataSO>(path));
                names.Add(Path.GetFileNameWithoutExtension(path));
            }
            view.SetLevels(names, levels.IndexOf(source));
        }

        private void RefreshStatus() => view.ShowDraft(source != null ? source.name : "New Level", IsDirty);

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }
            disposed = true;
            EventManager.EditorActionRequested -= HandleAction;
            EventManager.ItemSelected -= Select;
            EventManager.CellPressed -= Place;
            Undo.undoRedoPerformed -= OnUndoRedo;
            if (view != null)
            {
                view.Closed -= Dispose;
                if (view.Board != null)
                {
                    view.Board.SetInputEnabled(false);
                }
            }
            if (draft != null)
            {
                Undo.ClearUndo(draft);
                UnityEngine.Object.DestroyImmediate(draft);
            }
        }
    }
}
