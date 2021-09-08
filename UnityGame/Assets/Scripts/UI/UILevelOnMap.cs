using Gameplay;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utils;

namespace UI
{
    public class UILevelOnMap : MonoBehaviour
    {
        public TextMeshProUGUI TextNumber;
        public int LevelNumber;
        public LevelSet Levels;
        public Button LevelButton;

        public Image DisableLevel;
        public Image EnableLevel;

        public GameObject PlayerPointer;

        public GameObject[] CollectedStars;
        public string LevelName { get; private set; }

        private bool _unlocked = false;
        private string _previousLevelName;

        private void Awake()
        {
            LevelName = Levels.Levels[LevelNumber - 1].SceneName;
            SetSceneSettings(LevelNumber, LevelName);
        }

        private void Start()
        {
            CheckUnlockedLevel();
            SetPlayerPointerActive();
            SetCollectedOnLevelStarts();
        }

        private void SetPlayerPointerActive()
        {

            if (PlayerPrefs.HasKey("Last Level"))
            {
                if (GamePersist.Instance.LastLevel == LevelName)
                {
                    PlayerPointer.SetActive(true);
                }
            }
            else if (LevelNumber == 1)
            {
                PlayerPointer.SetActive(true);
            }
        }

        private void CheckUnlockedLevel()
        {
            if (LevelNumber - 2 >= 0)
            {
                _previousLevelName = Levels.Levels[LevelNumber - 2].SceneName;
                if (GamePersist.Instance.LevelData.ContainsKey(_previousLevelName) || GamePersist.Instance.LevelData.ContainsKey(LevelName))
                {
                    UnlockLevel();
                }
            }
            else
            {
                UnlockLevel();
            }
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
            DisableLevel.enabled = false;
            EnableLevel.enabled = true;
            LevelButton.onClick.AddListener(LoadLevel);
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
