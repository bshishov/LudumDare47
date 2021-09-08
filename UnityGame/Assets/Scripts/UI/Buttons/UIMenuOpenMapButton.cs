using Audio;

namespace UI.Buttons
{
    public class UIMenuOpenMapButton : UIBaseButton
    {
        public SoundAsset ClickSound;
        
        protected override void OnButtonPressed()
        {
            UIDebugText.Instance.ShowDebugText("FindObjectOfType<UIMenuFrameController>().OpenMap();");
            FindObjectOfType<UIMenuFrameController>().OpenMap();
        }
    }
}