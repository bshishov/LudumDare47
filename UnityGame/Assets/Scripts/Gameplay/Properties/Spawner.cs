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
        private int? _shootAt;
        private UITimerManager _uiTimerManager;

        private void Start()
        {
        }

        public void OnInitialized(Level level)
        {
            _entity = GetComponent<Entity>();
            _uiTimerManager = GameObject.FindObjectOfType<UITimerManager>();
            
            if (!_shootAt.HasValue)
                _shootAt = level.CurrentTurnNumber + Delay;
            
            if (_uiTimerManager != null)
                _uiTimerManager.SetTimer(gameObject, Delay);
        }

        public void OnTurnStarted(Level level)
        {
            UpdateTimer(level);
            if(_shootAt.Value == level.CurrentTurnNumber)
                level.Dispatch(new SpawnCommand(_entity.Id));
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
                    Debug.Log($"Spawned {entity.Id}");
                    
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
            {
                Debug.Log($"Despawning {spawnChange.SpawnedObjectId}");
                level.Despawn(spawnChange.SpawnedObjectId);
            }
        }

        public void OnTurnRolledBack(Level level)
        {
            UpdateTimer(level);
        }

        private void UpdateTimer(Level level)
        {
            var turnsRemaining =  _shootAt.Value - level.CurrentTurnNumber;
            
            if (turnsRemaining >= 0)
            {
                if (_uiTimerManager != null)
                    _uiTimerManager.SetTimer(gameObject, turnsRemaining);
            }
            else
            {
                Debug.Log($"Clearing timer for {gameObject}");
                if (_uiTimerManager != null)
                    _uiTimerManager.DeleteTimer(gameObject);
            }
        }
    }
}