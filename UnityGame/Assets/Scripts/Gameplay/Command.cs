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
}