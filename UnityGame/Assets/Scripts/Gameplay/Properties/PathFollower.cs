using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gameplay.Properties
{
    [RequireComponent(typeof(Entity))]
    public class PathFollower : MonoBehaviour, ICommandHandler
    {
        public string PathName = "Rails";
        public bool DestroyIfNowhereToMove = false;
        private Entity _entity;

        private void Start()
        {
            _entity = GetComponent<Entity>();
        }

        public void OnInitialized(Level level)
        {
        }

        public void OnAfterPlayerMove(Level level)
        {
            var currentPathNode = GetPathNodeAt(level, _entity.Position);
            if (currentPathNode != null)
            {
                var directionToNextNode = currentPathNode.GetNextPathNodeDirection(level);
                if (directionToNextNode.HasValue)
                {
                    level.Dispatch(new MoveCommand(_entity.Id, directionToNextNode.Value, true));
                }
                else if (DestroyIfNowhereToMove)
                    level.Dispatch(new DestroyCommand(_entity.Id));
            }
        }

        private PathNode GetPathNodeAt(Level level, Vector2Int position)
        {
            return level.GetActiveEntitiesAt(position)
                .Select(entity => entity.GetComponent<PathNode>())
                .FirstOrDefault(pathNode => pathNode != null && pathNode.PathName == PathName);
        }

        public IEnumerable<IChange> Handle(Level level, ICommand command)
        {
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