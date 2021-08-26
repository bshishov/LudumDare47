using Audio;

namespace UI.Buttons
{
    public class UIMapOpenMenu : UIBaseButton
    {
        public SoundAsset ClickSound;

        protected override void OnButtonPressed()
        {
            FindObjectOfType<UIMenuFrameController>().OpenMainMenu();
        }
    }
}
