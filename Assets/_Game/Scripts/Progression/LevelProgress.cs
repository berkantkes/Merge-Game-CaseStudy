using UnityEngine;

namespace MergeGame.Progression
{
    public class LevelProgress
    {
        public int DisplayLevel { get; } = 1;
        public int LoopLevel { get; } = 1;

        public LevelProgress(int displayLevel, int levelCount)
        {
            if (displayLevel < 1)
            {
                Debug.LogError("Level must be at least 1.");
                return;
            }

            if (levelCount < 1)
            {
                Debug.LogError("Level count must be at least 1.");
                return;
            }

            DisplayLevel = displayLevel;
            LoopLevel = (displayLevel - 1) % levelCount + 1;
        }
    }
}
