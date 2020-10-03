using UnityEngine;

namespace Gameplay
{
    public interface ICommand
    {
        int TargetId { get; }
    }

    public interface ICommandHandler
    {
        void Handle(Level level, ICommand command);
        void Revert(Level level, ICommand command);
    }

    public abstract class BaseCommand : ICommand
    {
        public int TargetId { get; private set; }

        protected BaseCommand(int target)
        {
            TargetId = target;
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
}