using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "MixerGroupLimitSettings", menuName = "Audio/Mixer Group Limit Settings")]
public class MixerGroupLimitSettings : ScriptableObject
{
    private Dictionary<AudioMixerGroup, int> _limits;
    public int DefaultLimit;

    public MixerGroupLimitSettingsEntry[] Settings;

    public int GetLimit(AudioMixerGroup mixerGroup)
    {
        // LazyLoading limits
        if (_limits == null)
        {
            _limits = new Dictionary<AudioMixerGroup, int>();
            foreach (var entry in Settings) _limits.Add(entry.MixerGroup, entry.Limit);
        }

        int limit;
        if (_limits.TryGetValue(mixerGroup, out limit)) return limit;
        return DefaultLimit;
    }

    [Serializable]
    public class MixerGroupLimitSettingsEntry
    {
        public int Limit = 10;
        public AudioMixerGroup MixerGroup;
    }
}