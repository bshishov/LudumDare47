using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Properties
{
    [RequireComponent(typeof(Entity))]
    public class IgniteOnHit : MonoBehaviour, ICommandHandler
    {
        public void OnInitialized(Level level)
        {
        }

        public void OnTurnStarted(Level level)
        {
        }

        public IEnumerable<IChange> Handle(Level level, ICommand command)
        {
            if (command is HitCommand hitCommand)
                level.DispatchEarly(new IgniteCommand(hitCommand.SourceId));
            else if(command is CollisionEvent collisionEvent)
                level.DispatchEarly(new IgniteCommand(collisionEvent.SourceId));
            yield break;
        }

        public void Revert(Level level, IChange change)
        {
        }

        public void OnTurnRolledBack(Level level)
        {
        }
    }
}