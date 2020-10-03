using System;
using UnityEngine;

namespace Utils.Debugger.Widgets
{
    public class CustomUIWidget : IWidget
    {
        private readonly Action<Rect> _drawAction;
        private readonly Vector2 Size;

        public CustomUIWidget(Vector2 size, Action<Rect> drawAction)
        {
            Size = size;
            _drawAction = drawAction;
        }

        public Vector2 GetSize(Style style)
        {
            return Size;
        }

        public void Draw(Rect rect, Style style)
        {
            if (_drawAction != null)
                _drawAction(rect);
            else
                GUI.Label(rect, "Missing DRAW function!");
        }
    }
}