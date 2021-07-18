using UnityEngine;
using UnityEngine.UI;

namespace UI.Buttons
{
    [RequireComponent(typeof(Button))]
    public class UINextLevelButton : MonoBehaviour
    {
        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(OnButtonPressed);
        }

        private void OnButtonPressed()
        {
            FindObjectOfType<UILoad>().LoadNext();
        }
    }
}