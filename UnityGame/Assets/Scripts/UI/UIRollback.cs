using UnityEngine;
using UnityEngine.EventSystems;

public class UIRollback : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
   
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Start click");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("Finish");
    }
}
