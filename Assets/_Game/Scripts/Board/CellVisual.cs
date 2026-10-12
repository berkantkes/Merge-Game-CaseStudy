using MergeGame.Items;
using MergeGame.Pooling;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MergeGame.Board
{
    public class CellVisual : PoolObject
    {
        [SerializeField] private GameObject alternateBackground;
        [SerializeField] private Image icon;
        [SerializeField] private GameObject badge;
        [SerializeField] private TextMeshProUGUI levelText;

        public void Initialize(int row, int column)
        {
            alternateBackground.SetActive((row + column) % 2 != 0);
        }

        public override void ResetState()
        {
            icon.sprite = null;
            icon.gameObject.SetActive(false);
            badge.SetActive(false);
            levelText.text = string.Empty;
            alternateBackground.SetActive(false);
        }

        public void ShowItem(ItemLevelDefinition definition, int level)
        {
            bool visible = definition != null;
            icon.gameObject.SetActive(visible);
            badge.SetActive(visible);
            if (!visible)
            {
                return;
            }
            icon.sprite = definition.Sprite;
            levelText.text = level.ToString();
        }
    }
}
