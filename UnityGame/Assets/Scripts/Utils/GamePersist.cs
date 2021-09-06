using UnityEngine;

namespace Utils
{
    public class GamePersist : Singleton<GamePersist>
    {
        public StringIntDictionary LevelData { get; private set; }
        public StringIntDictionary PlayerData { get; private set; }
        public string LastLevel { get; private set; }

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

        public void ClearAllSaveData() {
            PlayerPrefs.DeleteAll();
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

            if (PlayerPrefs.HasKey("Last Level"))
            {
                LastLevel = PlayerPrefs.GetString("Last Level");
            }
        }

        public void SaveLevelData(string levelName, int starsNumber)
        {
            LastLevel = levelName;

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

            var jsonLevelData = JsonUtility.ToJson(LevelData);
            PlayerPrefs.SetString("Levels Data", jsonLevelData);

            PlayerPrefs.SetString("Last Level", LastLevel);

            
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

            var jsonPlayerData = JsonUtility.ToJson(PlayerData);
            PlayerPrefs.SetString("Player Data", jsonPlayerData);

            PlayerPrefs.Save();
        }
    }

    public class StringIntDictionary : SerializableDictionary<string, int> { }
}
