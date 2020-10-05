using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Properties
{
    [RequireComponent(typeof(Entity))]
    public class DelayedDetonator : MonoBehaviour, ICommandHandler
    {
        public int Delay = 3;
        private int? _createdTurn;
        private Entity _entity;

        private void Start()
        {
            _entity = GetComponent<Entity>();
        }

        public void OnTurnStarted(Level level)
        {
            if (!_createdTurn.HasValue)
                _createdTurn = level.CurrentTurnNumber;

            if (level.CurrentTurnNumber - _createdTurn >= Delay)
                level.Dispatch(new DetonateCommand(_entity.Id));
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