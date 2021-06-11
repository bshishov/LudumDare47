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

        public void OnInitialized(Level level)
        {
            _entity = GetComponent<Entity>();
            _transform = GetComponent<Transform>();
        }

        public IEnumerable<IChange> Handle(Level level, ICommand command)
        {
            foreach (var entityInTargetPos in level.GetActiveEntitiesAt(_entity.Position))
            {

                if ((entityInTargetPos.ObjectType.ToString() != "Player") && (entityInTargetPos.ObjectType.ToString() != "Collectable"))
                {
                    if (command is HitCommand)
                    {
                        var sourceId = ((HitCommand)command).SourceId;
                        var targetId = ((HitCommand)command).TargetId;
                        Debug.Log(sourceId + "SourceId");
                        Debug.Log(targetId + "TargetId");
                        _transform.position = new Vector3(_transform.position.x, _transform.position.y + 1.5f, _transform.position.z);
                        Debug.Log("Rise");
                        yield return new Rise(_entity.Id);

                    }
                }
            }
        }

        public void Revert(Level level, IChange change)
        {
            if (change is Rise)
            {
                _entity.GetComponent<Transform>().position = new Vector3(_entity.GetComponent<Transform>().position.x, _entity.GetComponent<Transform>().position.y - 1.5f, _entity.GetComponent<Transform>().position.z);
                Debug.Log("Derise");
            }

        }

        public void OnTurnRolledBack(Level level)
        {
        }
        public void OnAfterPlayerMove(Level level)
        {
        }
    }
}