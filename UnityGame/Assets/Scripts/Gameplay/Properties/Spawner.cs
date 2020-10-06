using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Properties
{
    [RequireComponent(typeof(Entity))]
    public class Spawner : MonoBehaviour, ICommandHandler
    {
        public GameObject Prefab;
        public int Delay = 0;

        [Header("Visuals")]
        public Animator Animator;
        public string AnimOnSpawnTrigger;
        public FxObject ShootFx;

        private Entity _entity;
        private int _spawnAtTurn;
        private UITimerManager _uiTimerManager;

        public void OnInitialized(Level level)
        {
            _entity = GetComponent<Entity>();
            _uiTimerManager = GameObject.FindObjectOfType<UITimerManager>();
            _spawnAtTurn = level.CurrentTurnNumber + Delay;
            
            if (_uiTimerManager != null)
                _uiTimerManager.SetTimer(gameObject, Delay);
        }

        public void OnAfterPlayerMove(Level level)
        {
            if (_spawnAtTurn == level.CurrentTurnNumber)
            {
                level.Dispatch(new SpawnCommand(_entity.Id));
                if (_uiTimerManager != null)
                    _uiTimerManager.DeleteTimer(gameObject);
            }
            else
            {
                UpdateTimer(level);    
            }
        }

        public IEnumerable<IChange> Handle(Level level, ICommand command)
        {
            if (command is SpawnCommand)
            {
                var entity = level.Spawn(
                    Prefab, 
                    _entity.Position + Utils.MoveDelta(_entity.Orientation),
                    _entity.Orientation);
                
                if (entity != null)
                {
                    if (Animator != null)
                        Animator.SetTrigger(AnimOnSpawnTrigger);
                    
                    ShootFx?.Trigger(transform);

                    yield return new SpawnChange(_entity.Id, entity.Id);
                }
            }
        }

        public void Revert(Level level, IChange change)
        {
            if (change is SpawnChange spawnChange)
                level.Despawn(spawnChange.SpawnedObjectId);
        }

        public void OnTurnRolledBack(Level level)
        {
            UpdateTimer(level);
        }

        private void UpdateTimer(Level level)
        {
            var turnsRemaining =  _spawnAtTurn - level.CurrentTurnNumber;
            
            if (turnsRemaining >= 0)
            {
                if (_uiTimerManager != null)
                    _uiTimerManager.SetTimer(gameObject, turnsRemaining);
            }
            else
            {
                if (_uiTimerManager != null)
                    _uiTimerManager.DeleteTimer(gameObject);
            }
        }
    }
}