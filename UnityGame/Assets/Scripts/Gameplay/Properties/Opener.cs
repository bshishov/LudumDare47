using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Properties
{

    [RequireComponent(typeof(Entity))]
    public class Opener : MonoBehaviour, ICommandHandler
    {
        
        [SerializeField]
        private List<GameObject> _openableObjects;

        public void OnInitialized(Level level)
        {
            
        }

        

        public void OnAfterPlayerMove(Level level)
        {
            
        }

        

        public void OnTurnRolledBack(Level level)
        {
           
        }

        public void Revert(Level level, IChange change)
        {
          
        }

        public IEnumerable<IChange> Handle(Level level, ICommand command)
        {
            throw new System.NotImplementedException();
        }
    }
}