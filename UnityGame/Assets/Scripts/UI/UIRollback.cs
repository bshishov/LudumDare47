using Gameplay;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class UIRollback : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private bool _buttonPressed;

        public void OnPointerDown(PointerEventData eventData)
        {
            _buttonPressed = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _buttonPressed = false;
        }

        private void Update() 
        {
            if (_buttonPressed && Common.CurrentLevel != null) {
                Common.CurrentLevel.PlayerRollback();
            }
        }
    }
}
