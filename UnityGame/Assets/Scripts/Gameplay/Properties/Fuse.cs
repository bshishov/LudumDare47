using System.Collections.Generic;
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
        
        public bool Ignited { get; private set; }
        
        private int _igniteTurn;
        private Entity _entity;
        private UITimerManager _uiTimerManager;

        private void Start()
        {
            _entity = GetComponent<Entity>();
            _uiTimerManager = GameObject.FindObjectOfType<UITimerManager>();
        }

        public void OnInitialized(Level level)
        {
        }

        public void OnTurnStarted(Level level)
        {
            if (!Ignited)
            {
                if (AutoIgnite)
                    level.DispatchEarly(new IgniteCommand(_entity.Id));
            }
            else
            {
                var remaining =  (_igniteTurn + Delay) - level.CurrentTurnNumber;
                
                if (remaining == 0)
                {
                    level.DispatchEarly(new DetonateCommand(_entity.Id));
                    Sparks?.Stop();
                    
                    if (_uiTimerManager != null)
                        _uiTimerManager.DeleteTimer(gameObject);
                }
                else if(remaining > 0)
                {
                    if (_uiTimerManager != null)
                        _uiTimerManager.SetTimer(gameObject, remaining);
                }
            }
        }

        public IEnumerable<IChange> Handle(Level level, ICommand command)
        {
            if (command is IgniteCommand && !Ignited)
            {
                Ignited = true;
                _igniteTurn = level.CurrentTurnNumber;
                Sparks?.Trigger(transform);
                yield return new FuseIgnited(_entity.Id, Delay);
            }
        }

        public void Revert(Level level, IChange change)
        {
            if (change is FuseIgnited)
            {
                if (_uiTimerManager != null)
                    _uiTimerManager.DeleteTimer(gameObject);
                
                Ignited = false;
                Sparks?.Stop();
            }
        }

        public void OnTurnRolledBack(Level level)
        {
        }
    }
}