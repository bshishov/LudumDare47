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

        public void OpenShop()
        {
            FrameManager.TransitionTo(ShopFrame, Transition, 0);
            SoundManager.Instance.Play(TransitionSound);
        }

        public void OpenMap()
        {
            if (FrameManager.ActiveFrame == ShopFrame)
            {
                FrameManager.TransitionTo(MapFrame, Transition, 1);
            }
            else
            {
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