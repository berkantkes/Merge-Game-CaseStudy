using UnityEngine;

namespace MergeGame.Pooling
{
    public class PoolObject : MonoBehaviour
    {
        private ObjectPool owner;

        public void InitializePool(ObjectPool pool)
        {
            owner = pool;
        }

        public void ReturnToPool()
        {
            if (owner != null)
            {
                owner.Return(this);
            }
        }

        public virtual void ResetState()
        {
        }
    }
}
