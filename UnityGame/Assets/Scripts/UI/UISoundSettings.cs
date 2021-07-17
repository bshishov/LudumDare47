using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace UI
{
    public class UISoundSettings : MonoBehaviour
    {
        public Image SoundOn;
        public Image SoundOff;
        public bool SoundStatus { get; private set; }

        private Button _soundButton;

        private const string SoundStatusKey = "Player sound";
        private void Start()
        {
            _soundButton = GetComponent<Button>();
            _soundButton.onClick.AddListener(ChangeSoundStatus);

            LoadSoundStatus();
        }

        public void ChangeSoundStatus()
        {
            SoundStatus = !SoundStatus;
            SoundIconChange();
            Save(SoundStatusKey, SoundStatus ? 1 : 0);
        }

        private void LoadSoundStatus()
        {
            if (GamePersist.Instance.PlayerData.ContainsKey(SoundStatusKey))
            {
                SoundStatus = (GamePersist.Instance.PlayerData[SoundStatusKey] == 1 ? true : false);
            } else {
                SoundStatus = true;
            }

            SoundIconChange();
        }

        private void SoundIconChange()
        {
            SoundOn.enabled = SoundStatus;
            SoundOff.enabled = !SoundStatus;
        }
        private void Save(string key, int value)
        {
            if (GamePersist.Instance != null)
            {
                GamePersist.Instance.SavePlayerData(key, value);
            }
        }
    }
}
