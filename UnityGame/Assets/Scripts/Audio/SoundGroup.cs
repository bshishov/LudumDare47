using UnityEngine;

namespace Audio
{
    [CreateAssetMenu(fileName = "Sound Group", menuName = "Audio/Sound Group")]
    public class SoundGroup : ScriptableObject, ISoundGroup
    {
        public int MaxSoundsInGroup;
        
        public int GetMaxConcurrentSounds()
        {
            return MaxSoundsInGroup;
        }

        public string GetId()
        {
            return name;
        }
    }
}