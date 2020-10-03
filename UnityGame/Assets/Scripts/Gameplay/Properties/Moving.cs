using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Properties
{
    public class Moving : MonoBehaviour, ICommandHandler
    {
        private Entity _entity;

        private void Start()
        {
            _entity = GetComponent<Entity>();
        }

        public void OnTurnStarted(Level level)
        {
            level.Dispatch(new MoveCommand(_entity.Id, _entity.Orientation, false));
        }

        public IEnumerable<IChange> Handle(Level level, ICommand command)
        {
            yield break;
        }

        public void Revert(Level level, IChange change)
        {
        }
    }
}