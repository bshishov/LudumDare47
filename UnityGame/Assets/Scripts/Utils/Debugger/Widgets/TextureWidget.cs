using UnityEngine;

namespace Utils.Debugger.Widgets
{
    public class TextureWidget : IValueWidget<Texture>
    {
        public const float DefaultWidth = 256f;
        public const float DefaultHeight = 256f;
        private readonly Vector2 _size;
        private Texture _value;

        public TextureWidget(float width = DefaultWidth, float height = DefaultHeight)
        {
            _size = new Vector2(width, height);
        }

        public TextureWidget(Texture value, float width = DefaultWidth, float height = DefaultHeight) : this(width,
            height)
        {
            SetValue(value);
        }

        public void Draw(Rect rect, Style style)
        {
            GUI.DrawTexture(rect, _value);
        }

        public Vector2 GetSize(Style style)
        {
            return _size;
        }

        public void SetValue(object o)
        {
            _value = o as Texture;
        }

        public void SetValue(Texture value)
        {
            _value = value;
        }
    }
}