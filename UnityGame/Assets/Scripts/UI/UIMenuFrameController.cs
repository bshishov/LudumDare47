using System;
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
        
        [Header("Frames")] 
        public FrameData MainMenuFrame;
        public FrameData ShopFrame;
        public FrameData MapFrame;
        public FrameData PacksFrame;

        public void OpenShop()
        {
            FrameManager.TransitionTo(ShopFrame, Transition);
        }
        
        public void OpenMap()
        {
            FrameManager.TransitionTo(MapFrame, Transition);
        }
        
        public void OpenPacks()
        {
            FrameManager.TransitionTo(PacksFrame, Transition);
        }
        
        public void OpenMainMenu()
        {
            FrameManager.TransitionTo(MainMenuFrame, Transition);
        }
    }
}