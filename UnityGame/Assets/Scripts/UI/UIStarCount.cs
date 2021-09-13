using UnityEngine;
using UnityEngine.UI;
using Gameplay;
using TMPro;

namespace UI
{
    public class UIStarCount : MonoBehaviour
    {
        public TextMeshProUGUI Label;
        private PlayerStats _playerStats;

        private void Start()
        {
            _playerStats = PlayerStats.Instance;

            Label.text = _playerStats.TotalNumberOfStars.ToString();
        }

    }
}