using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    public interface ICommand
    {
        int TargetId { get; }
    }

    public interface IChange
    {
        int TargetId { get; }
    }

    public interface ICommandHandler
    {
        void OnTurnStarted(Level level);
        IEnumerable<IChange> Handle(Level level, ICommand command);
        void Revert(Level level, IChange change);
    }

    public abstract class BaseCommand : ICommand
    {
        public int TargetId { get; }

        protected BaseCommand(int target)
        {
            TargetId = target;
        }
    }

    public abstract class BaseChange : IChange
    {
        public int TargetId {get; }
        
        protected BaseChange(int targetId)
        {
            TargetId = targetId;
        }
    }

    public class MoveCommand : BaseCommand
    {
        public readonly Direction Direction;
        public readonly bool UpdateOrientation;

        public MoveCommand(int target, Direction direction, bool updateOrientation) : base(target)
        {
            Direction = direction;
            UpdateOrientation = updateOrientation;
        }
        
        public override string ToString()
        {
            return $"MoveCommand(id: {TargetId}, {Direction})";
        }
    }

    public class DestroyCommand : BaseCommand
    {
        public int SourceId;
        
        public DestroyCommand(int target, int sourceId) : base(target)
        {
            SourceId = sourceId;
        }
        
        public override string ToString()
        {
            return $"DestroyCommand(id: {TargetId})";
        }
    }

    public class DestroyedChange : BaseChange
    {
        public DestroyedChange(int targetId) : base(targetId)
        {
        }
    }
    
    public class HitCommand : BaseCommand
    {
        public int SourceId;
        public Direction Direction;
        
        public HitCommand(int target, int sourceId, Direction direction) : base(target)
        {
            SourceId = sourceId;
            Direction = direction;
        }
        
        public override string ToString()
        {
            return $"HitCommand(id: {TargetId})";
        }
    }

    public class MoveChange : BaseChange
    {
        public Vector2Int OriginalPosition;
        public Vector2Int TargetPosition;
        public Direction OriginalOrientation;
        public Direction TargetOrientation;

        public MoveChange(int targetId) : base(targetId)
        {
        }

        public override string ToString()
        {
            return $"MoveChange(id: {TargetId}, {OriginalPosition} -> {TargetPosition})";
        }
    }

    public class StoppedMoving : BaseChange
    {
        public StoppedMoving(int targetId) : base(targetId)
        {
        }
    }
}