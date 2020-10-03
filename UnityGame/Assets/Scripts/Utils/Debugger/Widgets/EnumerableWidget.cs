using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils.Debugger.Widgets
{
    public class EnumerableWidget<T> : INestedWidget, IValueWidget<IEnumerable<T>>
    {
        private readonly Cache<int, bool> _expanded = new Cache<int, bool>(false);
        private readonly Cache<int, IValueWidget> _widgetsCache;
        private IEnumerable<T> _value;

        public EnumerableWidget()
        {
            _widgetsCache = new Cache<int, IValueWidget>(GetWidget);
        }

        public EnumerableWidget(IEnumerable<T> val) : this()
        {
            SetValue(val);
        }

        public void Draw(Rect rect, Style style)
        {
        }

        public Vector2 GetSize(Style style)
        {
            return Vector2.zero;
        }

        public IEnumerator<KeyValuePair<string, IWidget>> GetEnumerator()
        {
            if (_value == null)
                yield break;

            var i = 0;
            foreach (var v in _value)
            {
                var widget = _widgetsCache.Get(i);
                if (widget != null)
                    widget.SetValue(v);
                yield return new KeyValuePair<string, IWidget>(i.ToString(), widget);
                i += 1;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool GetExpanded(int index)
        {
            return _expanded.Get(index);
        }

        public void SetExpanded(int index, bool value)
        {
            _expanded.Set(index, value);
        }

        public void SetValue(IEnumerable<T> value)
        {
            if (value.Equals(_value))
                return;

            _expanded.Clear();
            _value = value;
        }

        public void SetValue(object o)
        {
            SetValue((IEnumerable<T>) o);
        }

        private IValueWidget GetWidget(int index)
        {
            var w = Debugger.GetDefaultWidget(typeof(T));
            Debug.LogFormat("New w={0}", w);
            return w;
        }
    }
}