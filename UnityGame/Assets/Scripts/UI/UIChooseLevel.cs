using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class UIChooseLevel : MonoBehaviour
    {
        public TextMeshProUGUI TextNumber;
        private bool _unlocked = false;
        private string _levelName;
        private UICanvasGroupFader _fader;


        public void SetSceneSettings(int number, string levelName, GameObject fader)
        {
            _fader = fader.GetComponent<UICanvasGroupFader>();
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
            _fader.FadeIn();
            _fader.StateChanged += () =>
            {
                if (_fader.State == UICanvasGroupFader.FaderState.FadedIn)
                {
                    SceneManager.LoadScene(_levelName);
                }
            };
           
        }

        [ContextMenu("UnlockLevel")]
        private void UnlockLevel()
        {
            _unlocked = true;
        }

    }
}
