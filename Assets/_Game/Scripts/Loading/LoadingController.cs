using System.Collections;
using MergeGame.Pooling;
using UnityEngine;

namespace MergeGame.Loading
{
    public class LoadingController : MonoBehaviour
    {
        private IEnumerator Start()
        {
            yield return null;
            PoolFactory.Instance.Initialize(); 
            yield return SceneLoader.Load("Menu");
        }
    }
}
