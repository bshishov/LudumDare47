namespace UI.Buttons
{
    public class UIMenuOpenPacksButton : UIBaseButton
    {
        protected override void OnButtonPressed()
        {
            FindObjectOfType<UIMenuFrameController>().OpenPacks();
        }
    }
}