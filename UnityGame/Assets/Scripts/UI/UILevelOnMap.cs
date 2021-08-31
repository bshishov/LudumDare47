using Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

namespace UI
{
    public class UILevelOnMap : MonoBehaviour
    {
        public TextMeshProUGUI TextNumber;
        public int LevelNumber;
        public LevelSet Levels;

        public GameObject[] CollectedStars;

        private bool _unlocked = false;
        public string LevelName { get; private set; }

        private void Awake()
        {
            LevelName = Levels.Levels[LevelNumber - 1].SceneName;
            SetSceneSettings(LevelNumber, LevelName);
        }

        private void Start()
        {
            SetCollectedOnLevelStarts();
        }

        private void SetCollectedOnLevelStarts()
        {
            if (GamePersist.Instance.LevelData.ContainsKey(LevelName))
            {
                for (int i = 0; i < GamePersist.Instance.LevelData[LevelName]; i++)
                {
                    CollectedStars[i].SetActive(true);
                }
            }
        }

        public void SetSceneSettings(int number, string levelName)
        {
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
            SceneManager.LoadScene(LevelName);
        }

        [ContextMenu("UnlockLevel")]
        private void UnlockLevel()
        {
            _unlocked = true;
        }

        public void SetLevel(int index, LevelSet.Level level)
        {
            SetSceneSettings(index, level.SceneName);
            if (level.AlwaysUnlocked)
                UnlockLevel();
        }
    }
}
