using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Properties
{
    [RequireComponent(typeof(Entity))]
    public class DelayedAutoDestroy : MonoBehaviour, ICommandHandler
    {
        public int Delay = 3;
        
        private Entity _entity;

        private void Start()
        {
            _entity = GetComponent<Entity>();
        }

        public void OnInitialized(Level level)
        {
        }

        public void OnTurnStarted(Level level)
        {
            var currentTurn = level.GetCurrentTurn();
            if (currentTurn.Number == Delay)
            {
                level.Dispatch(new DestroyCommand(_entity.Id));
            }
        }

        public IEnumerable<IChange> Handle(Level level, ICommand command)
        {
            yield break;
        }

        public void Revert(Level level, IChange change)
        {
        }

        public void OnTurnRolledBack(Level level)
        {
        }
    }
}