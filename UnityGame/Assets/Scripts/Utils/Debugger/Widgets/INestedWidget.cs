using System.Collections.Generic;

namespace Utils.Debugger.Widgets
{
    public interface INestedWidget : IWidget, IEnumerable<KeyValuePair<string, IWidget>>
    {
        bool GetExpanded(int index);
        void SetExpanded(int index, bool value);
    }
}