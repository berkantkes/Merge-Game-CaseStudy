using MergeGame.Progression;
using MergeGame.Loading;
using TMPro;
using UnityEngine;

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
            SceneLoader.Load(Constants.SceneNames.Game);
        }
    }
}
