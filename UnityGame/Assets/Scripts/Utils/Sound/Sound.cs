using System;
using UnityEngine;
using UnityEngine.Audio;

[Serializable]
public class Sound
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
}