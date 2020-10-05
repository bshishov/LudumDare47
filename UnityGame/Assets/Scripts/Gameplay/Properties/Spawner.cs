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
        private int? _createdTurn;
        private UITimerManager _uiTimerManager;

        private void Start()
        {
            _entity = GetComponent<Entity>();
            _uiTimerManager = GameObject.FindObjectOfType<UITimerManager>();
        }
        
        public void OnTurnStarted(Level level)
        {
            if (!_createdTurn.HasValue)
                _createdTurn = level.CurrentTurnNumber;

            var remaining =  (_createdTurn.Value + Delay) - level.CurrentTurnNumber;

            if (remaining > 0)
            {
                Debug.Log($"Timer {gameObject}: {remaining}");
                if (_uiTimerManager != null)
                    _uiTimerManager.SetTimer(gameObject, remaining);
            }
            else if(remaining == 0)
            {
                Debug.Log($"Clearing timer for {gameObject}");
                if (_uiTimerManager != null && Delay > 0)
                    _uiTimerManager.DeleteTimer(gameObject);
                level.Dispatch(new SpawnCommand(_entity.Id));
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
                _createdTurn = null;
            }
        }
    }
}