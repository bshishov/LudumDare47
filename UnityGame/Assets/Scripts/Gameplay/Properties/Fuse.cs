using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Properties
{
    [RequireComponent(typeof(Entity))]
    public class Fuse : MonoBehaviour, ICommandHandler
    {
        public int Delay = 3;
        public bool AutoIgnite = true;
        
        public bool Ignited { get; private set; }
        
        private int _igniteTurn;
        private Entity _entity;

        private void Start()
        {
            _entity = GetComponent<Entity>();
        }

        public void OnTurnStarted(Level level)
        {
            if (!Ignited)
            {
                if (AutoIgnite)
                    level.DispatchEarly(new IgniteCommand(_entity.Id));
            }
            else
            {
                if(level.CurrentTurn - _igniteTurn >= Delay)
                    level.DispatchEarly(new DetonateCommand(_entity.Id));
            }
        }

        public IEnumerable<IChange> Handle(Level level, ICommand command)
        {
            if (command is IgniteCommand && !Ignited)
            {
                Ignited = true;
                _igniteTurn = level.CurrentTurn;
                yield return new FuseIgnited(_entity.Id, Delay);
            }
        }

        public void Revert(Level level, IChange change)
        {
            if (change is FuseIgnited)
            {
                Ignited = false;
            }
        }
    }
}