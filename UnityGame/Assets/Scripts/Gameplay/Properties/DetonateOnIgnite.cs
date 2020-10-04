using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Properties
{
    [RequireComponent(typeof(Entity))]
    public class DetonateOnIgnite: MonoBehaviour, ICommandHandler
    {
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
            if (command is IgniteCommand)
                level.Dispatch(new DetonateCommand(_entity.Id));
            yield break;
        }

        public void Revert(Level level, IChange change)
        {
        }
    }
}