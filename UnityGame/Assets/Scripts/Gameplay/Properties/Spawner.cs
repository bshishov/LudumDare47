using System.Collections.Generic;
using UI;
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
            SetUiTimer(Delay);
        }

        public void OnAfterPlayerMove(Level level)
        {
            var remainingTurns = _spawnAtTurn - level.CurrentTurnNumber;
            SetUiTimer(remainingTurns - 1);
            
            if (remainingTurns == 0)
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
                SetUiTimer(null);
                
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
            {
                level.Despawn(spawnChange.SpawnedObjectId);
                SetUiTimer(null);
            }
        }

        public void OnTurnRolledBack(Level level)
        {
            var timeRemaining = _spawnAtTurn - level.CurrentTurnNumber;
            SetUiTimer(timeRemaining);
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