using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using Utils;

namespace UI
{
    public class UISoundSettings : MonoBehaviour
    {
        public Image SoundOn;
        public Image SoundOff;
        public bool SoundStatus { get; private set; }
        
        [Header("Volume Controls")]
        public AudioMixer AudioMixer;

        private Button _soundButton;

        private const string SoundStatusKey = "Player sound";
        private void Start()
        {
            _soundButton = GetComponent<Button>();
            _soundButton.onClick.AddListener(ChangeSoundStatus);

            LoadSoundStatus();
        }

        private void ChangeSoundStatus()
        {
            SoundStatus = !SoundStatus;
            UpdateSoundIcon();
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

            UpdateSoundIcon();
        }

        private void UpdateSoundIcon()
        {
            SoundOn.enabled = SoundStatus;
            SoundOff.enabled = !SoundStatus;

            if (SoundStatus)
            {
                SetVolumeLinear(1);
            }
            else
            {
                SetVolumeLinear(0);
            }
        }
        
        private void Save(string key, int value)
        {
            if (GamePersist.Instance != null)
            {
                GamePersist.Instance.SavePlayerData(key, value);
            }
        }
        
        private void SetVolumeLinear(float volume)
        {
            if (AudioMixer == null)
                return;

            var level = -80f;
            if (volume > 0)
                level = Mathf.Log(Mathf.Clamp01(volume)) * 20;

            AudioMixer.SetFloat("Volume", level);
        }
    }
}
