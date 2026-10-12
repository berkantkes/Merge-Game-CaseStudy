using System;
using UnityEngine;

namespace MergeGame.Pooling
{
    [Serializable]
    public class PoolDefinition
    {
        public PoolType type;
        public PoolObject prefab;
        [Min(0)] public int initialSize;
    }
}
