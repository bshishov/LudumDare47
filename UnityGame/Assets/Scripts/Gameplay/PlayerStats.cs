using Utils;
using UnityEngine;
using System;

namespace Gameplay
{
    public class PlayerStats : Singleton<PlayerStats>
    {
        public int NumberOfRollback { get; private set; }
        public int TotalNumberOfStars { get; private set; }

        [SerializeField]
        private GamePersist _gamePersist;

        private const string StarsColectedKey = "Player Stars";
        private const string RollbackKey = "Player Rollbacks";

        private void Start()
        {
            DontDestroyOnLoad(this);
            Load();
        }

        private void Load()
        {
            if (_gamePersist.PlayerData.ContainsKey(StarsColectedKey)) {
                TotalNumberOfStars = _gamePersist.PlayerData[StarsColectedKey];
            }

            if (_gamePersist.PlayerData.ContainsKey(RollbackKey))
            {
                NumberOfRollback = _gamePersist.PlayerData[RollbackKey];
            } else {
                //start number of rollback
                NumberOfRollback = 250;
            }

        }

        public void AddStars(int stars)
        {
            TotalNumberOfStars += stars;
            Save(StarsColectedKey, TotalNumberOfStars);
        }

        public void AddRollbackNumber(int rollback)
        {
            NumberOfRollback += rollback;
            Save(RollbackKey, NumberOfRollback);
        }

        public void RemoveRollbackNumber()
        {
            NumberOfRollback--;
            Save(RollbackKey, NumberOfRollback);
        }
        private void Save(string key, int value) {
            if (_gamePersist != null)
            {
                _gamePersist.SavePlayerData(key, value);
            }
        }
    }
}