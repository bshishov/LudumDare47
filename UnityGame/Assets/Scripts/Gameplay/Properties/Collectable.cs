using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gameplay.Properties
{
    [RequireComponent(typeof(Entity))]
    public class Collectable : MonoBehaviour, ICommandHandler
    {
        [Header("Visuals")]
        private Entity _entity;

        [SerializeField]
        private FxObject _collectFx;
        [SerializeField]
        private FxObject _revertCollectFx;


        [FormerlySerializedAs("DisableRenderersWhenInactive")]
        public Renderer[] DisableRenderersWhenInactive;

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
                        _collectFx.Trigger(transform);
                        _entity.Deactivate();

                        if (DisableRenderersWhenInactive != null)
                            foreach (var rnd in DisableRenderersWhenInactive)
                                rnd.enabled = false;

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

                if (DisableRenderersWhenInactive != null)
                    foreach (var rnd in DisableRenderersWhenInactive)
                        rnd.enabled = true;

                _entity.Activate();
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