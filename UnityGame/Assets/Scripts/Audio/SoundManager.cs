using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Audio
{
    public class SoundManager : MonoBehaviour
    {
        public const int MaxSounds = 32;

        private static SoundManager _instance;
        private readonly Dictionary<string, int> _groupCounter = new Dictionary<string, int>();
        private readonly List<SoundHandler> _handlers = new List<SoundHandler>();
        private readonly List<SoundHandler> _inactiveHandlers = new List<SoundHandler>();
        private SoundHandler _musicHandler;

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
                if (handler.Group != null)
                {
                    var groupId = handler.Group.GetId();
                    if (_groupCounter.ContainsKey(groupId))
                        _groupCounter[groupId] -= 1;
                }

                Destroy(handler.Source.gameObject);
                _handlers.Remove(handler);
            }

            _inactiveHandlers.Clear();
        }

        private SoundHandler Play(
            AudioClip clip, 
            float volume = 1f, 
            bool loop = false, 
            float pitch = 1f,
            bool ignoreListenerPause = false, 
            float delay = 0f, 
            AudioMixerGroup mixerGroup = null,
            ISoundGroup @group = null)
        {
            if (clip == null)
                return null;

            if (_handlers.Count >= MaxSounds)
            {
                Debug.Log("[SoundManager] Too many sounds");
                return null;
            }

            var go = new GameObject($"Sound: {clip.name}");
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

            var sound = new SoundHandler(source) {Group = @group};
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
            var soundGroup = sound.GetGroup();
            var soundsInGroup = 0;

            if (soundGroup != null)
            {
                var groupId = soundGroup.GetId();
                if (_groupCounter.ContainsKey(groupId))
                {
                    soundsInGroup = _groupCounter[groupId];
                    if (soundsInGroup >= soundGroup.GetMaxConcurrentSounds())
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
                mixerGroup,
                sound.GetGroup()
            );

            if (soundGroup != null && handler != null)
            {
                var groupId = soundGroup.GetId();
                if (_groupCounter.ContainsKey(groupId))
                    // There are sounds in group so increment by one
                    _groupCounter[groupId] = soundsInGroup + 1;
                else
                    // First sound in group
                    _groupCounter.Add(groupId, 1);
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


            if (_musicHandler != null)
            {
                _musicHandler.Source.clip = sound.Clip;
                //MusicHandler.Volume = clip.VolumeModifier;
                _musicHandler.Source.volume = sound.VolumeModifier;
                _musicHandler.IsLooped = sound.Loop;
                _musicHandler.Pitch = sound.Pitch;
                _musicHandler.Source.Play();
                _musicHandler.MixerGroup = sound.MixerGroup;
                return _musicHandler;
            }

            var handler = Play(sound);
            DontDestroyOnLoad(handler.Source.gameObject);
            _musicHandler = handler;
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
            GUI.Label(new Rect(10, h, 400, 20), $"{kvp.Key}: {kvp.Value}");
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
            
            public ISoundGroup Group { get; set; }

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