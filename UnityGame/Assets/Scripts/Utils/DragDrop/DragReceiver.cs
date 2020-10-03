using UnityEngine;
using UnityEngine.EventSystems;

namespace Utils.DragDrop
{
    public class DragReceiver<T> : MonoBehaviour,
        IPointerEnterHandler,
        IPointerExitHandler
        where T : class, IDragData
    {
        public void OnPointerEnter(PointerEventData eventData)
        {
            var data = Dragger.DraggedData as T;
            if (data == null)
                return;

            if (CanReceive(data))
            {
                var dragger = data.DraggedBy as Dragger<T>;
                if (dragger != null)
                    dragger.Receiver = this;
                DraggingOverEnter(data);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            var data = Dragger.DraggedData as T;
            if (data == null)
                return;

            if (CanReceive(data))
            {
                var dragger = data.DraggedBy as Dragger<T>;
                if (dragger != null)
                    dragger.Receiver = null;
                DraggingOverExit(data);
            }
        }

        public virtual bool CanReceive(T data)
        {
            return true;
        }

        public virtual void Receive(T data)
        {
        }

        public virtual void DraggingOverEnter(T data)
        {
        }

        public virtual void DraggingOverExit(T data)
        {
        }
    }
}