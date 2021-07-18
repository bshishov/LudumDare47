using UnityEngine;
using UnityEngine.UI;
using Gameplay;

namespace UI
{
    public class UIStarCount : MonoBehaviour
    {
        private Text _numberOfStarText;
        private PlayerStats _playerStats;

        private void Start()
        {
            _numberOfStarText = GetComponent<Text>();
            _playerStats = PlayerStats.Instance;
            
            Common.LevelStarCollected += OnLevelStarCollected;
        }

        private void OnDestroy()
        {
            Common.LevelStarCollected -= OnLevelStarCollected;
        }

        private void OnLevelStarCollected(Level level)
        {
            _numberOfStarText.text = "Total starts - " + _playerStats.TotalNumberOfStars + "\n" + "On level stars - " + level.CollectedStars;
        }
    }
}