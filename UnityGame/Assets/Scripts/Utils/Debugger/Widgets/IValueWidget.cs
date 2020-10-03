namespace Utils.Debugger.Widgets
{
    public interface IValueWidget : IWidget
    {
        void SetValue(object o);
    }

    public interface IValueWidget<in T> : IValueWidget
    {
        void SetValue(T value);
    }
}