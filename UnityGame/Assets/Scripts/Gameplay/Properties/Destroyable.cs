using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Properties
{
    [RequireComponent(typeof(Entity))]
    public class Destroyable : MonoBehaviour, ICommandHandler
    {
        [Header("Visuals")] 
        public FxObject DestroyedFx;
        public Animator Animator;
        public string AnimDiedBool;
        public bool DisableRenderersWhenInactive = false;
        
        private Entity _entity;

        private void Start()
        {
            _entity = GetComponent<Entity>();
        }

        public void OnTurnStarted(Level level)
        {
        }

        public IEnumerable<IChange> Handle(Level level, ICommand command)
        {
            if (_entity == null)
                yield break;
            
            if (command is DestroyCommand)
            {
                DestroyedFx?.Trigger(transform);
                _entity.Deactivate();
                if(Animator != null)
                    Animator.SetBool(AnimDiedBool, true);
                
                if(DisableRenderersWhenInactive)
                    foreach (var rnd in gameObject.GetComponentsInChildren<Renderer>())
                        rnd.enabled = false;
                
                yield return new DestroyedChange(_entity.Id);
            }
        }

        public void Revert(Level level, IChange change)
        {
            if (change is DestroyedChange)
            {
                DestroyedFx?.Stop();
                
                if(Animator != null)
                    Animator.SetBool(AnimDiedBool, false);
                
                
                if(DisableRenderersWhenInactive)
                    foreach (var rnd in gameObject.GetComponentsInChildren<Renderer>())
                        rnd.enabled = true;
                
                _entity.Activate();
            }
        }
    }
}