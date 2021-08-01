using UnityEngine;
using UnityEngine.Audio;

namespace Audio
{
    public interface ISound
    {
        AudioClip GetAudioClip();
        float GetPitch();
        float GetDelay();
        AudioMixerGroup GetMixedGroup();
        bool IsOnLoop();
        float GetVolumeModifier();
        bool ShouldIgnoreListenerPause();
        ISoundGroup GetGroup();
    }
}