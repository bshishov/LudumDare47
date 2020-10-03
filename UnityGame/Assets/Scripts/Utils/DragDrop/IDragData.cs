using UnityEngine;

namespace Utils.DragDrop
{
    public interface IDragData
    {
        Transform Transform { get; }
        Dragger DraggedBy { get; set; }
    }
}