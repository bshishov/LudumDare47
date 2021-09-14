using Audio;
using UnityEngine;

namespace UI
{
    public class UIMenuMusic : MonoBehaviour
    {
        public Sound MenuMusic;
        void Start() => MusicManager.Play(MenuMusic);
    }
}
