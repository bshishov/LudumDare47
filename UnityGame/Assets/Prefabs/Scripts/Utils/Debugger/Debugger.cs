#if DEBUG
#define USE_REFLECTION
#endif

using System;
using System.Collections.Generic;
using UnityEngine;
using Utils.Debugger.Widgets;
using Object = UnityEngine.Object;

namespace Utils.Debugger
{
    public static class Debugger
    {
        private static DebuggerComponent _instance;

        public static DebuggerComponent Default
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Object.FindObjectOfType<DebuggerComponent>();
                    if (_instance == null)
                        _instance = Create();
                }

                return _instance;
            }
        }

        private static DebuggerComponent Create()
        {
            var obj = new GameObject("[DEBUGGER]");
            var comp = obj.AddComponent<DebuggerComponent>();
            Object.DontDestroyOnLoad(obj);
            return comp;
        }

        private static bool IsSubclassOfRawGeneric(Type generic, Type toCheck)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur) return true;

                toCheck = toCheck.BaseType;
            }

            return false;
        }

        public static IValueWidget<T> GetDefaultWidget<T>()
        {
            return GetDefaultWidget(typeof(T)) as IValueWidget<T>;
        }

        public static IValueWidget GetDefaultWidget(Type type)
        {
            if (type == typeof(string) || 
                type == typeof(bool)) 
                return new StringWidget();

            if (type == typeof(int) || 
                type == typeof(float) || 
                type == typeof(double)) 
                return new NumericWidget();
            
#if USE_REFLECTION
            if (type.IsGenericType)
            {
                var genericArguments = type.GetGenericArguments();

                // List
                if (IsSubclassOfRawGeneric(typeof(IEnumerable<>), type))
                {
                    var elemType = typeof(EnumerableWidget<>).MakeGenericType(genericArguments[0]);
                    return (IValueWidget) Activator.CreateInstance(elemType);
                }

                // Dictionary
                if (IsSubclassOfRawGeneric(typeof(IDictionary<,>), type))
                {
                    var elemType =
                        typeof(DictionaryWidget<,>).MakeGenericType(genericArguments[0], genericArguments[1]);
                    return (IValueWidget) Activator.CreateInstance(elemType);
                }
            }

            return new ObjectWidget();
#else
            return new StringWidget();
#endif
        }
    }
}