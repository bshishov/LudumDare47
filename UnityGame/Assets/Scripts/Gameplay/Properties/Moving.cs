using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Properties
{
    [RequireComponent(typeof(Entity))]
    public class Moving : MonoBehaviour, ICommandHandler
    {
        public bool StopOnFrontCollision = true;
        
        private Entity _entity;
        private bool _isMoving = true;

        private void Start()
        {
            _entity = GetComponent<Entity>();
        }

        public void OnTurnStarted(Level level)
        {
            if(_isMoving)
                level.Dispatch(new MoveCommand(_entity.Id, _entity.Orientation, false));
        }

        public IEnumerable<IChange> Handle(Level level, ICommand command)
        {
            if (command is CollisionEvent collisionEvent)
            {
                if (StopOnFrontCollision && collisionEvent.Direction == Direction.Front)
                {
                    _isMoving = false;
                    yield return new StoppedMoving(_entity.Id);
                }
            }
        }

        public void Revert(Level level, IChange change)
        {
            if (change is StoppedMoving)
            {
                _isMoving = true;
            }
        }
    }
}