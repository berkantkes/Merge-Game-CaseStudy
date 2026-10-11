using UnityEngine;

namespace MergeGame.Progression
{
    public class LevelProgressStore
    {
        private const string CurrentLevelKey = "MergeGame.Progression.CurrentLevel";
        public const int DefaultLevelCount = 5;

        private readonly int levelCount = DefaultLevelCount;

        public LevelProgressStore(int levelCount = DefaultLevelCount)
        {
            if (levelCount < 1)
            {
                Debug.LogError("Level count must be at least 1.");
                return;
            }

            this.levelCount = levelCount;
        }

        public LevelProgress Load()
        {
            int savedLevel = PlayerPrefs.GetInt(CurrentLevelKey, 1);
            return new LevelProgress(Mathf.Max(1, savedLevel), levelCount);
        }

        public LevelProgress Save(int displayLevel)
        {
            if (displayLevel < 1)
            {
                Debug.LogError("Level must be at least 1.");
                return Load();
            }

            var progress = new LevelProgress(displayLevel, levelCount);

            PlayerPrefs.SetInt(CurrentLevelKey, progress.DisplayLevel);
            PlayerPrefs.Save();

            return progress;
        }

        public LevelProgress AdvanceLevel()
        {
            var progress = Load();
            if (progress.DisplayLevel == int.MaxValue)
            {
                Debug.LogError("Level limit reached.");
                return progress;
            }

            return Save(progress.DisplayLevel + 1);
        }

        public LevelProgress ResetProgress()
        {
            return Save(1);
        }
    }
}
