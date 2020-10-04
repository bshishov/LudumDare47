using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class UIChooseLevel : MonoBehaviour
    {
        public Text TextNumber;
        private bool _unlocked = false;
        private string _levelName;
        

        private void Start()
        {
           
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
            if (_unlocked)
                SceneManager.LoadScene(_levelName);
        }
    }
}
