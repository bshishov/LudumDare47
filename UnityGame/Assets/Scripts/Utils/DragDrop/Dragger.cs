using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Utils.DragDrop
{
    public class Dragger : MonoBehaviour
    {
        public static IDragData DraggedData;
    }

    public class Dragger<T> : Dragger,
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler
        where T : class, IDragData
    {
        private Transform _draggableTransform;
        public DragReceiver<T> Receiver;

        public void OnBeginDrag(PointerEventData eventData)
        {
            // Already dragging
            if (DraggedData != null)
                return;

            var draggable = DragStarted(eventData);
            if (draggable != null && draggable.Transform != null)
            {
                Debug.Log("[UIDraggable] Drag started");
                DraggedData = draggable;
                _draggableTransform = draggable.Transform;
                draggable.DraggedBy = this;

                SetRaycastOptions(false);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_draggableTransform != null) _draggableTransform.position = Input.mousePosition;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (_draggableTransform != null)
            {
                SetRaycastOptions(true);

                var data = DraggedData as T;
                if (Receiver != null && data != null)
                {
                    if (Receiver.CanReceive(data))
                    {
                        Receiver.Receive(DraggedData as T);
                        DragWasReceived(Receiver, data);
                    }
                }
                else
                {
                    DragWasCancelled(data);
                }

                DraggedData.DraggedBy = null;
                DraggedData = null;
                Receiver = null;
                _draggableTransform = null;
            }
        }

        protected virtual T DragStarted(PointerEventData eventData)
        {
            throw new NotImplementedException();
        }

        protected virtual void DragWasReceived(DragReceiver<T> receiver, T data)
        {
        }

        protected virtual void DragWasCancelled(T data)
        {
        }

        private void SetRaycastOptions(bool value)
        {
            var cg = _draggableTransform.GetComponent<CanvasGroup>();
            if (cg != null)
                cg.blocksRaycasts = value;
        }
    }
}