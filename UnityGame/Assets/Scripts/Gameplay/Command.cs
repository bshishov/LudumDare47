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
        ICommand Command { get; }
        int TargetId { get; }
    }

    public interface ICommandHandler
    {
        void OnTurnStarted(Level level);
        IEnumerable<IChange> Handle(Level level, ICommand command);
        void Apply(Level level, IChange change);
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
        public ICommand Command { get; }
        public int TargetId => Command.TargetId;
        
        protected BaseChange(ICommand cmd)
        {
            Command = cmd;
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
    }

    public class MoveChange : BaseChange
    {
        public Vector2Int OriginalPosition;
        public Vector2Int TargetPosition;
        public Direction OriginalOrientation;
        public Direction TargetOrientation;

        public MoveChange(ICommand cmd) : base(cmd)
        {
        }
    }
}