using System;
using Attributes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gameplay
{
    [Serializable]
    [CreateAssetMenu(fileName = "levels", menuName = "Level Set", order = 0)]
    public class LevelSet : ScriptableObject
    {
        [Serializable]
        public class Level
        {
            public string SceneName;
            public bool AlwaysUnlocked = true;
            public bool Enabled = true;
        }
        
        public string MenuScene;
        
        [Reorderable]
        public Level[] Levels;

        public string GetNextLevel()
        {
            var currentLevelIndex = GetCurrentLevelIndex();

            // Cant determine current level index (non-indexed scene)
            if (currentLevelIndex < 0)
                return MenuScene;
            
            // Find first enabled level with higher index
            for (var levelIndex = currentLevelIndex + 1; levelIndex < Levels.Length; levelIndex++)
            {
                var level = Levels[levelIndex];
                if (level.Enabled)
                    return level.SceneName;
            }

            // Otherwise - goto menu
            return MenuScene;
        }

        public int GetCurrentLevelIndex()
        {
            var sceneCount = SceneManager.sceneCount;
            for (var activeSceneIndex = 0; activeSceneIndex < sceneCount; activeSceneIndex++)
            {
                var activeSceneName = SceneManager.GetSceneAt(activeSceneIndex).name;
                for (var levelIndex = 0; levelIndex < Levels.Length; levelIndex++)
                {
                    if (Levels[levelIndex].SceneName.Equals(activeSceneName))
                        return levelIndex;
                }
            }

            return -1;
        }
    }
}
