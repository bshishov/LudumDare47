using UnityEngine;

namespace Gameplay.Properties
{
    [RequireComponent(typeof(Movable))]
    public class PlayerControlled : MonoBehaviour, ICommandHandler
    {
        private Entity _entity;
        
        private void Start()
        {
            _entity = GetComponent<Entity>();
        }
        
        public void Handle(Level level, ICommand command)
        {
        }
    }
}