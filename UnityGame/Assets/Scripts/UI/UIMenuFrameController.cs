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
            FrameManager.TransitionTo(ShopFrame, Transition);
            SoundManager.Instance.Play(TransitionSound);
        }
        
        public void OpenMap()
        {
            FrameManager.TransitionTo(MapFrame, Transition);
            SoundManager.Instance.Play(TransitionSound);
        }
        
        public void OpenPacks()
        {
            FrameManager.TransitionTo(PacksFrame, Transition);
            SoundManager.Instance.Play(TransitionSound);
        }
        
        public void OpenMainMenu()
        {
            FrameManager.TransitionTo(MainMenuFrame, Transition);
            SoundManager.Instance.Play(TransitionSound);
        }
    }
}