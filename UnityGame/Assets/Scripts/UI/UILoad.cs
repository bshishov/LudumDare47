using Gameplay;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class UILoad : MonoBehaviour
    {
        public LevelSet Levels;

        public bool IsLoadFromGameFrame { get; private set; }

        private void Start()
        {
            Time.timeScale = 1f;
        }

        private void LoadLevel(string levelString)
        {
            SceneManager.LoadScene(levelString);
        }

        public void LoadMenu()
        {
            IsLoadFromGameFrame = true;
            LoadLevel(Levels.MenuScene);
        }

        public void LoadNext()
        {
            LoadLevel(Levels.GetNextLevel());
        }
    }
}
