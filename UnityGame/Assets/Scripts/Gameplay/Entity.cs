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
        public Animator Animator;
        public string AnimMoveTrigger;
        public string AnimDeathBool;
        public bool DisableRenderersWhenInactive = true;

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
                    Utils.LevelToWorld(Position) + Offset,
                    Utils.DirectionToRotation(Orientation),
                    Utils.LevelToWorld(tgtPosition) + Offset,
                    Utils.DirectionToRotation(tgtOrientation),
                    animationSpeed);
            }
            else
            {
                transform.position = Utils.LevelToWorld(tgtPosition) + Offset;
                transform.rotation = Utils.DirectionToRotation(tgtOrientation);
            }

            if (this.Animator != null)
                Animator.SetTrigger(AnimMoveTrigger);

            Position = tgtPosition;
            Orientation = tgtOrientation;
        }

        public void Deactivate()
        {
            IsActive = false;

            if (this.Animator != null)
                Animator.SetBool(AnimDeathBool, true);
            
            if(DisableRenderersWhenInactive)
                foreach (var rnd in gameObject.GetComponentsInChildren<Renderer>())
                    rnd.enabled = false;
        }

        public void Activate()
        {
            IsActive = true;
            
            if (this.Animator != null)
                Animator.SetBool(AnimDeathBool, false);
            
            if(DisableRenderersWhenInactive)
                foreach (var rnd in gameObject.GetComponentsInChildren<Renderer>())
                    rnd.enabled = true;
        }
        
        private void SetPositionAndOrientationFromTransform()
        {
            Position = Utils.WorldToLevel(transform.position - Offset);
            Orientation = Utils.DirectionFromForwardVector(transform.forward);
        }

        private void SetTransformFromPositionAndOrientation()
        {
            // Set world position from level position
            transform.position = Utils.LevelToWorld(Position) + Offset;
            transform.rotation = Utils.DirectionToRotation(Orientation);
        }

        private void OnDrawGizmos()
        {
            var levelPos = Utils.WorldToLevel(transform.position - Offset);
            var orientation = Utils.DirectionFromForwardVector(transform.forward);
            DrawLogicalTransformGizmos(levelPos, orientation, Color.magenta);
            //DrawLogicalTransformGizmos(Position, Orientation, Color.black);
        }

        private void DrawLogicalTransformGizmos(Vector2Int levelPos, Direction orientation, Color color)
        {
            var worldCellCenter = Utils.LevelToWorld(levelPos);
            
            Gizmos.color = color;
            Gizmos.DrawWireCube(worldCellCenter, Utils.CellSize);

            var orTop = worldCellCenter;
            var orLeft = worldCellCenter;
            var orRight = worldCellCenter;
            switch (orientation)
            {
                case Direction.Front:
                    orTop += Vector3.forward * 0.5f;
                    orLeft += Vector3.left * 0.5f;
                    orRight += Vector3.right * 0.5f;
                    break;
                case Direction.Right:
                    orTop += Vector3.right * 0.5f;
                    orLeft += Vector3.forward * 0.5f;
                    orRight += Vector3.back * 0.5f;
                    break;
                case Direction.Back:
                    orTop += Vector3.back * 0.5f;
                    orLeft += Vector3.right * 0.5f;
                    orRight += Vector3.left * 0.5f;
                    break;
                case Direction.Left:
                    orTop += Vector3.left * 0.5f;
                    orLeft += Vector3.back * 0.5f;
                    orRight += Vector3.forward * 0.5f;
                    break;
            }

            Gizmos.DrawLine(orLeft, orTop);
            Gizmos.DrawLine(orTop, orRight);
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