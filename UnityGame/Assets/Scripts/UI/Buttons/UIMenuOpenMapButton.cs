using Audio;

namespace UI.Buttons
{
    public class UIMenuOpenMapButton : UIBaseButton
    {
        public SoundAsset ClickSound;
        
        protected override void OnButtonPressed()
        {
            UIDebugText.Instance.ShowDebugText("UIMenuOpenMapButton OnButtonPressed");
            FindObjectOfType<UIMenuFrameController>().OpenMap();
        }
    }
}