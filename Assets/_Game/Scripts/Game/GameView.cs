using System;
using DG.Tweening;
using MergeGame.Events;
using MergeGame.Items;
using MergeGame.Pooling;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MergeGame.Game
{
    public class GameView : MonoBehaviour
    {
        [SerializeField] private Button spawnButton;
        [SerializeField] private RectTransform spawnOrigin;
        [SerializeField] private RectTransform flightLayer;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private float scoreBumpScale = 1.2f;
        [SerializeField] private float scoreBumpDuration = 0.16f;

        private int displayedScore;
        private Tween scoreBumpTween;

        public void ShowScore(int score)
        {
            bool increased = score > displayedScore;
            displayedScore = score;
            scoreText.text = score.ToString("N0", System.Globalization.CultureInfo.InvariantCulture);
            StopScoreBump();
            if (increased)
            {
                PlayScoreBump();
            }
        }

        private void PlayScoreBump()
        {
            var sequence = DOTween.Sequence();
            sequence.Append(scoreText.rectTransform.DOScale(scoreBumpScale, scoreBumpDuration * 0.4f).SetEase(Ease.OutQuad));
            sequence.Append(scoreText.rectTransform.DOScale(1f, scoreBumpDuration * 0.6f).SetEase(Ease.InOutQuad));
            sequence.OnComplete(() =>
            {
                scoreBumpTween = null;
            });
            scoreBumpTween = sequence;
        }

        private void StopScoreBump()
        {
            if (scoreBumpTween != null)
            {
                scoreBumpTween.Kill();
                scoreBumpTween = null;
            }
            scoreText.rectTransform.localScale = Vector3.one;
        }

        public void PlaySpawn(Sprite sprite, RectTransform target, Action completed)
        {
            var flight = PoolFactory.Instance.Get<SpawnItemVisual>(PoolType.SpawnItemVisual, flightLayer);
            flight.Play(sprite, spawnOrigin, target, flightLayer, completed);
        }

        private void OnEnable()
        {
            EventManager.ScoreChanged += ShowScore;
            spawnButton.onClick.AddListener(OnSpawnClicked);
        }

        private void OnDisable()
        {
            EventManager.ScoreChanged -= ShowScore;
            spawnButton.onClick.RemoveListener(OnSpawnClicked);
            StopScoreBump();
        }

        private void OnSpawnClicked()
        {
            EventManager.SpawnRequested?.Invoke();
        }

        public void SetSpawnEnabled(bool enabled)
        {
            spawnButton.interactable = enabled;
        }
    }
}
