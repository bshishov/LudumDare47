using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gameplay.Properties
{
    [RequireComponent(typeof(Entity))]
    public class Collectable : MonoBehaviour, ICommandHandler
    {
        private Entity _entity;

        [Header("Visuals")]
        [SerializeField]
        private FxObject _collectFx;
        [SerializeField]
        private FxObject _revertCollectFx;

        [FormerlySerializedAs("DisableRenderersWhenInactive")]
        [SerializeField]
        private Renderer[] _disableRenderersWhenInactive;

        public void OnInitialized(Level level)
        {
            _entity = GetComponent<Entity>();
        }

        public IEnumerable<IChange> Handle(Level level, ICommand command)
        {

            foreach (var entityInTargetPos in level.GetActiveEntitiesAt(_entity.Position))
            {

                if (entityInTargetPos.ObjectType.ToString() == "Player")
                {
                    if (command is HitCommand)
                    {
                        _revertCollectFx.Stop();
                        _collectFx.Trigger(transform);

                        _entity.Deactivate();

                        if (_disableRenderersWhenInactive != null)
                            foreach (var rnd in _disableRenderersWhenInactive)
                                rnd.enabled = false;

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
                _collectFx.Stop();
                _revertCollectFx.Trigger(transform);

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