using UnityEngine;
using Gameplay;
using UnityEngine.UI;

namespace UI
{
    public class UISoundSettings : MonoBehaviour
    {
        public Image SoundOn;
        public Image SoundOff;

        private Button _soundButton;
        private void Start()
        {
            _soundButton = GetComponent<Button>();
            _soundButton.onClick.AddListener(ChangeSound);

            SoundIconChange();
        }

        private void SoundIconChange()
        {
            if (PlayerStats.Instance.SoundStatus)
            {
                SoundOn.enabled = true;
                SoundOff.enabled = false;
            } else
            {
                SoundOn.enabled = false;
                SoundOff.enabled = true;
            }
        }

        private void ChangeSound()
        {
            PlayerStats.Instance.SoundStatus = !PlayerStats.Instance.SoundStatus;
            SoundIconChange();
        }
    }
}
