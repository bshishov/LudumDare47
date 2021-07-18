using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class UIMainIconColor : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public Image Icon;
        public Color IconPressedColor;

        private Color _iconUnpressedColor;
        
        private void Start()
        {
            _iconUnpressedColor = Icon.color;
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            Icon.color = IconPressedColor;
        }
        
        public void OnPointerUp(PointerEventData eventData)
        {
            Icon.color = _iconUnpressedColor;
        }
    }
}