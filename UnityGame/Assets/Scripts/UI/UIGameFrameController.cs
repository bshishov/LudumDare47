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
        public BaseTransition PauseTransition;
        public SoundAsset PauseSound;

        [Header("Frames")] 
        public FrameData GameFrame;
        public FrameData PauseFrame;
        public FrameData WinFrame;
        public FrameData LoseFrame;

        private FrameData _prePauseFrame;

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
                FrameManager.TransitionTo(LoseFrame, Transition, 0);
            }
            else if(state == Level.GameState.CatGirlDied)
            {
                FrameManager.TransitionTo(LoseFrame, Transition, 0);
            }
            else if (state == Level.GameState.Win) 
            {
                FrameManager.TransitionTo(WinFrame, Transition, 0);
            }
            else if (state == Level.GameState.WaitingForPlayerCommand)
            {
                if(FrameManager.ActiveFrame != GameFrame)
                    FrameManager.TransitionTo(GameFrame, Transition, 0);
            }
        }
        
        private void OnPauseStateChanged(bool isPaused)
        {
            if (isPaused)
            {
                _prePauseFrame = FrameManager.ActiveFrame;
                FrameManager.TransitionTo(PauseFrame, PauseTransition, 0);
                SoundManager.Instance.Play(PauseSound);
            }
            else
                FrameManager.TransitionTo(_prePauseFrame, PauseTransition, 0);
        }
    }
}