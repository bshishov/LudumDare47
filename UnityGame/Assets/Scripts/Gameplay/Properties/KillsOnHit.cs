using System.Collections.Generic;
using Audio;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gameplay.Properties
{
    [RequireComponent(typeof(Entity))]
    public class KillsOnHit : MonoBehaviour, ICommandHandler
    {
        public bool KillsOnCollide = true;
        public bool SelfDestroyOnHit = true;
        public bool SelfDestroyOnCollide = true;

        [Header("Sounds")] 
        public SoundAsset HitKillSound;
        public SoundAsset CollideKillSound;

        [Header("Deadly sides")]
        public bool Front = true;
        public bool Back = true;
        public bool Left = true;
        public bool Right = true;

        private Entity _entity;

        private void Awake()
        {
        }

        public void OnInitialized(Level level)
        {
            _entity = GetComponent<Entity>();
        }

        public void OnAfterPlayerMove(Level level)
        {
        }

        public IEnumerable<IChange> Handle(Level level, ICommand command)
        {
            if (command is HitCommand hitCommand && IsDeadlySide(hitCommand.Direction))
            {
                level.DispatchEarly(new DestroyCommand(hitCommand.SourceId));
                if (SelfDestroyOnHit)
                    level.DispatchEarly(new DestroyCommand(_entity.Id));
                SoundManager.Instance.Play(HitKillSound);
            }

            if (KillsOnCollide && command is CollisionEvent collisionEvent && IsDeadlySide(collisionEvent.Direction))
            {
                level.DispatchEarly(new DestroyCommand(collisionEvent.SourceId));
                if (SelfDestroyOnCollide)
                    level.DispatchEarly(new DestroyCommand(_entity.Id));
                SoundManager.Instance.Play(CollideKillSound);
            }

            yield break;
        }

        bool IsDeadlySide(Direction relativeCollisionDirection)
        {
            return (Front && relativeCollisionDirection == Direction.Front) ||
                   (Back && relativeCollisionDirection == Direction.Back) ||
                   (Left && relativeCollisionDirection == Direction.Left) ||
                   (Right && relativeCollisionDirection == Direction.Right);
        }

        public void Revert(Level level, IChange change)
        {
        }

        public void OnTurnRolledBack(Level level)
        {
        }
    }
}