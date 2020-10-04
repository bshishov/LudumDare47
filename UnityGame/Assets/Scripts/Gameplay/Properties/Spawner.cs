using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Properties
{
    [RequireComponent(typeof(Entity))]
    public class Spawner : MonoBehaviour, ICommandHandler
    {
        public GameObject Prefab;
        public int Delay = 0;
        
        private Entity _entity;

        private void Start()
        {
            _entity = GetComponent<Entity>();
        }
        
        public void OnTurnStarted(Level level)
        {
            var currentTurn = level.GetCurrentTurn();
            if (currentTurn.Number == Delay)
            {
                level.Dispatch(new SpawnCommand(_entity.Id));
            }
        }

        public IEnumerable<IChange> Handle(Level level, ICommand command)
        {
            if (command is SpawnCommand spawnCommand)
            {
                var entity = level.Spawn(
                    Prefab, 
                    _entity.Position + Utils.MoveDelta(_entity.Orientation),
                    _entity.Orientation);
                if (entity != null)
                {
                    yield return new SpawnChange(_entity.Id, entity.Id);
                }
            }
        }

        public void Revert(Level level, IChange change)
        {
            if (change is SpawnChange spawnChange)
            {
                level.DestroyEntity(spawnChange.SpawnedObjectId);
            }
        }
    }
}