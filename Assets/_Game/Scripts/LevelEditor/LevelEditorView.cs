using System;
using System.Collections.Generic;
using System.Globalization;
using MergeGame.Board;
using MergeGame.Events;
using MergeGame.Items;
using MergeGame.Levels;
using MergeGame.Pooling;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MergeGame.LevelEditor
{
    public class LevelEditorView : MonoBehaviour
    {
        [SerializeField] private BoardView board;
        [SerializeField] private ItemCatalogSO catalog;
        [SerializeField] private LevelDataSO initialLevel;
        [SerializeField] private TMP_Dropdown levelDropdown;
        [SerializeField] private TMP_InputField rowsInput, columnsInput, rowSpacingInput, columnSpacingInput;
        [SerializeField] private TMP_Text draftLabel, statusLabel, selectionLabel;
        [SerializeField] private TMP_Text errorLabel;
        [SerializeField] private RectTransform palette;
        [SerializeField] private LevelEditorActionButton[] actions;
        public BoardView Board => board;
        public ItemCatalogSO Catalog => catalog;
        public LevelDataSO InitialLevel => initialLevel;
        public int SelectedLevelIndex => levelDropdown.value;
        public event Action Closed;
        private readonly List<LevelEditorPaletteItem> paletteItems = new List<LevelEditorPaletteItem>();

        private void Awake()
        {
            foreach (var entry in actions)
            {
                var action = entry.action;
                entry.button.onClick.AddListener(() =>
                {
                    EventManager.EditorActionRequested?.Invoke(action);
                });
            }
        }

        public void SetLevels(List<string> names, int selectedIndex)
        {
            levelDropdown.ClearOptions();
            levelDropdown.AddOptions(names);
            levelDropdown.SetValueWithoutNotify(Mathf.Max(0, selectedIndex));
            levelDropdown.RefreshShownValue();
        }

        public bool TryReadSize(out int rows, out int columns, out float rowSpacing, out float columnSpacing)
        {
            bool r = int.TryParse(rowsInput.text, out rows);
            bool c = int.TryParse(columnsInput.text, out columns);
            bool rs = float.TryParse(rowSpacingInput.text, NumberStyles.Float, CultureInfo.InvariantCulture, out rowSpacing);
            bool cs = float.TryParse(columnSpacingInput.text, NumberStyles.Float, CultureInfo.InvariantCulture, out columnSpacing);
            return r && c && rs && cs;
        }

        public void ShowSize(LevelDataSO data)
        {
            rowsInput.SetTextWithoutNotify(data.Rows.ToString());
            columnsInput.SetTextWithoutNotify(data.Columns.ToString());
            rowSpacingInput.SetTextWithoutNotify(data.RowSpacing.ToString(CultureInfo.InvariantCulture));
            columnSpacingInput.SetTextWithoutNotify(data.ColumnSpacing.ToString(CultureInfo.InvariantCulture));
        }

        public void ShowDraft(string name, bool dirty) => draftLabel.text = name + (dirty ? "  * UNSAVED" : "  - SAVED");
        public void ShowStatus(string text, bool error = false)
        {
            statusLabel.gameObject.SetActive(!error);
            errorLabel.gameObject.SetActive(error);
            var label = error ? errorLabel : statusLabel;
            label.text = text;
        }

        public void SelectItem(ItemStack item)
        {
            selectionLabel.text = item.IsEmpty ? "ERASER - Click a cell to clear it" :
                catalog.TryGet(item.Type, item.Level, out var definition) ? $"{definition.DisplayName} - Level {item.Level}" : "Select an item";
            foreach (var paletteItem in paletteItems)
            {
                paletteItem.SetSelected(item);
            }
        }

        public void BuildPalette()
        {
            var poolFactory = PoolFactory.Instance;
            if (poolFactory == null || palette == null)
            {
                Debug.LogError("missing palette references");
                return;
            }
            poolFactory.Initialize();
            board.Initialize(poolFactory);
            ClearPalette();
            foreach (var chain in catalog.Chains)
            {
                for (int i = 0; i < chain.MaxLevel; i++)
                {
                    var definition = chain.Levels[i];
                    var item = new ItemStack(chain.Type, i + 1);
                    var paletteItem = poolFactory.Get<LevelEditorPaletteItem>(PoolType.PaletteItem, palette);
                    paletteItem.name = definition.DisplayName;
                    paletteItem.Bind(item, definition);
                    paletteItems.Add(paletteItem);
                }
            }
        }

        private void ClearPalette()
        {
            foreach (var paletteItem in paletteItems)
            {
                if (paletteItem != null)
                {
                    paletteItem.ReturnToPool();
                }
            }
            paletteItems.Clear();
        }

        private void OnDestroy()
        {
            ClearPalette();
            Closed?.Invoke();
            Closed = null;
        }
    }
}
