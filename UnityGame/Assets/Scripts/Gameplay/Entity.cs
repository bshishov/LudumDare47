using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Gameplay
{
    public class Entity : MonoBehaviour
    {
        [EnumMask] public ObjectType ObjectType = Gameplay.ObjectType.Wall;
        public Vector2Int Position { get; private set; }
        public Direction Orientation { get; private set; }
        public bool IsActive { get; private set; }

        [Header("Visuals")]
        public Vector3 Offset = Vector3.zero;
        public bool AlignOnStart = true;

        private ICommandHandler[] _handlers;

        public int Id { get; private set; }  // Level-bound id

        private MovementAnimator _movementAnimator;

        private void Start()
        {
            _movementAnimator = GetComponent<MovementAnimator>();
            _handlers = gameObject.GetComponentsInChildren<ICommandHandler>();
        }

        public void Initialize(int id)
        {
            Id = id;
            IsActive = true;
            if (AlignOnStart)
                Align();
        }

        public IEnumerable<IChange> Execute(Level level, ICommand command)
        {
            foreach (var commandHandler in _handlers)
            {
                foreach (var change in commandHandler.Handle(level, command))
                {
                    yield return change;
                }
            }
        }

        public void Revert(Level level, IChange change)
        {
            foreach (var commandHandler in _handlers)
            {
                commandHandler.Revert(level, change);
            }
        }

        public void OnTurnStarted(Level level)
        {
            foreach (var commandHandler in _handlers)
            {
                commandHandler.OnTurnStarted(level);
            }
        }

        public void MoveTo(Vector2Int tgtPosition, Direction tgtOrientation, float animationSpeed = 1f)
        {
            if (_movementAnimator != null)
            {
                _movementAnimator.StartAnimation(
                    Level.LevelToWorld(Position) + Offset,
                    Level.DirectionToRotation(Orientation),
                    Level.LevelToWorld(tgtPosition) + Offset,
                    Level.DirectionToRotation(tgtOrientation),
                    animationSpeed);
            }
            else
            {
                transform.position = Level.LevelToWorld(tgtPosition) + Offset;
                transform.rotation = Level.DirectionToRotation(tgtOrientation);
            }

            Position = tgtPosition;
            Orientation = tgtOrientation;
        }

        public void Deactivate()
        {
            IsActive = false;
            
            // TODO: Animate
            foreach (var rnd in gameObject.GetComponentsInChildren<Renderer>())
                rnd.enabled = false;
        }

        public void Activate()
        {
            IsActive = true;
            
            // TODO: Animate
            foreach (var rnd in gameObject.GetComponentsInChildren<Renderer>())
                rnd.enabled = true;
        }
        
        private void SetPositionAndOrientationFromTransform()
        {
            Position = Level.WorldToLevel(transform.position - Offset);
            Orientation = Level.DirectionFromForwardVector(transform.forward);
        }

        private void SetTransformFromPositionAndOrientation()
        {
            // Set world position from level position
            transform.position = Level.LevelToWorld(Position) + Offset;
            transform.rotation = Level.DirectionToRotation(Orientation);
        }

        private void OnDrawGizmos()
        {
            var levelPos = Level.WorldToLevel(transform.position - Offset);
            var orientation = Level.DirectionFromForwardVector(transform.forward);
            DrawLogicalTransformGizmos(levelPos, orientation, Color.magenta);
            DrawLogicalTransformGizmos(Position, Orientation, Color.black);
        }

        private void DrawLogicalTransformGizmos(Vector2Int levelPos, Direction orientation, Color color)
        {
            var worldCellCenter = Level.LevelToWorld(levelPos);
            
            Gizmos.color = color;
            Gizmos.DrawWireCube(worldCellCenter, Level.CellSize);

            var orientationGizmoPosition = worldCellCenter;
            switch (orientation)
            {
                case Direction.Front:
                    orientationGizmoPosition += Vector3.forward * 0.5f; 
                    break;
                case Direction.Right:
                    orientationGizmoPosition += Vector3.right * 0.5f;
                    break;
                case Direction.Back:
                    orientationGizmoPosition += Vector3.back * 0.5f;
                    break;
                case Direction.Left:
                    orientationGizmoPosition += Vector3.left * 0.5f;
                    break;
            }
            
            Gizmos.DrawSphere(orientationGizmoPosition, 0.2f);
        }

        [ContextMenu("Align")]
        private void Align()
        {
            // Transform to Level
            SetPositionAndOrientationFromTransform();
            
            // Level to transform
            SetTransformFromPositionAndOrientation();
        }
    }
}