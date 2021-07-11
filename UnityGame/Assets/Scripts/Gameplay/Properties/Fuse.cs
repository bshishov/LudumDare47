using System.Collections.Generic;
using Audio;
using UI;
using UnityEngine;

namespace Gameplay.Properties
{
    [RequireComponent(typeof(Entity))]
    public class Fuse : MonoBehaviour, ICommandHandler
    {
        public int Delay = 3;
        public bool AutoIgnite = true;

        [Header("Visuals")] 
        public FxObject Sparks;

        [Header("Sound")] 
        public SoundAsset IgniteSound;

        private int _detonateAtTurn = -1;
        private Entity _entity;
        private UITimerManager _uiTimerManager;

        private void Start()
        {
            _entity = GetComponent<Entity>();
            _uiTimerManager = GameObject.FindObjectOfType<UITimerManager>();
        }

        public void OnInitialized(Level level)
        {
            // Start visuals prematurely
            if (AutoIgnite)
            {
                SoundManager.Instance.Play(IgniteSound);
                _detonateAtTurn = level.CurrentTurnNumber + Delay;
                Sparks?.Trigger(transform);
                SetUiTimer(Delay);
            }
        }

        public void OnAfterPlayerMove(Level level)
        {
            // It's finally FUSE's turn
            var timeRemaining = _detonateAtTurn - level.CurrentTurnNumber;
            SetUiTimer(timeRemaining - 1);
            
            if (timeRemaining == 0)
            {
                // Ignite phase is completed, send detonate command
                // and stop ignite phase
                level.DispatchEarly(new DetonateCommand(_entity.Id));
            }
        }

        public IEnumerable<IChange> Handle(Level level, ICommand command)
        {
            if (command is IgniteCommand && _detonateAtTurn < 0)
            {
                SoundManager.Instance.Play(IgniteSound);
                _detonateAtTurn = level.CurrentTurnNumber + Delay;
                Sparks?.Trigger(transform);
                SetUiTimer(Delay - 1);
                yield return new FuseIgnited(_entity.Id, Delay);
            }
            else if(command is DetonateCommand)
            {
                SetUiTimer(null);
                Sparks?.Stop();
            }
            else if(command is DestroyCommand)
            {
                SetUiTimer(null);
                Sparks?.Stop();
            }
        }

        public void Revert(Level level, IChange change)
        {
            if (change is FuseIgnited)
            {
                _detonateAtTurn = -1;
                Sparks?.Trigger(transform);
                SetUiTimer(null);
            }
        }

        public void OnTurnRolledBack(Level level)
        {
            var timeRemaining = _detonateAtTurn - level.CurrentTurnNumber;
            SetUiTimer(timeRemaining);
            if (timeRemaining > 0)
                Sparks?.Trigger(transform);
            else
                Sparks?.Stop();
        }

        private void SetUiTimer(int? number)
        {
            if(_uiTimerManager == null)
                return;
            
            if(number.HasValue && number.Value >= 0)
                _uiTimerManager.SetTimer(gameObject, number.Value);
            else
                _uiTimerManager.DeleteTimer(gameObject);
        }
    }
}