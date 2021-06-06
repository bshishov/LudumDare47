using Gameplay;
using UnityEngine;
using UnityEngine.UI;

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
            _numberOfStarText.text = "Stars on level - " + GameSettings.NumberOfStarsOnLevel + "\n" + "Total starts - " + GameSettings.TotalNumberOfStars;
        }
    }
}