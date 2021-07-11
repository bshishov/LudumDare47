using System.Collections.Generic;
using Audio;
using UnityEngine;

namespace Gameplay.Properties
{
    [RequireComponent(typeof(Entity))]
    public class Explodable : MonoBehaviour, ICommandHandler
    {
        public int Radius = 1;
        public bool ExplodeOnDestroy = true;

        [Header("Sounds")] 
        public SoundAsset ExplosionSound;
        
        private Entity _entity;

        private void Start()
        {
            _entity = GetComponent<Entity>();
        }

        public void OnInitialized(Level level)
        {
        }

        public void OnAfterPlayerMove(Level level)
        {
        }

        public IEnumerable<IChange> Handle(Level level, ICommand command)
        {
            if (ExplodeOnDestroy && command is DestroyCommand)
                Explode(level);
            else if(command is DetonateCommand)
                Explode(level);

            yield break;
        }

        private void Explode(Level level)
        {
            SoundManager.Instance.Play(ExplosionSound);
            foreach (var target in level.GetActiveEntitiesInRadius(_entity.Position, Radius))
                level.DispatchEarly(new DestroyCommand(target.Id));
        }

        public void Revert(Level level, IChange change)
        {
        }

        public void OnTurnRolledBack(Level level)
        {
        }
    }
}