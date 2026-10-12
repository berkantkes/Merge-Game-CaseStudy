using MergeGame.Events;
using MergeGame.Items;
using MergeGame.Pooling;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MergeGame.LevelEditor
{
    public class LevelEditorPaletteItem : PoolObject
    {
        [SerializeField] private Button button;
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text label;
        [SerializeField] private GameObject selectionHighlight;

        private ItemStack item;

        private void Awake()
        {
            button.onClick.AddListener(OnClicked);
        }

        public void Bind(ItemStack value, ItemLevelDefinition definition)
        {
            item = value;
            icon.sprite = definition.Sprite;
            label.text = $"{definition.DisplayName}\nLV {item.Level}";
            selectionHighlight.SetActive(false);
        }

        public void SetSelected(ItemStack selected)
        {
            selectionHighlight.SetActive(item.Type == selected.Type && item.Level == selected.Level);
        }

        private void OnClicked()
        {
            EventManager.ItemSelected?.Invoke(item);
        }

        public override void ResetState()
        {
            item = default;
            icon.sprite = null;
            label.text = string.Empty;
            selectionHighlight.SetActive(false);
        }

        private void OnDestroy()
        {
            if (button != null)
            {
                button.onClick.RemoveListener(OnClicked);
            }
        }
    }
}
