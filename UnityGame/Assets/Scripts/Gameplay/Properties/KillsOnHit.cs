using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Properties
{
    [RequireComponent(typeof(Entity))]
    public class KillsOnHit : MonoBehaviour, ICommandHandler
    {
        public bool SelfDestroyOnHit = true;
        
        [Header("Deadly sides")]
        public bool Front = true;
        public bool Back = true;
        public bool Left = true;
        public bool Right = true;
        
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
            if (command is HitCommand hitCommand)
            {
                // TODO: Side detection
                level.DispatchEarly(new DestroyCommand(target: hitCommand.SourceId, sourceId: _entity.Id));
                if (SelfDestroyOnHit)
                {
                    level.DispatchEarly(new DestroyCommand(target: _entity.Id, sourceId: _entity.Id));
                }
            }
            yield break;
        }

        public void Revert(Level level, IChange change)
        {
        }
    }
}