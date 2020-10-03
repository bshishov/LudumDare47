using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Properties
{
    [RequireComponent(typeof(Entity))]
    public class Movable : MonoBehaviour, ICommandHandler
    {
        public bool IsProjectile = false;
        private Entity _entity;
        private bool _isMoving = true;

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
                _entity.MoveTo(moveChange.OriginalPosition, moveChange.OriginalOrientation, 2f);
            else if (change is StoppedMoving)
            {
                _isMoving = true;
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
            var entityInTargetPos = level.GetActiveEntityAt(targetPos);
            if (entityInTargetPos != null)
            {
                var movable = entityInTargetPos.GetComponent<Movable>();
                if(movable != null)
                    return movable.CanMove(level, dir);

                var obstacle = entityInTargetPos.GetComponent<Obstacle>();
                if (obstacle != null)
                {
                    if (IsProjectile)
                        return !obstacle.BlockProjectiles;
                    return !obstacle.BlockObjects;
                }
            }

            // No obstacles
            return true;
        }

        public IEnumerable<IChange> DoMove(Level level, Direction direction, bool updateOrientation)
        {
            if(!_isMoving)
                yield break;


            var canMove = CanMove(level, direction);
            
            // Update logical position
            var targetPos = _entity.Position + MoveDelta(direction);

            // Move neighbor movable (push)
            var entityInTargetPos = level.GetActiveEntityAt(targetPos);
            if (entityInTargetPos != null)
            {
                // Collisions
                level.DispatchEarly(new HitCommand(entityInTargetPos.Id, _entity.Id, direction));
                level.DispatchEarly(new HitCommand(_entity.Id, entityInTargetPos.Id, RevertDirection(direction)));
                
                // Push
                if (canMove)
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
            }
            
            // If we were moving but cant move now - stop
            if (!canMove)
            {
                yield return new StoppedMoving(_entity.Id);
                yield break;
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

        private static Direction RevertDirection(Direction direction)
        {
            switch (direction)
            {
                case Direction.Down:
                    return Direction.Up;
                case Direction.Up:
                    return Direction.Down;
                case Direction.Left:
                    return Direction.Right;
                case Direction.Right:
                    return Direction.Left;
                default:
                    return direction;
            }
        }
    }
}