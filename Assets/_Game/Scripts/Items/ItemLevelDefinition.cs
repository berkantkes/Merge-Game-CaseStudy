using System;
using UnityEngine;

namespace MergeGame.Items
{
    [Serializable]
    public class ItemLevelDefinition
    {
        [SerializeField] private string displayName;
        [SerializeField] private Sprite sprite;
        [SerializeField, Min(0)] private int mergeScore;
        public string DisplayName => displayName;
        public Sprite Sprite => sprite;
        public int MergeScore
        {
            get
            {
                return mergeScore;
            }
        }
    }
}
