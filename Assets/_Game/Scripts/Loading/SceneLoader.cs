using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MergeGame.Pooling;
using MergeGame.Events;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MergeGame.Loading
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] private Canvas transitionCanvas;
        [SerializeField] private CanvasGroup transitionGroup;
        [SerializeField] private float fadeDuration = 0.25f;

        private static SceneLoader instance;
        private bool isLoading;
        private Tween fadeTween;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                gameObject.SetActive(false);
                Destroy(gameObject);
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject);
            HideTransition();
        }

        public static void Load(string sceneName)
        {
            if (instance == null)
            {
                Debug.LogError("missing scene loader");
                return;
            }
            if (instance.isLoading)
            {
                return;
            }
            instance.LoadSceneAsync(sceneName).Forget();
        }

        private async UniTask LoadSceneAsync(string sceneName)
        {
            isLoading = true;
            var cancellationToken = destroyCancellationToken;
            try
            {
                transitionCanvas.enabled = true;
                transitionGroup.blocksRaycasts = true;
                EventManager.BoardInteractionCancelled?.Invoke();
                await FadeAsync(1f, cancellationToken);
                ReturnPoolItems();
                await SceneManager.LoadSceneAsync(sceneName);
                await UniTask.NextFrame(cancellationToken);
                Canvas.ForceUpdateCanvases();
                await FadeAsync(0f, cancellationToken);
            }
            finally
            {
                StopFade();
                if (this != null)
                {
                    HideTransition();
                }
                isLoading = false;
            }
        }

        private async UniTask FadeAsync(float alpha, CancellationToken cancellationToken)
        {
            fadeTween = transitionGroup.DOFade(alpha, fadeDuration).SetEase(Ease.Linear).SetUpdate(true);
            await UniTask.WaitUntil(IsFadeFinished, cancellationToken: cancellationToken);
            fadeTween = null;
        }

        private bool IsFadeFinished()
        {
            return !fadeTween.IsActive() || fadeTween.IsComplete();
        }

        private void ReturnPoolItems()
        {
            if (PoolFactory.Instance != null)
            {
                PoolFactory.Instance.ReturnAll();
            }
        }

        private void StopFade()
        {
            if (fadeTween != null)
            {
                fadeTween.Kill();
                fadeTween = null;
            }
        }

        private void HideTransition()
        {
            transitionGroup.alpha = 0f;
            transitionGroup.blocksRaycasts = false;
            transitionCanvas.enabled = false;
        }

        private void OnDestroy()
        {
            StopFade();
            if (instance == this)
            {
                instance = null;
            }
        }
    }
}
