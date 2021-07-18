using Audio;
using Gameplay;
using UIF.Data;
using UIF.Scripts;
using UIF.Scripts.Transitions;
using UnityEngine;

namespace UI
{
    public class UIGameFrameController : MonoBehaviour
    {
        public FrameManager FrameManager;
        public BaseTransition Transition;
        public SoundAsset PauseSound;

        [Header("Frames")] 
        public FrameData GameFrame;
        public FrameData PauseFrame;
        public FrameData WinFrame;
        public FrameData LoseFrame;

        private void Start()
        {
            Common.LevelStateChanged += OnLevelStateChanged;
            Common.PauseStateChanged += OnPauseStateChanged;
        }

        private void OnDestroy()
        {
            Common.LevelStateChanged -= OnLevelStateChanged;
            Common.PauseStateChanged -= OnPauseStateChanged;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Common.TogglePause();
            }
        }

        private void OnLevelStateChanged(Level level, Level.GameState state)
        {
            if (state == Level.GameState.PlayerDied)
            {
                FrameManager.TransitionTo(LoseFrame, Transition);
            }
            else if(state == Level.GameState.CatGirlDied)
            {
                FrameManager.TransitionTo(LoseFrame, Transition);
            }
            else if (state == Level.GameState.Win) 
            {
                FrameManager.TransitionTo(WinFrame, Transition);
            }
            else if (state == Level.GameState.WaitingForPlayerCommand)
            {
                if(FrameManager.ActiveFrame != GameFrame)
                    FrameManager.TransitionTo(GameFrame, Transition);
            }
        }
        
        private void OnPauseStateChanged(bool isPaused)
        {
            if (isPaused)
            {
                FrameManager.TransitionTo(PauseFrame, Transition);
                SoundManager.Instance.Play(PauseSound);
            }
            else
                FrameManager.TransitionTo(GameFrame, Transition);
        }
    }
}