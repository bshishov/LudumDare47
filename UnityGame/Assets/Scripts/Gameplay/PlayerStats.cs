using Utils;
using UnityEngine;
using System;

namespace Gameplay
{
    public class PlayerStats : Singleton<PlayerStats>
    {
        public int NumberOfRollback { get; private set; }
        public int TotalNumberOfStars { get; private set; }

        private const string StarsColectedKey = "Player Stars";
        private const string RollbackKey = "Player Rollbacks";

        private void Start()
        {
            PlayerStats[] objs = FindObjectsOfType<PlayerStats>();

            if (objs.Length > 1)
            {
                Destroy(this.gameObject);
            }

            DontDestroyOnLoad(this);


            Load();

        }

        private void Load()
        {
            if (GamePersist.Instance.PlayerData.ContainsKey(StarsColectedKey))
            {
                TotalNumberOfStars = GamePersist.Instance.PlayerData[StarsColectedKey];
            }

            if (GamePersist.Instance.PlayerData.ContainsKey(RollbackKey))
            {
                NumberOfRollback = GamePersist.Instance.PlayerData[RollbackKey];
            } else
            {
                //start number of rollback
                NumberOfRollback = 99999;
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
        private void Save(string key, int value)
        {
            if (GamePersist.Instance != null)
            {
                GamePersist.Instance.SavePlayerData(key, value);
            }
        }
    }
}