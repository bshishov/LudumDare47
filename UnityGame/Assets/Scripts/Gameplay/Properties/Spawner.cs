using System.Collections.Generic;
using Audio;
using UnityEngine;

namespace Gameplay.Properties
{
    [RequireComponent(typeof(Entity))]
    public class Spawner : MonoBehaviour, ICommandHandler, IHasTimer
    {
        public GameObject Prefab;
        public int Delay;

        [Header("Visuals")]
        public Animator Animator;
        public string AnimOnSpawnTrigger;
        public FxObject ShootFx;

        [Header("Sounds")] 
        [SerializeField] private SoundAsset SpawnSound; 
        [SerializeField] private SoundAsset RevertSpawnSound; 

        private Entity _entity;
        private int _turnsUntilSpawn;

        public void OnInitialized(Level level)
        {
            _entity = GetComponent<Entity>();
            _turnsUntilSpawn = Delay;
        }

        public void OnAfterPlayerMove(Level level)
        {
            if (_turnsUntilSpawn == 0)
                level.Dispatch(new SpawnCommand(_entity.Id, _entity.Orientation));
            
            _turnsUntilSpawn -= 1;
        }

        public IEnumerable<IChange> Handle(Level level, ICommand command)
        {
            if (command is SpawnCommand spawnCommand)
            {
                var entity = level.Spawn(
                    Prefab,
                    _entity.Position + Utils.MoveDelta(_entity.Orientation),
                    _entity.Orientation);

                foreach (var entityInTargetPos in level.GetActiveEntitiesAt(entity.Position))
                {
                    if (entityInTargetPos.Id != entity.Id)
                    { 
                        // If current object collides with target object
                        if (CollisionConfig.ObjectsCollide(entity.ObjectType, entityInTargetPos.ObjectType))
                        {
                            level.DispatchEarly(new CollisionEvent(
                                target: entityInTargetPos.Id,
                                sourceId: entity.Id,
                                direction: Utils.AbsoluteDirectionToRelative(Utils.RevertDirection(spawnCommand.Direction), entityInTargetPos.Orientation)));
                            level.DispatchEarly(new CollisionEvent(
                                target: entity.Id,
                                sourceId: entityInTargetPos.Id,
                                direction: Utils.AbsoluteDirectionToRelative(spawnCommand.Direction, entity.Orientation)));
                        }
                        if (CollisionConfig.ObjectsHit(entity.ObjectType, entityInTargetPos.ObjectType))
                        {
                            level.DispatchEarly(new HitCommand(
                                target: entityInTargetPos.Id,
                                sourceId: entity.Id,
                                direction: Utils.AbsoluteDirectionToRelative(Utils.RevertDirection(spawnCommand.Direction), entityInTargetPos.Orientation)));
                            level.DispatchEarly(new HitCommand(
                                target: entity.Id,
                                sourceId: entityInTargetPos.Id,
                                direction: Utils.AbsoluteDirectionToRelative(spawnCommand.Direction, entity.Orientation)));
                        }
                    }
                }

                if (entity != null)
                {
                    if (Animator != null)
                        Animator.SetTrigger(AnimOnSpawnTrigger);

                    SoundManager.Instance.Play(SpawnSound);
                    ShootFx?.Trigger(transform);

                    yield return new SpawnChange(_entity.Id, entity.Id);
                }
            }
        }

        public void Revert(Level level, IChange change)
        {
            if (change is SpawnChange spawnChange)
            {
                SoundManager.Instance.Play(RevertSpawnSound);
                level.Kill(spawnChange.SpawnedObjectId);
            }
        }

        public void OnTurnRolledBack(Level level)
        {
            _turnsUntilSpawn += 1;
        }

        public int? GetCurrentTimerValue()
        {
            if (_turnsUntilSpawn >= 0) 
                return _turnsUntilSpawn;
            return null;
        }
    }
}