using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Properties
{
    [RequireComponent(typeof(Entity))]
    public class Movable : MonoBehaviour, ICommandHandler
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
            if (command is MoveCommand moveCommand)
            {
                foreach (var change in DoMove(level, moveCommand.Direction, moveCommand.UpdateOrientation))
                    yield return change;
            }
        }

        public void Revert(Level level, IChange change)
        {
            if (change is MoveChange moveChange)
                _entity.MoveTo(moveChange.OriginalPosition, moveChange.OriginalOrientation);
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

        public IEnumerable<IChange> DoMove(Level level, Direction direction, bool updateOrientation)
        {
            if(!CanMove(level, direction))
                yield break;
            
            // Update logical position
            var targetPos = _entity.Position + MoveDelta(direction);

            // Move neighbor movable (push)
            var entityInTargetPos = level.GetEntityAt(targetPos);
            if (entityInTargetPos != null)
            {
                var movable = entityInTargetPos.GetComponent<Movable>();
                if (movable != null)
                {
                    foreach (var change in movable.DoMove(level, direction, false))
                    {
                        yield return change;
                    }
                }
            }
            
            // Move self
            targetPos = _entity.Position + MoveDelta(direction);
            var targetOrientation = _entity.Orientation;
            if (updateOrientation)
                targetOrientation = direction;
            
            var selfMove = new MoveChange(_entity.Id)
            {
                OriginalOrientation = _entity.Orientation,
                OriginalPosition = _entity.Position,
                TargetOrientation = targetOrientation,
                TargetPosition = targetPos
            };
            _entity.MoveTo(targetPos, targetOrientation);
            yield return selfMove;
        }
    }
}