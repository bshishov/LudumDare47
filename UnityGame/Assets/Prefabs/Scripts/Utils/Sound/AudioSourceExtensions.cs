using UnityEngine;

public static class AudioSourceExtensions
{
    public static void PlayClip(this AudioSource audioSource, AudioClipWithVolume clip, float additionalModifier = 1f)
    {
        if (clip == null || clip.Clip == null || clip.VolumeModifier < 1e-4)
            return;

        if (audioSource == null)
            return;

        audioSource.PlayOneShot(clip.Clip, clip.VolumeModifier * additionalModifier);
    }
}