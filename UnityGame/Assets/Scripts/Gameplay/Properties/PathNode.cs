using UnityEngine;

namespace Gameplay.Properties
{
    [RequireComponent(typeof(Entity))]
    public class PathNode : MonoBehaviour
    {
        public string PathName = "Rails";

        [Header("Next")] 
        public bool NextFront = true;
        public bool NextLeft = false;
        public bool NextRight = false;
        public bool NextBack = false;

        private Entity _entity;

        private void Start()
        {
            _entity = GetComponent<Entity>();
        }

        public Direction? GetNextPathNodeDirection(Level level)
        {
            Direction? relative = null;
            if (NextFront && CanMoveTo(level, Direction.Front))
                relative = Direction.Front;

            if(NextBack && CanMoveTo(level, Direction.Back))
                relative = Direction.Back;

            if(NextLeft && CanMoveTo(level, Direction.Left))
                relative = Direction.Left;

            if(NextRight && CanMoveTo(level, Direction.Right))
                relative = Direction.Right;

            if (relative.HasValue)
                return Utils.RelativeDirectionToAbsolute(relative.Value, _entity.Orientation);

            return null;
        }

        private bool CanMoveTo(Level level, Direction relativeDirection)
        {
            var direction = Utils.RelativeDirectionToAbsolute(relativeDirection, _entity.Orientation);
            var nextPos = _entity.Position + Utils.MoveDelta(direction);
                
            foreach (var entity in level.GetActiveEntitiesAt(nextPos))
            {
                var pathNode = entity.GetComponent<PathNode>();
                if (pathNode != null && pathNode.PathName == PathName)
                    return true;
            }

            return false;
        }
    }
}