using MergeGame.Pooling;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MergeGame.Loading
{
    public static class SceneLoader
    {
        public static AsyncOperation Load(string sceneName)
        {
            if (PoolFactory.Instance != null)
            {
                PoolFactory.Instance.ReturnAll();
            }
            return SceneManager.LoadSceneAsync(sceneName);
        }
    }
}
