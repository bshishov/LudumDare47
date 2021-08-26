using UnityEngine;
using UnityEngine.UI;
using Gameplay;

namespace UI.Buttons
{
    [RequireComponent(typeof(Button))]
    public class UILoadMainMenuButton : MonoBehaviour
    {
        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(OnButtonPressed);
        }

        private void OnButtonPressed()
        {

            Common.OnLevelRestart();
            FindObjectOfType<UILoad>().LoadMenu();
        }
    }
}