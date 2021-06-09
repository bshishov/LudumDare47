using UnityEngine;
using UnityEngine.UI;
using Gameplay;

namespace UI
{
    public class UIStarCount : MonoBehaviour
    {
        private Text _numberOfStarText;

        private void Start()
        {
            _numberOfStarText = GetComponent<Text>();
        }

        private void Update()
        {
            _numberOfStarText.text = "Total starts - " + PlayerStats.Instance.TotalNumberOfStars + "\n" + "On level stars - " + Level.Instance.CollectedStars;
        }
    }
}