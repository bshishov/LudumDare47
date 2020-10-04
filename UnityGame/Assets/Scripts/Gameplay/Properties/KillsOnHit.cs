using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Properties
{
    [RequireComponent(typeof(Entity))]
    public class KillsOnHit : MonoBehaviour, ICommandHandler
    {
        public bool SelfDestroyOnHit = true;
        
        [Header("Deadly sides")]
        public bool Front = true;
        public bool Back = true;
        public bool Left = true;
        public bool Right = true;
        
        private Entity _entity;

        private void Start()
        {
            _entity = GetComponent<Entity>();
        }

        public void OnTurnStarted(Level level)
        {
        }

        public IEnumerable<IChange> Handle(Level level, ICommand command)
        {
            if (command is HitCommand hitCommand && IsDeadlySide(hitCommand.Direction))
            {
                level.DispatchEarly(new DestroyCommand(hitCommand.SourceId));
                if (SelfDestroyOnHit)
                    level.DispatchEarly(new DestroyCommand(_entity.Id));
            }
            else if(command is CollisionEvent collisionEvent && IsDeadlySide(collisionEvent.Direction))
            {
                level.DispatchEarly(new DestroyCommand(collisionEvent.SourceId));
                if (SelfDestroyOnHit)
                    level.DispatchEarly(new DestroyCommand(_entity.Id));
            }
            yield break;
        }

        bool IsDeadlySide(Direction relativeCollisionDirection)
        {
            return (Front && relativeCollisionDirection == Direction.Front) ||
                   (Back && relativeCollisionDirection == Direction.Back) ||
                   (Left && relativeCollisionDirection == Direction.Left) ||
                   (Right && relativeCollisionDirection == Direction.Right);
        }

        public void Revert(Level level, IChange change)
        {
        }
    }
}