using System.Collections.Generic;
using UnityEngine;

namespace MergeGame.Pooling
{
    public class ObjectPool
    {
        private readonly PoolObject prefab;
        private readonly Transform root;
        private readonly Stack<PoolObject> available = new Stack<PoolObject>();
        private readonly HashSet<PoolObject> active = new HashSet<PoolObject>();

        public ObjectPool(PoolObject prefab, Transform root, int initialSize)
        {
            this.prefab = prefab;
            this.root = root;
            for (int i = 0; i < initialSize; i++)
            {
                available.Push(Create());
            }
        }

        private PoolObject Create()
        {
            var item = Object.Instantiate(prefab, root);
            item.gameObject.SetActive(false);
            item.InitializePool(this);
            return item;
        }

        public PoolObject Get(Transform parent)
        {
            PoolObject item = null;
            while (available.Count > 0 && item == null)
            {
                item = available.Pop();
            }
            if (item == null)
            {
                item = Create();
            }
            item.transform.SetParent(parent, false);
            item.transform.localScale = prefab.transform.localScale;
            item.transform.localRotation = prefab.transform.localRotation;
            item.transform.SetAsLastSibling();
            active.Add(item);
            item.gameObject.SetActive(true);
            return item;
        }

        public void Return(PoolObject item)
        {
            bool wasActive = active.Remove(item);
            if (!wasActive || item == null)
            {
                return;
            }
            item.ResetState();
            item.gameObject.SetActive(false);
            item.transform.SetParent(root, false);
            available.Push(item);
        }

        public void ReturnAll()
        {
            var items = new List<PoolObject>(active);
            foreach (var item in items)
            {
                Return(item);
            }
        }
    }
}
