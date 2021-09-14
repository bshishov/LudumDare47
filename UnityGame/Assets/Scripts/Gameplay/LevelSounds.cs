using Audio;
using UnityEngine;

namespace Gameplay
{
    public class LevelSounds : MonoBehaviour
    {
        [Header("Sounds")] 
        [SerializeField] private SoundAsset TurnRollbackSound;
        [SerializeField] private SoundAsset TurnRollbackDeniedSound;
        [SerializeField] private SoundAsset LevelCompleteSound;
        [SerializeField] private SoundAsset LevelFailedSound;
        
        [Header("Music")] 
        [SerializeField] private Sound InGameMusic;

        void Start()
        {
            var level = Common.CurrentLevel;
            if (level != null)
            {
                level.TurnRollbackSucceeds += LevelOnTurnRollbackSucceeds;
                level.TurnRollbackDenied += LevelOnTurnRollbackDenied;
                level.StateChanged += LevelOnStateChanged;
            }
            
            MusicManager.Play(InGameMusic);
        }

        private void LevelOnStateChanged(Level.GameState state)
        {
            if (state == Level.GameState.Win)
                SoundManager.Instance.Play(LevelCompleteSound);
            if (state == Level.GameState.PlayerDied || state == Level.GameState.CatGirlDied)
                SoundManager.Instance.Play(LevelFailedSound);
        }

        private void LevelOnTurnRollbackDenied()
        {
            SoundManager.Instance.Play(TurnRollbackDeniedSound);
        }

        private void LevelOnTurnRollbackSucceeds()
        {
            SoundManager.Instance.Play(TurnRollbackSound);
        }
    }
}