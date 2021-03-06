﻿using UnityEngine;

namespace Utils
{
    public class GamePersist : Singleton<GamePersist>
    {
        public StringIntDictionary LevelData { get; private set; }
        public StringIntDictionary PlayerData { get; private set; }

        public void Awake()
        {
            GamePersist[] objs = FindObjectsOfType<GamePersist>();

            if (objs.Length > 1)
            {
                Destroy(this.gameObject);
            }

            DontDestroyOnLoad(this);
            Load();
        }

        private void Load()
        {
            LevelData = new StringIntDictionary();
            PlayerData = new StringIntDictionary();

            if (PlayerPrefs.HasKey("Levels Data"))
            {
                var json = PlayerPrefs.GetString("Levels Data");
                LevelData = JsonUtility.FromJson<StringIntDictionary>(json);
            }

            if (PlayerPrefs.HasKey("Player Data"))
            {
                var json = PlayerPrefs.GetString("Player Data");
                PlayerData = JsonUtility.FromJson<StringIntDictionary>(json);
            }
        }

        private void Save()
        {
            var jsonLevelData = JsonUtility.ToJson(LevelData);
            PlayerPrefs.SetString("Levels Data", jsonLevelData);

            var jsonPlayerData = JsonUtility.ToJson(PlayerData);
            PlayerPrefs.SetString("Player Data", jsonPlayerData);

            PlayerPrefs.Save();
        }

        public void SaveLevelData(string levelName, int starsNumber)
        {
            if (!LevelData.ContainsKey(levelName))
            {
                LevelData.Add(levelName, starsNumber);
            } else
            {
                //Player collected more starts than the first time 
                if (starsNumber > LevelData[levelName])
                {
                    LevelData[levelName] = starsNumber;
                }
            }

            Save();
        }

        public void SavePlayerData(string playerStat, int starsCollected)
        {
            if (!PlayerData.ContainsKey(playerStat))
            {
                PlayerData.Add(playerStat, starsCollected);
            } else
            {
                PlayerData[playerStat] = starsCollected;
            }

            Save();
        }
    }

    public class StringIntDictionary : SerializableDictionary<string, int> { }
}
