namespace MergeGame.Items
{
    public struct ItemStack
    {
        public ItemType Type { get; }
        public int Level { get; }
        public bool IsEmpty => Type == ItemType.None;

        public ItemStack(ItemType type, int level)
        {
            Type = type;
            Level = type == ItemType.None ? 0 : level;
        }
    }
}
