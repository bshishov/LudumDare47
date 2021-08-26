using Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class UIChooseLevel : MonoBehaviour
    {
        public TextMeshProUGUI TextNumber;
        public int LevelNumber;
        public LevelSet Levels;

        private bool _unlocked = false;
        private string _levelName;

        private void Start()
        {
            SetSceneSettings(LevelNumber, Levels.Levels[LevelNumber-1].SceneName);
        }

        public void SetSceneSettings(int number, string levelName)
        {
            _levelName = levelName;
            TextNumber.text = number.ToString();
            if (PlayerPrefs.GetInt(string.Format(levelName), 0) == 1)
            {
                _unlocked = true;
                // VISUALS
            }
            else
            {
                // VISUALS
            }

        }

        public void LoadLevel()
        {
            SceneManager.LoadScene(_levelName);
        }

        [ContextMenu("UnlockLevel")]
        private void UnlockLevel()
        {
            _unlocked = true;
        }

        public void SetLevel(int index, LevelSet.Level level)
        {
            SetSceneSettings(index, level.SceneName);
            if(level.AlwaysUnlocked)
                UnlockLevel();
        }
    }
}
