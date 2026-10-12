using Cysharp.Threading.Tasks;
using MergeGame.Pooling;
using UnityEngine;

namespace MergeGame.Loading
{
    public class LoadingController : MonoBehaviour
    {
        private void Start()
        {
            InitializeAsync().Forget();
        }

        private async UniTask InitializeAsync()
        {
            await UniTask.NextFrame(destroyCancellationToken);
            PoolFactory.Instance.Initialize();
            SceneLoader.Load(Constants.SceneNames.Menu);
        }
    }
}
