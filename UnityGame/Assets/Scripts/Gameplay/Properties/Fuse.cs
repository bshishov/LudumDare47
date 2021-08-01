using System.Collections.Generic;
using Audio;
using UnityEngine;

namespace Gameplay.Properties
{
    [RequireComponent(typeof(Entity))]
    public class Fuse : MonoBehaviour, ICommandHandler, IHasTimer
    {
        public int Delay = 3;
        public bool AutoIgnite = true;

        [Header("Visuals")] 
        public FxObject Sparks;

        [Header("Sound")] 
        public SoundAsset IgniteSound;

        private Entity _entity;
        private int _turnsUntilDetonate;

        private void Start()
        {
            _entity = GetComponent<Entity>();
        }

        public void OnInitialized(Level level)
        {
            // Start visuals prematurely
            if (AutoIgnite)
            {
                _turnsUntilDetonate = Delay;
                SoundManager.Instance.Play(IgniteSound);
                Sparks?.Trigger(transform);
            }
            else
            {
                _turnsUntilDetonate = -1;
            }
            UpdateSparksState();
        }

        public void OnAfterPlayerMove(Level level)
        {
            if (_turnsUntilDetonate == 0)
            {
                // Ignite phase is completed, send detonate command
                // and stop ignite phase
                level.DispatchEarly(new DetonateCommand(_entity.Id));
            }

            _turnsUntilDetonate--;
            UpdateSparksState();
        }

        public IEnumerable<IChange> Handle(Level level, ICommand command)
        {
            if (command is IgniteCommand)
            {
                _turnsUntilDetonate = Delay;
                SoundManager.Instance.Play(IgniteSound);
                
                yield return new FuseIgnited(_entity.Id, Delay);
            }
            else if(command is DetonateCommand)
            {
            }
            else if(command is DestroyCommand)
            {
            }
        }

        public void Revert(Level level, IChange change)
        {
            if (change is FuseIgnited)
            {
                Debug.LogWarning("TEST THIS WHEN THERE SHOULD BE REVERTABLE IGNITE MECHANICS");
                _turnsUntilDetonate = -100; // HACK, NEEDS TESTING
            }
            else if (change is DestroyedChange)
            {
            }
        }

        public void OnTurnRolledBack(Level level)
        {
            _turnsUntilDetonate += 1;
            UpdateSparksState();
        }

        public int? GetCurrentTimerValue()
        {
            if (_turnsUntilDetonate >= 0)
                return _turnsUntilDetonate;
            return null;
        }

        private void UpdateSparksState()
        {
            if (_turnsUntilDetonate >= 0)
            {
                Sparks?.Trigger(transform);
            }
            else
            {
                Sparks?.Stop();
            }
        }
    }
}