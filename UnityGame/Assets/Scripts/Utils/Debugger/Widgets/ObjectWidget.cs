using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Utils.Debugger.Widgets
{
    public class ObjectWidget : INestedWidget, IValueWidget<object>
    {
        private readonly BindingFlags _bindingFlags = BindingFlags.Instance | BindingFlags.Public;
        private readonly Cache<int, bool> _expanded = new Cache<int, bool>(false);
        private readonly Cache<FieldInfo, IValueWidget> _fieldWidgetsCache;
        private readonly Cache<PropertyInfo, IValueWidget> _propWidgetsCache;
        private FieldInfo[] _fields;
        private PropertyInfo[] _props;
        private Type _type;
        private object _value;

        public ObjectWidget()
        {
            _propWidgetsCache = new Cache<PropertyInfo, IValueWidget>(GetWidget);
            _fieldWidgetsCache = new Cache<FieldInfo, IValueWidget>(GetWidget);
        }

        public ObjectWidget(object value) : this()
        {
            SetValue(value);
        }

        public Vector2 GetSize(Style style)
        {
            return Vector2.zero;
        }

        public void Draw(Rect rect, Style style)
        {
            if (_value == null)
                GUI.Label(rect, "null");
            else
                GUI.Label(rect, _type.Name);
        }

        public IEnumerator<KeyValuePair<string, IWidget>> GetEnumerator()
        {
            if (_value == null)
                yield break;

            var i = 0;
            foreach (var field in _fields)
            {
                IValueWidget widget = null;

                try
                {
                    // If property is expanded
                    // Do lazy widget lookup
                    if (_expanded.Get(i))
                    {
                        widget = _fieldWidgetsCache.Get(field);

                        if (widget != null)
                        {
                            var v = field.GetValue(_value);
                            widget.SetValue(v);
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogWarning(e);
                }

                i += 1;
                yield return new KeyValuePair<string, IWidget>(field.Name, widget);
            }

            foreach (var prop in _props)
            {
                IValueWidget widget = null;

                try
                {
                    // If property is expanded
                    // Do lazy widget lookup
                    if (_expanded.Get(i))
                    {
                        widget = _propWidgetsCache.Get(prop);

                        if (widget != null)
                        {
                            var v = prop.GetValue(_value, null);
                            widget.SetValue(v);
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogWarning(e);
                }

                i += 1;
                yield return new KeyValuePair<string, IWidget>(string.Format("get {0}", prop.Name), widget);
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

        public void SetValue(object value)
        {
            if (value.Equals(_value))
                return;

            _value = value;
            _type = value.GetType();
            _props = _type.GetProperties(_bindingFlags);
            _fields = _type.GetFields(_bindingFlags);

            _expanded.Clear();
        }

        private IValueWidget GetWidget(PropertyInfo prop)
        {
            return Debugger.GetDefaultWidget(prop.PropertyType);
        }

        private IValueWidget GetWidget(FieldInfo prop)
        {
            return Debugger.GetDefaultWidget(prop.FieldType);
        }
    }
}