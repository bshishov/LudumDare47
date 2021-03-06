﻿using System.Collections.Generic;
using Audio;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gameplay.Properties
{
    [RequireComponent(typeof(Entity))]
    public class Collectable : MonoBehaviour, ICommandHandler
    {
        private Entity _entity;

        [Header("Sounds")] 
        [SerializeField] private SoundAsset PickupSound;
        [SerializeField] private SoundAsset RevertPickupSound;
        
        [Header("Visuals")]
        [SerializeField]
        private FxObject _collectFx;
        [SerializeField]
        private FxObject _revertCollectFx;
        [SerializeField]
        private FxObject _haloFx;

        [FormerlySerializedAs("DisableRenderersWhenInactive")]
        [SerializeField]
        private Renderer[] _disableRenderersWhenInactive;

        public void OnInitialized(Level level)
        {
            _entity = GetComponent<Entity>();
            _haloFx.Trigger(transform);
        }

        public IEnumerable<IChange> Handle(Level level, ICommand command)
        {
            if (command is HitCommand)
            {
                foreach (var entityInTargetPos in level.GetActiveEntitiesAt(_entity.Position))
                {

                    if (entityInTargetPos.ObjectType.ToString() == "Player")
                    {
                        _haloFx.Stop();
                        _revertCollectFx.Stop();
                        _collectFx.Trigger(transform);

                        _entity.Deactivate();

                        if (_disableRenderersWhenInactive != null)
                            foreach (var rnd in _disableRenderersWhenInactive)
                                rnd.enabled = false;

                        SoundManager.Instance.Play(PickupSound);
                        level.CollectStar();

                        yield return new Collection(_entity.Id);

                    }
                }
            }
        }

        public void Revert(Level level, IChange change)
        {
            if (change is Collection)
            {
                SoundManager.Instance.Play(RevertPickupSound);
                _collectFx.Stop();
                _revertCollectFx.Trigger(transform);
                _haloFx.Trigger(transform);
                if (_disableRenderersWhenInactive != null)
                    foreach (var rnd in _disableRenderersWhenInactive)
                        rnd.enabled = true;

                _entity.Activate();

                level.LoseStar();
            }
        }

        public void OnAfterPlayerMove(Level level)
        {
        }

        public void OnTurnRolledBack(Level level)
        {
        }
    }
}