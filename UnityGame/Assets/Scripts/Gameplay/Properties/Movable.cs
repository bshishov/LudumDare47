using UnityEngine;

namespace Gameplay.Properties
{
    [RequireComponent(typeof(Entity))]
    public class Movable : MonoBehaviour, ICommandHandler
    {
        public static float MoveTime = 0.3f;
        private Entity _entity;

        private void Start()
        {
            _entity = GetComponent<Entity>();
        }
        
        public void Handle(Level level, ICommand command)
        {
            if (command is MoveCommand moveCommand && CanMove(level, moveCommand.Direction))
            {
               DoMove(level, moveCommand.Direction, moveCommand.UpdateOrientation);
            }
        }

        public void Revert(Level level, ICommand command)
        {
            if (command is MoveCommand moveCommand)
            {
                
            }
        }

        public static Vector2Int MoveDelta(Direction dir)
        {
            switch (dir)
            {
                case Direction.Up:
                    return Vector2Int.up;
                case Direction.Right:
                    return Vector2Int.right;
                case Direction.Down:
                    return Vector2Int.down;
                case Direction.Left:
                    return Vector2Int.left;
                default:
                    return Vector2Int.zero;
            }
        }

        public bool CanMove(Level level, Direction dir)
        {
            // Recursive movement ability checking
            var targetPos = _entity.Position + MoveDelta(dir);
            var entityInTargetPos = level.GetEntityAt(targetPos);
            if (entityInTargetPos != null)
            {
                var movable = entityInTargetPos.GetComponent<Movable>();
                if(movable != null)
                    return movable.CanMove(level, dir);
            }

            // TODO: Check walls and non-movable
            return true;
        }

        private void DoMove(Level level, Direction modeDirection, bool updateOrientation)
        {
            var targetPos = _entity.Position + MoveDelta(modeDirection);
            var entityInTargetPos = level.GetEntityAt(targetPos);
            if (entityInTargetPos != null)
            {
                // Move neighbor movable (push)
                level.Dispatch(new MoveCommand(entityInTargetPos.Id, modeDirection, false));
            }

            // Update logical position
            var targetOrientation = _entity.Orientation;
            if (updateOrientation)
                targetOrientation = modeDirection;

            _entity.MoveTo(targetPos, targetOrientation);
        }
    }
}