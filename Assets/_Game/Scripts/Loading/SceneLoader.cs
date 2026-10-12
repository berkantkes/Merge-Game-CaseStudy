using MergeGame.Pooling;
using MergeGame.Events;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MergeGame.Loading
{
    public static class SceneLoader
    {
        public static AsyncOperation Load(string sceneName)
        {
            EventManager.BoardInteractionCancelled?.Invoke();
            if (PoolFactory.Instance != null)
            {
                PoolFactory.Instance.ReturnAll();
            }
            return SceneManager.LoadSceneAsync(sceneName);
        }
    }
}
