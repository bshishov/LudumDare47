namespace Audio
{
    public interface ISoundGroup
    {
        int GetMaxConcurrentSounds();

        string GetId();
    }
}