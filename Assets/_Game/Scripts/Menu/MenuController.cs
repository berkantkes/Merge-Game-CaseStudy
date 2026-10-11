using MergeGame.Progression;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MergeGame.Menu
{
    public class MenuController : MonoBehaviour
    {
        [SerializeField] private TMP_Text levelText;

        private void OnEnable()
        {
            if (levelText == null)
            {
                Debug.LogError("Level text is missing.");
                return;
            }

            var progress = new LevelProgressStore().Load();
            levelText.text = "LEVEL " + progress.DisplayLevel;
        }

        public void Play()
        {
            SceneManager.LoadScene("Game");
        }
    }
}
