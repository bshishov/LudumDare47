using UnityEngine;
using UnityEngine.Audio;

namespace Audio
{
    [CreateAssetMenu(fileName = "Sound", menuName = "Sound", order = 0)]
    public class SoundAsset : ScriptableObject, ISound
    {
        public AudioClip Clip;
        public bool Loop;
        public AudioMixerGroup MixerGroup;
        [Range(0f, 1.5f)] public float VolumeModifier = 1f;

        [Header("Delay")] 
        [Range(0, 1f)] public float Delay;
        public bool RandomizeDelay;
        [Range(0, 2f)] public float MaxAdditionalDelay;

        [Header("Other")] 
        public bool IgnoreListenerPause;
        public SoundGroup Group;

        [Header("Pitch")] 
        [Range(0.5f, 2f)] public float Pitch = 1f;
        public bool RandomizePitch;
        [Range(0f, 0.2f)] public float MaxPitchShift = 0.05f;

        public AudioClip GetAudioClip()
        {
            return Clip;
        }

        public float GetPitch()
        {
            if (RandomizePitch)
                return Random.Range(Pitch - MaxPitchShift, Pitch + MaxPitchShift);
            return Pitch;
        }

        public float GetDelay()
        {
            if(RandomizeDelay)
                return Delay + Random.value * MaxAdditionalDelay;
            return Delay;
        }

        public AudioMixerGroup GetMixedGroup()
        {
            return MixerGroup;
        }

        public bool IsOnLoop()
        {
            return Loop;
        }

        public float GetVolumeModifier()
        {
            return VolumeModifier;
        }

        public bool ShouldIgnoreListenerPause()
        {
            return IgnoreListenerPause;
        }

        public ISoundGroup GetGroup()
        {
            return Group;
        }
    }
}