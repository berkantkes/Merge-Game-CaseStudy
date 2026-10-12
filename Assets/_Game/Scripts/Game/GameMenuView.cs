using MergeGame.Events;
using UnityEngine;
using UnityEngine.UI;

namespace MergeGame.Game
{
    public class GameMenuView : MonoBehaviour
    {
        [SerializeField] private GameObject popup;
        [SerializeField] private Button openButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button menuButton;

        private void OnEnable()
        {
            openButton.onClick.AddListener(Open);
            closeButton.onClick.AddListener(Close);
            restartButton.onClick.AddListener(Restart);
            menuButton.onClick.AddListener(ReturnToMenu);
        }

        private void OnDisable()
        {
            openButton.onClick.RemoveListener(Open);
            closeButton.onClick.RemoveListener(Close);
            restartButton.onClick.RemoveListener(Restart);
            menuButton.onClick.RemoveListener(ReturnToMenu);
        }

        private void Open()
        {
            popup.SetActive(true);
            EventManager.MenuPopupChanged?.Invoke(true);
        }

        private void Close()
        {
            popup.SetActive(false);
            EventManager.MenuPopupChanged?.Invoke(false);
        }

        private void Restart()
        {
            DisableButtons();
            EventManager.RestartRequested?.Invoke();
        }

        private void ReturnToMenu()
        {
            DisableButtons();
            EventManager.MenuRequested?.Invoke();
        }

        private void DisableButtons()
        {
            restartButton.interactable = false;
            menuButton.interactable = false;
            closeButton.interactable = false;
        }
    }
}
