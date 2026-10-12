using System;
using MergeGame.Items;
using MergeGame.LevelEditor;

namespace MergeGame.Events
{
    public static class EventManager
    {
        public static Action<int, int> CellPressed;
        public static Action<ItemStack> ItemSelected;
        public static Action<LevelEditorAction> EditorActionRequested;
    }
}
