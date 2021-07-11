using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Audio
{
    public class SoundManager : MonoBehaviour
    {
        public const int MaxSounds = 32;

        private static SoundManager _instance;
        private readonly Dictionary<AudioMixerGroup, int> _groupCounter = new Dictionary<AudioMixerGroup, int>();

        private readonly List<SoundHandler> _handlers = new List<SoundHandler>();
        private readonly List<SoundHandler> _inactiveHandlers = new List<SoundHandler>();
        public MixerGroupLimitSettings LimitSettings;

        public SoundHandler MusicHandler;

        public static SoundManager Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;

                var existing = FindObjectOfType<SoundManager>();
                if (existing != null)
                {
                    Debug.Log("[SoundManager] Reusing");
                    _instance = existing;
                    return existing;
                }

                Debug.Log("[SoundManager] Instantiating");
                var go = new GameObject("[SoundManager]");
                var manager = go.AddComponent<SoundManager>();
                if (manager.LimitSettings == null)
                {
                    Debug.Log("[SoundManager] Loading Limit Settings from resources");
                    manager.LimitSettings = Resources.Load<MixerGroupLimitSettings>("SoundLimitSettings");
                }

                _instance = manager;
                return manager;
            }
        }

        private void Awake()
        {
            if (!Instance.Equals(this))
            {
                Destroy(this);
                return;
            }

            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            foreach (var handler in _handlers)
                if (!handler.IsActive)
                {
                    _inactiveHandlers.Add(handler);
                }
                else
                {
                    if (handler.AttachedTo != null)
                        handler.Source.transform.position = handler.AttachedTo.transform.position;
                }

            foreach (var handler in _inactiveHandlers)
            {
                var sourceGroup = handler.Source.outputAudioMixerGroup;
                if (sourceGroup != null && _groupCounter.ContainsKey(sourceGroup))
                    _groupCounter[sourceGroup] -= 1;
                Destroy(handler.Source.gameObject);
                _handlers.Remove(handler);
            }

            _inactiveHandlers.Clear();
        }

        public SoundHandler Play(
            AudioClip clip, 
            float volume = 1f, 
            bool loop = false, 
            float pitch = 1f,
            bool ignoreListenerPause = false, 
            float delay = 0f, 
            AudioMixerGroup mixerGroup = null)
        {
            if (clip == null)
                return null;

            if (_handlers.Count >= MaxSounds)
            {
                Debug.Log("[SoundManager] Too many sounds");
                return null;
            }

            var go = new GameObject(string.Format("Sound: {0}", clip.name));
            go.transform.SetParent(transform);
            var source = go.AddComponent<AudioSource>();

            source.clip = clip;
            source.priority = 128;
            source.playOnAwake = false;
            source.volume = volume;
            source.loop = loop;
            source.pitch = pitch;
            source.outputAudioMixerGroup = mixerGroup;
            source.ignoreListenerPause = ignoreListenerPause;

            var sound = new SoundHandler(source);
            _handlers.Add(sound);

            if (delay > 0)
                source.PlayDelayed(delay);
            else
                source.Play();

            return sound;
        }

        public SoundHandler Play(AudioClipWithVolume clip, bool loop = false, float pitch = 1f,
            bool ignoreListenerPause = false, float delay = 0f)
        {
            if (clip == null)
                return null;
            return Play(clip.Clip, clip.VolumeModifier, loop, clip.Pitch * pitch, ignoreListenerPause, delay);
        }

        public SoundHandler Play(ISound sound)
        {
            if (sound == null)
                return null;

            var mixerGroup = sound.GetMixedGroup();
            var isLimiting = mixerGroup != null && LimitSettings != null;
            var isInGroup = false;
            var soundsInGroup = 0;

            if (isLimiting)
            {
                isInGroup = _groupCounter.ContainsKey(mixerGroup);
                if (isInGroup)
                {
                    soundsInGroup = _groupCounter[mixerGroup];
                    if (soundsInGroup >= LimitSettings.GetLimit(mixerGroup))
                        //Debug.Log(string.Format("[SoundManager] Too many sounds for group {0}", sound.MixerGroup));
                        return null;
                }
            }

            var pitch = sound.GetPitch();
            var delay = sound.GetDelay();
            var clip = sound.GetAudioClip();

            var handler = Play(
                clip, 
                sound.GetVolumeModifier(), 
                sound.IsOnLoop(), 
                pitch, 
                sound.ShouldIgnoreListenerPause(), 
                delay,
                mixerGroup
            );

            if (isLimiting && handler != null)
            {
                if (isInGroup)
                    // There are sounds in group so increment by one
                    _groupCounter[mixerGroup] = soundsInGroup + 1;
                else
                    // First sound in group
                    _groupCounter.Add(mixerGroup, 1);
            }

            return handler;
        }

        public SoundHandler Play(ISound sound, Transform attachTo)
        {
            var s = Play(sound);
            s?.AttachToObject(attachTo);
            return s;
        }

        public SoundHandler PlayMusic(Sound sound)
        {
            if (sound == null)
                return null;

            if (sound.Clip == null)
                return null;


            if (MusicHandler != null)
            {
                MusicHandler.Source.clip = sound.Clip;
                //MusicHandler.Volume = clip.VolumeModifier;
                MusicHandler.Source.volume = sound.VolumeModifier;
                MusicHandler.IsLooped = sound.Loop;
                MusicHandler.Pitch = sound.Pitch;
                MusicHandler.Source.Play();
                MusicHandler.MixerGroup = sound.MixerGroup;
                return MusicHandler;
            }

            var handler = Play(sound);
            DontDestroyOnLoad(handler.Source.gameObject);
            MusicHandler = handler;
            return handler;
        }

        private void OnGUI()
        {
#if DEBUG
            /*
        if(_groupCounter == null)
            return;
        
        var h = 10;
        foreach (var kvp in _groupCounter)
        {
            GUI.Label(new Rect(10, h, 400, 20), string.Format("{0}: {1}", kvp.Key.name, kvp.Value));
            h += 20;
        }
        */
#endif
        }

        public class SoundHandler
        {
            public SoundHandler(AudioSource source)
            {
                Source = source;
            }

            public AudioSource Source { get; }

            public bool IsLooped
            {
                get => Source.loop;
                set
                {
                    if (IsActive) Source.loop = value;
                }
            }

            public float Pitch
            {
                get => Source.pitch;
                set
                {
                    if (IsActive) Source.pitch = value;
                }
            }

            public float Volume
            {
                get => Source.volume;
                set
                {
                    if (IsActive) Source.volume = value;
                }
            }

            public bool IgnoreListenerPause
            {
                get => Source.ignoreListenerPause;
                set
                {
                    if (IsActive) Source.ignoreListenerPause = value;
                }
            }

            public AudioMixerGroup MixerGroup
            {
                get => Source.outputAudioMixerGroup;
                set
                {
                    if (IsActive) Source.outputAudioMixerGroup = value;
                }
            }

            public bool IsActive
            {
                get
                {
                    if (Source == null)
                        return false;
                    if (Source.clip != null)
                        if (Source.clip.loadState == AudioDataLoadState.Loading)
                            return true;
                    return Source.isPlaying;
                }
            }

            public Transform AttachedTo { get; private set; }

            public void Stop()
            {
                if (Source != null)
                    Source.Stop();
            }

            public void AttachToObject(Transform transform1,
                float spatialBlend = 0.7f,
                float minDistance = 15f,
                float maxDistance = 40f)
            {
                AttachedTo = transform1;
                Source.spatialBlend = spatialBlend;
                Source.rolloffMode = AudioRolloffMode.Linear;
                Source.minDistance = minDistance;
                Source.maxDistance = maxDistance;
                Source.dopplerLevel = 0f;
            }
        }
    }
}