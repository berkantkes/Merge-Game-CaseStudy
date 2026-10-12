using System.Collections.Generic;
using UnityEngine;

namespace MergeGame.Pooling
{
    public class PoolFactory : MonoBehaviour
    {
        [SerializeField] private PoolDefinition[] definitions;
        private readonly Dictionary<PoolType, ObjectPool> pools = new Dictionary<PoolType, ObjectPool>();
        public static PoolFactory Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void Initialize()
        {
            foreach (var definition in definitions)
            {
                if (pools.ContainsKey(definition.type))
                {
                    continue;
                }
                if (definition.prefab == null)
                {
                    Debug.LogError("missing pool prefab");
                    continue;
                }
                var root = new GameObject(definition.type.ToString()).transform;
                root.SetParent(transform, false);
                root.gameObject.SetActive(false);
                var pool = new ObjectPool(definition.prefab, root, definition.initialSize);
                pools.Add(definition.type, pool);
            }
        }

        public T Get<T>(PoolType type, Transform parent) where T : PoolObject
        {
            bool found = pools.TryGetValue(type, out var pool);
            if (!found)
            {
                Debug.LogError("missing pool");
                return null;
            }
            return (T)pool.Get(parent);
        }

        public void ReturnAll()
        {
            foreach (var pool in pools.Values)
            {
                pool.ReturnAll();
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
    }
}
