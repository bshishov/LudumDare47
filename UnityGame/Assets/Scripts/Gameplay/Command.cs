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
        void OnInitialized(Level level);
        void OnAfterPlayerMove(Level level);
        IEnumerable<IChange> Handle(Level level, ICommand command);
        void Revert(Level level, IChange change);
        void OnTurnRolledBack(Level level);
    }

    public abstract class BaseCommand : ICommand
    {
        public int TargetId { get; }

        protected BaseCommand(int targetId)
        {
            TargetId = targetId;
        }
    }

    public abstract class BaseChange : IChange
    {
        public int TargetId { get; }

        protected BaseChange(int targetId)
        {
            TargetId = targetId;
        }
    }

    public class MoveCommand : BaseCommand
    {
        public readonly Direction Direction;
        public readonly bool UpdateOrientation;
        public readonly MovementType MovementType = MovementType.Default;

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
        public DestroyCommand(int targetId) : base(targetId)
        {
        }

        public override string ToString()
        {
            return $"DestroyCommand(id: {TargetId})";
        }
    }
    public class SkipTurn : BaseCommand
    {
        public SkipTurn(int targetId) : base(targetId)
        {
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
        public readonly int SourceId;
        public readonly Direction Direction;

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

    public class CollisionEvent : BaseCommand
    {
        public int SourceId;
        public Direction Direction;

        public CollisionEvent(int target, int sourceId, Direction direction) : base(target)
        {
            SourceId = sourceId;
            Direction = direction;
        }

        public override string ToString()
        {
            return $"CollisionEvent(id: {TargetId})";
        }
    }


    public class MoveChange : BaseChange
    {
        public Vector2Int OriginalPosition;
        public Vector2Int TargetPosition;
        public Direction OriginalOrientation;
        public Direction TargetOrientation;
        public MovementType MovementType;

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

    public class Collection : BaseChange
    {
        public Collection(int targetId) : base(targetId)
        {
        }
    }

    public class Rise : BaseChange
    {
        public Rise(int targetId) : base(targetId)
        {
        }
    }

    public class SpawnCommand : BaseCommand
    {
        public Direction Direction;
        public SpawnCommand(int target, Direction direction) : base(target)
        {
            Direction = direction;
        }
    }

    public class SpawnChange : BaseChange
    {
        public readonly int SpawnedObjectId;

        public SpawnChange(int targetId, int spawnedObjectId) : base(targetId)
        {
            SpawnedObjectId = spawnedObjectId;
        }
    }


    public class DetonateCommand : BaseCommand
    {
        public DetonateCommand(int targetId) : base(targetId) { }
    }

    public class IgniteCommand : BaseCommand
    {
        public IgniteCommand(int targetId) : base(targetId) { }
    }

    public class FuseIgnited : BaseChange
    {
        public readonly int Delay;

        public FuseIgnited(int targetId, int delay) : base(targetId)
        {
            Delay = delay;
        }
    }
}