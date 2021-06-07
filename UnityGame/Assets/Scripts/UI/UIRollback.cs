using Gameplay;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIRollback : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool _buttonPressed = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        _buttonPressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _buttonPressed = false;
    }

    private void Update() {

        if (_buttonPressed) {
            Level.Instance.PlayerRollback();
        }
    
    }
}
