namespace Audio
{
    public static class MusicManager
    {
        private static SoundManager.SoundHandler _activeMusicHandler;
        private static Sound _activeMusic;
        
        public static void Play(Sound sound)
        {
            if (_activeMusic != sound)
            {
                _activeMusic = sound;
                SoundManager.Instance.PlayMusic(sound);
            }
        }
    }
}