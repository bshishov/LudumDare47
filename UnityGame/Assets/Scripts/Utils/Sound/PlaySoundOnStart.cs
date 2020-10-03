using UnityEngine;

namespace Sounds
{
    public class PlaySoundOnStart : MonoBehaviour
    {
        public Sound Sound;

        private void Start()
        {
            SoundManager.Instance.Play(Sound);
        }
    }
}