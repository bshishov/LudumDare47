using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public class GamePersist : MonoBehaviour
    {
        private Dictionary<string, int> _levelData = new Dictionary<string, int>();
        private Dictionary<string, int> _playerData = new Dictionary<string, int>();
        public void Awake()
        {
            DontDestroyOnLoad(this);
        }

        private void OnEnable()
        {
        }
        private void OnDisable()
        {
        }
        public void SaveLevelData(string levelName, int starsNumber)
        {
            if (!_levelData.ContainsKey(levelName))
            {
                _levelData.Add(levelName, starsNumber);
            } else
            {

                //Player collected more starts than the first time 
                if (starsNumber > _levelData[levelName])
                {
                    _levelData[levelName] = starsNumber;
                }
            }
        }
        public void SavePlayerData(string playerStat, int starsCollected)
        {
            if (!_playerData.ContainsKey(playerStat))
            {
                _playerData.Add(playerStat, starsCollected);
            } else
            {
                _playerData[playerStat] = starsCollected;
            }

            foreach (var item in _playerData)
            {
                Debug.Log(item.Value + " " + item.Key);
            }
        }
    }
}
