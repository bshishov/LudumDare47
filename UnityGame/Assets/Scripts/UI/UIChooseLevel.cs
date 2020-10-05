using Assets.Scripts.Utils.UI;
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
            Debug.Log("Here");
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
