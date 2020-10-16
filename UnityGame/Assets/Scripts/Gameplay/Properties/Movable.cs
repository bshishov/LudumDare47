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

        public void OnInitialized(Level level)
        {
        }

        public void OnAfterPlayerMove(Level level)
        {
        }

        public IEnumerable<IChange> Handle(Level level, ICommand command)
        {
            if (command is MoveCommand moveCommand)
            {
                foreach (var change in DoMove(
                    level, 
                    moveCommand.Direction, 
                    moveCommand.UpdateOrientation, 
                    moveCommand.MovementType))
                    yield return change;
            }
        }

        public void Revert(Level level, IChange change)
        {
            if (change is MoveChange moveChange)
                _entity.MoveTo(
                    moveChange.OriginalPosition, 
                    moveChange.OriginalOrientation, 
                    moveChange.MovementType,
                    2f);
        }

        public void OnTurnRolledBack(Level level)
        {
        }

        public bool CanMove(Level level, Direction dir)
        {
            // Recursive movement ability checking
            var targetPos = _entity.Position + Utils.MoveDelta(dir);
            foreach (var entityInTargetPos in level.GetActiveEntitiesAt(targetPos))
            {
                // If we collide with object in a target position and an object can be pushed
                // Then recursively check whether it has some space to move
                if(CollisionConfig.ObjectsCollide(_entity.ObjectType, entityInTargetPos.ObjectType))
                {
                    if (CollisionConfig.CanPush(
                        _entity.ObjectType,
                        entityInTargetPos.ObjectType))
                    {
                        var movable = entityInTargetPos.GetComponent<Movable>();
                        if (movable != null)
                        {
                            return movable.CanMove(level, dir);
                        }
                    }
                    return false;
                }
            }

            // No obstacles or no collisions
            return true;
        }

        public IEnumerable<IChange> DoMove(Level level, Direction moveDirection, bool updateOrientation, MovementType movementType)
        {
            var canMove = CanMove(level, moveDirection);
            
            // Update logical position
            var targetPos = _entity.Position + Utils.MoveDelta(moveDirection);

            // Move neighbor movable (push)
            foreach (var entityInTargetPos in level.GetActiveEntitiesAt(targetPos))
            {
                // If current object collides with target object
                if(CollisionConfig.ObjectsCollide(_entity.ObjectType, entityInTargetPos.ObjectType))
                {
                    level.DispatchEarly(new CollisionEvent(
                        target: entityInTargetPos.Id, 
                        sourceId: _entity.Id, 
                        direction: Utils.AbsoluteDirectionToRelative(Utils.RevertDirection(moveDirection), entityInTargetPos.Orientation)));
                    level.DispatchEarly(new CollisionEvent(
                        target:_entity.Id, 
                        sourceId: entityInTargetPos.Id, 
                        direction: Utils.AbsoluteDirectionToRelative(moveDirection, _entity.Orientation)));
                
                    // Push (collidable only)
                    if (canMove && CollisionConfig.CanPush(
                        _entity.ObjectType,
                        entityInTargetPos.ObjectType))
                    {
                        var movable = entityInTargetPos.GetComponent<Movable>();
                        if (movable != null)
                        {
                            foreach (var change in movable.DoMove(level, moveDirection, false, MovementType.Pushed))
                            {
                                yield return change;
                            }
                        }
                    }
                }

                if (CollisionConfig.ObjectsHit(_entity.ObjectType, entityInTargetPos.ObjectType))
                {
                    level.DispatchEarly(new HitCommand(
                        target: entityInTargetPos.Id, 
                        sourceId: _entity.Id, 
                        direction: Utils.AbsoluteDirectionToRelative(Utils.RevertDirection(moveDirection), entityInTargetPos.Orientation)));
                    level.DispatchEarly(new HitCommand(
                        target:_entity.Id, 
                        sourceId: entityInTargetPos.Id, 
                        direction: Utils.AbsoluteDirectionToRelative(moveDirection, _entity.Orientation)));
                }
            }

            if (!canMove)
                yield break;  
            
            if(!_entity.IsActive)
                yield break;

            // Finally, move self
            targetPos = _entity.Position + Utils.MoveDelta(moveDirection);
            var targetOrientation = _entity.Orientation;
            if (updateOrientation)
                targetOrientation = moveDirection;
            
            var selfMove = new MoveChange(_entity.Id)
            {
                OriginalOrientation = _entity.Orientation,
                OriginalPosition = _entity.Position,
                TargetOrientation = targetOrientation,
                TargetPosition = targetPos,
                MovementType = movementType
            };
            _entity.MoveTo(targetPos, targetOrientation, movementType);
            yield return selfMove;
        }

        
    }
}