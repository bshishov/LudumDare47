using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gameplay.Properties
{
    [RequireComponent(typeof(Entity))]
    public class Destroyable : MonoBehaviour, ICommandHandler
    {
        [Header("Visuals")] 
        public FxObject DestroyedFx;
        public Animator Animator;
        public string AnimDiedBool;
        [FormerlySerializedAs("DisableRenderersWhenInactive")] 
        public bool DisableAllRenderersWhenInactive = false;
        public Renderer[] DisableRenderersWhenInactive;
        
        private Entity _entity;
        private UITimerManager _uiTimerManager;

        private void Start()
        {
        }

        public void OnInitialized(Level level)
        {
            _entity = GetComponent<Entity>();
            _uiTimerManager = GameObject.FindObjectOfType<UITimerManager>();
        }

        public void OnAfterPlayerMove(Level level)
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
                
                if(_uiTimerManager != null)
                    _uiTimerManager.DeleteTimer(gameObject);
                
                if(Animator != null)
                    Animator.SetBool(AnimDiedBool, true);
                
                if(DisableAllRenderersWhenInactive)
                    foreach (var rnd in gameObject.GetComponentsInChildren<Renderer>())
                        rnd.enabled = false;

                if (DisableRenderersWhenInactive != null)
                    foreach (var rnd in DisableRenderersWhenInactive)
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
                
                
                if(DisableAllRenderersWhenInactive)
                    foreach (var rnd in gameObject.GetComponentsInChildren<Renderer>())
                        rnd.enabled = true;
                
                if (DisableRenderersWhenInactive != null)
                    foreach (var rnd in DisableRenderersWhenInactive)
                        rnd.enabled = true;
                
                _entity.Activate();
            }
        }

        public void OnTurnRolledBack(Level level)
        {
        }
    }
}