using System;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

namespace Audio
{
    [Serializable]
    public class Sound : ISound
    {
        public AudioClip Clip;

        [Header("Delay")] [Range(0, 1f)] public float Delay;

        public bool IgnoreListenerPause;
        public bool Loop;
        [Range(0, 2f)] public float MaxAdditionalDelay;

        [Range(0f, 0.2f)] public float MaxPitchShift = 0.05f;

        public AudioMixerGroup MixerGroup;

        [Header("Pitch")] [Range(0.5f, 2f)] public float Pitch = 1f;

        public bool RandomizeDelay;
        public bool RandomizePitch;
        [Range(0f, 1.5f)] public float VolumeModifier = 1f;
        
        [Header("Group")]
        public ISoundGroup Group;
        
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