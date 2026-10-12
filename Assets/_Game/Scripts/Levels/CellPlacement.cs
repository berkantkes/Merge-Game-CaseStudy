using System;
using MergeGame.Items;
using UnityEngine;

namespace MergeGame.Levels
{
    [Serializable]
    public struct CellPlacement
    {
        [SerializeField] private int row;
        [SerializeField] private int column;
        [SerializeField] private ItemType itemType;
        [SerializeField] private int itemLevel;
        public int Row => row;
        public int Column => column;
        public ItemStack Item => new ItemStack(itemType, itemLevel);
    }
}
