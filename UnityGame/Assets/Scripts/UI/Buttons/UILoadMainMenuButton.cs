using UnityEngine;
using UnityEngine.UI;

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
            FindObjectOfType<UILoad>().LoadMenu();
        }
    }
}