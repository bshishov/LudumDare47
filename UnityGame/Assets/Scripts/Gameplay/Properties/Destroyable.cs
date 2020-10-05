using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Properties
{
    [RequireComponent(typeof(Entity))]
    public class Destroyable : MonoBehaviour, ICommandHandler
    {
        [Header("Visuals")] 
        public FxObject DestroyedFx;
        
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
            if (command is DestroyCommand)
            {
                DestroyedFx?.Trigger(transform);
                _entity.Deactivate();
                yield return new DestroyedChange(_entity.Id);
            }
        }

        public void Revert(Level level, IChange change)
        {
            if (change is DestroyedChange)
            {
                DestroyedFx?.Stop();
                _entity.Activate();
            }
        }
    }
}