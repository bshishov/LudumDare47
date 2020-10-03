using System;
using System.Linq;
using UnityEngine;
using Utils.Debugger.Widgets;
using Object = UnityEngine.Object;

namespace Utils.Debugger
{
    public class Logger : IWidget, ILogHandler
    {
        class UILogMessage
        {
            public string Text;
            public float Height = 0f;
            public GUIContent Content = null;
        }
        
        private readonly CyclePool<UILogMessage> _messages;
        private readonly bool _unityLog;
        private readonly int _rows;
        private Vector2 _scrollPosition = Vector2.zero;
        private const float WidgetWidth = 700f;

        public Logger(int historySize = 100, bool unityLog = true, int rows = 20)
        {
            _messages = new CyclePool<UILogMessage>(historySize);
            _unityLog = unityLog;
            _rows = rows;
        }

        public Vector2 GetSize(Style style)
        {
            return new Vector2(WidgetWidth, style.LineHeight * _rows);
        }

        public void Draw(Rect rect, Style style)
        {
            var lineWidth = WidgetWidth - 20f;
            var style1 = style.LogLineStyle;
            var style2 = style.LogLineStyleAlt;
            var totalHeight = 0f;
            var n = _messages.Length;
            for (var i = 0; i < n; i++)
            {
                var m = _messages.GetByIndexStartFromNewest(i);
                if (m.Content == null)
                {
                    m.Content = new GUIContent(m.Text);
                    m.Height = style1.CalcHeight(m.Content, lineWidth);
                }
                totalHeight += m.Height;
            }

            _scrollPosition = GUI.BeginScrollView(rect, _scrollPosition,
                new Rect(0, 0, WidgetWidth - 20f, totalHeight), 
                false, 
                true);
            
            
            var currentY = 0f;

            for (var i = 0; i < n; i++)
            {
                var labelStyle = style2;
                if (i % 2 == 0)
                    labelStyle = style1;

                var message = _messages.GetByIndexStartFromNewest(i);
                var content = message.Content;
                
                //GUI.Label(new Rect(0, currentY, lineWidth, lineHeight), content, labelStyle);
                
                // TODO: Make text selectable the right way smh
                GUI.TextArea(new Rect(0, currentY, lineWidth, message.Height), message.Text, labelStyle);
                
                currentY += message.Height;
            }

            GUI.EndScrollView();
        }

        public void Log(string message)
        {
            var m = _messages.GetOrCreate();
            m.Text = message;
            m.Content = null;
            
            if (_unityLog)
                Debug.Log(message);
        }

        public void LogFormat(string message, params object[] args)
        {
            var m = _messages.GetOrCreate();
            m.Text = string.Format(message, args);
            m.Content = null;
            
            if (_unityLog)
                Debug.LogFormat(message, args);
        }

        public void LogFormat(LogType logType, Object context, string format, params object[] args)
        {
            LogFormat(format, args);
        }

        public void LogException(Exception exception, Object context)
        {
            LogFormat(exception.ToString());
        }
    }
}