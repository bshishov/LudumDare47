using Utils;
using UnityEngine;

namespace Gameplay
{
    public class PlayerStats : Singleton<PlayerStats>
    {
        public int NumberOfRollback { get; private set; }
        public int TotalNumberOfStars { get; private set; }
        public bool SoundStatus { get; private set; }

        [SerializeField]
        private GamePersist _gamePersist;

        private const string StarsColectedKey = "Player Stars";
        private const string RollbackKey = "Player Rollbacks";
        private const string SoundStatusKey = "Player sound";

        private void Start()
        {
            DontDestroyOnLoad(this);
            NumberOfRollback = 2500;
        }

        public void AddStars(int stars)
        {
            TotalNumberOfStars += stars;
            _gamePersist.SavePlayerData(StarsColectedKey, TotalNumberOfStars);
        }

        public void AddRollbackNumber(int rollback) {
            NumberOfRollback += rollback;
            _gamePersist.SavePlayerData(RollbackKey, NumberOfRollback);
        }

        public void RemoveRollbackNumber()
        {
            NumberOfRollback--;
            _gamePersist.SavePlayerData(RollbackKey, NumberOfRollback);
        }

        public void ChangeSoundStatus() {

            SoundStatus = !SoundStatus;
            _gamePersist.SavePlayerData(SoundStatusKey, SoundStatus? 1:0);
        }
    }
}