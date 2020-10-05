using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Properties
{
    [RequireComponent(typeof(Entity))]
    public class Destroyable : MonoBehaviour, ICommandHandler
    {
        [Header("Visuals")] 
        public GameObject DestroyedFx;
        
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
                if (DestroyedFx != null)
                    Instantiate(DestroyedFx, transform.position, transform.rotation);
                
                _entity.Deactivate();
                yield return new DestroyedChange(_entity.Id);
            }
        }

        public void Revert(Level level, IChange change)
        {
            if (change is DestroyedChange)
            {
                _entity.Activate();
            }
        }
    }
}