using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils.Debugger.Widgets
{
    public class DictionaryWidget<T1, T2> : INestedWidget, IValueWidget<IDictionary<T1, T2>>
    {
        private readonly Cache<int, bool> _expanded = new Cache<int, bool>(false);
        private readonly Cache<T1, IValueWidget> _widgetsCache;
        private IDictionary<T1, T2> _value;

        public DictionaryWidget()
        {
            _widgetsCache = new Cache<T1, IValueWidget>(GetWidget);
        }

        public DictionaryWidget(IDictionary<T1, T2> dict) : this()
        {
            SetValue(dict);
        }

        public void Draw(Rect rect, Style style)
        {
        }

        public IEnumerator<KeyValuePair<string, IWidget>> GetEnumerator()
        {
            if (_value == null)
                yield break;

            foreach (var kvp in _value)
            {
                var widget = _widgetsCache.Get(kvp.Key);
                if (widget != null)
                    widget.SetValue(kvp.Value);
                yield return new KeyValuePair<string, IWidget>(kvp.Key.ToString(), widget);
            }
        }

        public Vector2 GetSize(Style style)
        {
            return Vector2.zero;
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

        public void SetValue(object o)
        {
            SetValue(o as IDictionary<T1, T2>);
        }

        public void SetValue(IDictionary<T1, T2> value)
        {
            if (value.Equals(_value))
                return;
            _expanded.Clear();
            _value = value;
        }

        private IValueWidget GetWidget(T1 key)
        {
            return Debugger.GetDefaultWidget(typeof(T2));
        }
    }
}