using System;
using UnityEngine;

namespace MergeGame.Items
{
    [Serializable]
    public class ItemLevelDefinition
    {
        [SerializeField] private string displayName;
        [SerializeField] private Sprite sprite;
        public string DisplayName => displayName;
        public Sprite Sprite => sprite;
    }
}
