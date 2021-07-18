using Gameplay;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class UILoad : MonoBehaviour
    {
        public LevelSet Levels;

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
            LoadLevel(Levels.MenuScene);
        }

        public void LoadNext()
        {
            LoadLevel(Levels.GetNextLevel());
        }
    }
}
