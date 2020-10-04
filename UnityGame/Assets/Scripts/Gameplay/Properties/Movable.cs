using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;

namespace Gameplay.Properties
{
    [RequireComponent(typeof(Entity))]
    public class Movable : MonoBehaviour, ICommandHandler
    {
        [EnumMask] public ObjectType CollidesWith = Gameplay.ObjectType.None;
        
        public bool CanPush = true;
        public bool CanBePushed = true;

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
                _entity.MoveTo(moveChange.OriginalPosition, moveChange.OriginalOrientation, 2f);
        }

        public bool CanMove(Level level, Direction dir)
        {
            // Recursive movement ability checking
            var targetPos = _entity.Position + Utils.MoveDelta(dir);
            foreach (var entityInTargetPos in level.GetActiveEntitiesAt(targetPos))
            {
                // If there is an active entity in target position
                if (entityInTargetPos != null)
                {
                    var movable = entityInTargetPos.GetComponent<Movable>();
                    if (movable != null)
                    {
                        // If we collide with object in a target position and an object can be pushed
                        // Then recursively check whether it has some space to move
                        if((CollidesWith & entityInTargetPos.ObjectType) > 0)
                        {
                            if(CanPush && movable.CanBePushed)
                                return movable.CanMove(level, dir);
                            return false;
                        }
                    }
                }
            }

            // No obstacles or no collisions
            return true;
        }

        public IEnumerable<IChange> DoMove(Level level, Direction direction, bool updateOrientation)
        {
            var canMove = CanMove(level, direction);
            
            // Update logical position
            var targetPos = _entity.Position + Utils.MoveDelta(direction);

            // Move neighbor movable (push)
            foreach (var entityInTargetPos in level.GetActiveEntitiesAt(targetPos))
            {
                if (entityInTargetPos != null)
                {
                    // If current object collides with target object
                    if((CollidesWith & entityInTargetPos.ObjectType) > 0)
                    {
                        level.DispatchEarly(new CollisionEvent(
                            target: entityInTargetPos.Id, 
                            sourceId: _entity.Id, 
                            direction: Utils.AbsoluteDirectionToRelative(direction, entityInTargetPos.Orientation)));
                        level.DispatchEarly(new CollisionEvent(
                            target:_entity.Id, 
                            sourceId: entityInTargetPos.Id, 
                            direction: Utils.AbsoluteDirectionToRelative(direction, _entity.Orientation)));
                    
                        // Push (collidable only)
                        if (canMove && CanPush)
                        {
                            var movable = entityInTargetPos.GetComponent<Movable>();
                            if (movable != null && movable.CanBePushed)
                            {
                                foreach (var change in movable.DoMove(level, direction, false))
                                {
                                    yield return change;
                                }
                            }
                        }
                    }
                }
            }

            if (!canMove)
                yield break;            

            // Finally, move self
            targetPos = _entity.Position + Utils.MoveDelta(direction);
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