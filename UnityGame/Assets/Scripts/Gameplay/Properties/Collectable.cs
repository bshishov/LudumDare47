using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Properties
{
    public class Collectable : MonoBehaviour, ICommandHandler
    {

        private void Start()
        {

        }

        private void Update()
        {

        }

        public IEnumerable<IChange> Handle(Level level, ICommand command)
        {
            throw new System.NotImplementedException();
        }

        public void OnAfterPlayerMove(Level level)
        {
            throw new System.NotImplementedException();
        }

        public void OnInitialized(Level level)
        {
            throw new System.NotImplementedException();
        }

        public void OnTurnRolledBack(Level level)
        {
            throw new System.NotImplementedException();
        }

        public void Revert(Level level, IChange change)
        {
            throw new System.NotImplementedException();
        }

       
    }
}