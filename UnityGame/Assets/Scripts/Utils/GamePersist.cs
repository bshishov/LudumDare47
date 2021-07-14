using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{

    [Serializable]
    public class GamePersist : MonoBehaviour
    {
        public List<DataForPersist> _dataForPersist = new List<DataForPersist>();
        public DataForPersist[] _test = new DataForPersist[] { new DataForPersist("t1", 1), new DataForPersist("t2", 2) };

        public void Awake()
        {
            DontDestroyOnLoad(this);
        }

        private void Start()
        {
            if (PlayerPrefs.HasKey("Game State"))
            {
                var json = PlayerPrefs.GetString("Game State");
                var er = JsonUtility.FromJson<DataForPersist>(json);
            }

            for (int i = 0; i < _dataForPersist.Count; i++)
            {
                Debug.Log($"{_dataForPersist[i].key} - {_dataForPersist[i].value}");
            }
        }
        private void OnApplicationQuit()
        {
            var json = JsonUtility.ToJson(new DataForPersist("t2", 2));
            PlayerPrefs.SetString("Game State", json);
            for (int i = 0; i < _dataForPersist.Count; i++)
            {
                Debug.Log($"{_dataForPersist[i].key} - {_dataForPersist[i].value}");
            }
        }
        public void SaveLevelData(string levelName, int starsNumber)
        {
            if (_dataForPersist.Count > 0)
            {
                for (int i = 0; i < _dataForPersist.Count; i++)
                {
                    if (_dataForPersist[i].key == levelName)
                    {
                        if (starsNumber > _dataForPersist[i].value)
                        {
                            _dataForPersist[i].value = starsNumber;
                        }
                    } else
                    {
                        var newData = new DataForPersist(levelName, starsNumber);
                        _dataForPersist.Add(newData);
                    }
                }
            } else
            {
                var newData = new DataForPersist(levelName, starsNumber);
                _dataForPersist.Add(newData);

            }
        }
        public void SavePlayerData(string playerStat, int starsCollected)
        {
        }

        [Serializable]
        public class DataForPersist
        {
            public string key;
            public int value;

            public DataForPersist(string b, int k) {
                key = b;
                value = k;
            }
        }
    }
}
