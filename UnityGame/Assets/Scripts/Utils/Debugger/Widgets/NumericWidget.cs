namespace Utils.Debugger.Widgets
{
    public class NumericWidget : StringWidget, IValueWidget<int>, IValueWidget<float>
    {
        public void SetValue(float value)
        {
            SetValue(value.ToString());
        }

        public void SetValue(int value)
        {
            SetValue(value.ToString());
        }
    }
}