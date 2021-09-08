using Audio;
using UIF.Data;
using UIF.Scripts;
using UIF.Scripts.Transitions;
using UnityEngine;

namespace UI
{
    public class UIMenuFrameController : MonoBehaviour
    {
        public FrameManager FrameManager;
        public BaseTransition Transition;
        public SoundAsset TransitionSound;

        [Header("Frames")]
        public FrameData MainMenuFrame;
        public FrameData ShopFrame;
        public FrameData MapFrame;
        public FrameData PacksFrame;

        public static bool IsMainMenuWasLoad;

        private void Awake()
        {
            //After back to main menu show map frame
            if (IsMainMenuWasLoad) {
                FindObjectOfType<FrameManager>().ChangeInitialFrame(MapFrame);
            }
        }
        public void OpenShop()
        {
            FrameManager.TransitionTo(ShopFrame, Transition, 0);
            SoundManager.Instance.Play(TransitionSound);
        }

        public void OpenMap()
        {
            IsMainMenuWasLoad = true;

            if (FrameManager.ActiveFrame == ShopFrame)
            {
                FrameManager.TransitionTo(MapFrame, Transition, 1);
            }
            else
            {
                //TODO
                //UIDebugText.Instance.ShowDebugText(MapFrame + " - frame data in UIMenuFrameController.OpenMap");
                FrameManager.TransitionTo(MapFrame, Transition, 0);
            }

            SoundManager.Instance.Play(TransitionSound);
        }

        public void OpenPacks()
        {
            FrameManager.TransitionTo(PacksFrame, Transition, 0);
            SoundManager.Instance.Play(TransitionSound);
        }

        public void OpenMainMenu()
        {
            FrameManager.TransitionTo(MainMenuFrame, Transition, 0);
            SoundManager.Instance.Play(TransitionSound);
        }
    }
}