using UnityEngine;

namespace Utils.Debugger.Widgets
{
    public class StringWidget : IValueWidget<string>, IValueWidget<object>
    {
        private string _value;

        public StringWidget()
        {
        }

        public StringWidget(string value) : this()
        {
            SetValue(value);
        }

        public StringWidget(object value) : this()
        {
            SetValue(value.ToString());
        }

        public void Draw(Rect rect, Style style)
        {
            GUI.Label(rect, _value);
        }

        public Vector2 GetSize(Style style)
        {
            if (_value != null)
                return new Vector2(
                    Mathf.Max(_value.Length * 10f, 200f),
                    style.LineHeight
                );

            return Vector2.zero;
        }

        public void SetValue(string value)
        {
            _value = value;
        }

        public void SetValue(object value)
        {
            _value = value.ToString();
        }
    }
}