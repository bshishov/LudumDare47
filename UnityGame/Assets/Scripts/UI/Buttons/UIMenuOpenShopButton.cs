namespace UI.Buttons
{
    public class UIMenuOpenShopButton : UIBaseButton
    {
        protected override void OnButtonPressed()
        {
            FindObjectOfType<UIMenuFrameController>().OpenShop();
        }
    }
}