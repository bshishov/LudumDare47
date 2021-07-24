namespace UI.Buttons
{
    public class UIMenuOpenMapButton : UIBaseButton
    {
        protected override void OnButtonPressed()
        {
            FindObjectOfType<UIMenuFrameController>().OpenMap();
        }
    }
}