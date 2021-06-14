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
        private int _countEntityInPosition;
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
                MoveDown();
            }
        }

        public void OnAfterPlayerMove(Level level)
        {
            _countEntityInPosition = 0;
            foreach (var item in level.GetActiveEntitiesAt(_entity.Position))
            {
                if (item.ObjectType.ToString() == "Rails") {
                    _countEntityInPosition -= 1;
                }
                _countEntityInPosition += 1;
            }

            if (_countEntityInPosition <= 1 && _isRaised)
            {
                MoveDown();
            }
        }
        public void OnTurnRolledBack(Level level)
        {
        }

        private void MoveDown()
        {
            if (_isRaised)
            {
                _transform.position = new Vector3(_transform.position.x, _transform.position.y - 1.5f, _transform.position.z);
                _isRaised = false;
            }
        }

    }

}