using System;
using UnityEngine;

namespace Utils.Debugger.Widgets
{
    public class ActionWidget : IValueWidget<Action>, IActionWidget
    {
        private const string DefaultCaption = "<Click or F5>";
        private Action _action;
        private readonly string _caption;

        public ActionWidget(Action action, string caption = DefaultCaption)
        {
            _action = action;
            _caption = caption;
        }

        public void DoAction()
        {
            _action?.Invoke();
        }

        public void SetValue(Action value)
        {
            _action = value;
        }

        public Vector2 GetSize(Style style)
        {
            return new Vector2(100, 20);
        }

        public void Draw(Rect rect, Style style)
        {
            if (GUI.Button(rect, _caption)) DoAction();
        }

        public void SetValue(object o)
        {
            _action = o as Action;
        }
    }
}