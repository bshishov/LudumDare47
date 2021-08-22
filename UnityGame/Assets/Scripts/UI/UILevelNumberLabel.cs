using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class UILevelNumberLabel : MonoBehaviour
    {
        public TextMeshProUGUI Label;

        void Start()
        {
            // This is a dirty hack to get level number
            var sceneName = SceneManager.GetActiveScene().name;
            var numberString = Regex.Match(sceneName, @"\d+").Value;
            if (!string.IsNullOrEmpty(numberString))
            {
                Label.text = numberString;
            }
        }
    }
}
