using Audio;
using UnityEngine;

namespace UI.Buttons
{
    public class UIMainBackButton : UIBaseButton
    {
        public SoundAsset ClickSound;

        protected override void OnButtonPressed()
        {

            var uiFrameController = FindObjectOfType<UIMenuFrameController>();
            if (uiFrameController.FrameManager.ActiveFrame == uiFrameController.MapFrame)
            {
                uiFrameController.OpenMainMenu();
            }

            if (uiFrameController.FrameManager.ActiveFrame == uiFrameController.ShopFrame)
            {
                uiFrameController.OpenMap();
            }
            //Debug.Log(uiFrameController.FrameManager.ActiveFrame);
        }
    }
}
