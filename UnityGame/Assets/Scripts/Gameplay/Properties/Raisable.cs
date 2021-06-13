using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Properties
{
    [RequireComponent(typeof(Entity))]
    public class Raisable : MonoBehaviour, ICommandHandler
    {
        private Entity _entity;
        private Transform _transform;

        private bool _isRaised = false;
        private int countEntityInPosition;
        public void OnInitialized(Level level)
        {
            _entity = GetComponent<Entity>();
            _transform = GetComponent<Transform>();
        }

        public IEnumerable<IChange> Handle(Level level, ICommand command)
        {
            if (command is HitCommand && !_isRaised)
            {
                _transform.position = new Vector3(_transform.position.x, _transform.position.y + 1.5f, _transform.position.z);

                _isRaised = true;
                yield return new Rise(_entity.Id);
            } 
        }

        public void Revert(Level level, IChange change)
        {
            if (change is Rise)
            {
                _transform.position = new Vector3(_transform.position.x, _transform.position.y - 1.5f, _transform.position.z);
                _isRaised = false;
            }
        }

        public void OnTurnRolledBack(Level level)
        {
        }
        public void OnAfterPlayerMove(Level level)
        {
            countEntityInPosition = 0;
            foreach (var item in level.GetActiveEntitiesAt(_entity.Position))
            {
                countEntityInPosition += 1;
            }

        }
    }
}