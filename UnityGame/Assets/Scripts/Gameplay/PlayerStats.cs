using Utils;

namespace Gameplay
{
    public class PlayerStats : Singleton<PlayerStats>
    {
        public int NumberOfRollback = 2500;
        public int TotalNumberOfStars = 0;

        public bool SoundStatus = true;
        private void Start()
        {
            DontDestroyOnLoad(this);
        }

        public void AddStars(int stars)
        {
            TotalNumberOfStars += stars;
        }
    }
}