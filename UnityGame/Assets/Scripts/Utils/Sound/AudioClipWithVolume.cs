using System;
using UnityEngine;

[Serializable]
public class AudioClipWithVolume
{
    public AudioClip Clip;

    public float Pitch = 1f;

    [Range(0f, 1.5f)] public float VolumeModifier = 1f;
}