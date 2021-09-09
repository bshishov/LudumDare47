using Audio;

namespace UI.Buttons
{
    public class UIMenuOpenMapButton : UIBaseButton
    {
        public SoundAsset ClickSound;
        
        protected override void OnButtonPressed()
        {
            FindObjectOfType<UIMenuFrameController>().OpenMap();
        }
    }
}