using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Properties
{
    [RequireComponent(typeof(Entity))]
    public class Spawner : MonoBehaviour, ICommandHandler
    {
        public GameObject Prefab;
        public int Delay = 0;

        [Header("Visuals")]
        public Animator Animator;
        public string AnimOnSpawnTrigger;
        public ParticleSystem ShootParticles;
        public GameObject ShootFx;
        
        private Entity _entity;
        private ObjectType _prefabObjectType = ObjectType.Default;
        private ParticleSystem _shootParticles;
        private int? _createdTurn;

        private void Start()
        {
            _entity = GetComponent<Entity>();
            if (ShootParticles != null)
            {
                _shootParticles = Instantiate(ShootParticles, transform);
                _shootParticles.Stop();
            }

            if (Prefab != null)
            {
                var entity = Prefab.GetComponent<Entity>();
                if (entity != null)
                {
                    _prefabObjectType = entity.ObjectType;
                }
            }
        }
        
        public void OnTurnStarted(Level level)
        {
            if (!_createdTurn.HasValue)
                _createdTurn = level.CurrentTurn;

            if (level.CurrentTurn - _createdTurn == Delay)
                level.Dispatch(new SpawnCommand(_entity.Id));
        }

        public IEnumerable<IChange> Handle(Level level, ICommand command)
        {
            if (command is SpawnCommand spawnCommand)
            {
                var entity = level.Spawn(
                    Prefab, 
                    _entity.Position + Utils.MoveDelta(_entity.Orientation),
                    _entity.Orientation);

                if (entity != null)
                {
                    if(_shootParticles != null)
                        _shootParticles.Emit(1);
                    
                    if (Animator != null)
                        Animator.SetTrigger(AnimOnSpawnTrigger);

                    if (ShootFx != null)
                        Instantiate(ShootFx, transform);
                    
                    yield return new SpawnChange(_entity.Id, entity.Id);
                }
            }
        }

        public void Revert(Level level, IChange change)
        {
            if (change is SpawnChange spawnChange)
            {
                level.DestroyEntity(spawnChange.SpawnedObjectId);
            }
        }
    }
}