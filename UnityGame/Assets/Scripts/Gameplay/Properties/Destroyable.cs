using System.Collections.Generic;
using Audio;
using UI;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gameplay.Properties
{
    [RequireComponent(typeof(Entity))]
    public class Destroyable : MonoBehaviour, ICommandHandler
    {
        [Header("Sounds")] 
        [SerializeField] private SoundAsset DestroySound; 
        [SerializeField] private SoundAsset RevertDestroySound; 
        
        [Header("Visuals")] 
        public FxObject DestroyedFx;
        public FxObject RevertDestroyedFx;
        public Animator Animator;
        public string AnimDiedBool;
        [FormerlySerializedAs("DisableRenderersWhenInactive")] 
        public bool DisableAllRenderersWhenInactive = false;
        public Renderer[] DisableRenderersWhenInactive;
        
        private Entity _entity;

        private void Start()
        {
        }

        public void OnInitialized(Level level)
        {
            _entity = GetComponent<Entity>();
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

                RevertDestroyedFx?.Stop();
                DestroyedFx?.Trigger(transform);
                _entity.Deactivate();
                
                //if(_uiTimerManager != null)
                    //_uiTimerManager.DeleteTimer(gameObject);
                
                if(Animator != null)
                    Animator.SetBool(AnimDiedBool, true);
                
                if(DisableAllRenderersWhenInactive)
                    foreach (var rnd in gameObject.GetComponentsInChildren<Renderer>())
                        rnd.enabled = false;

                if (DisableRenderersWhenInactive != null)
                    foreach (var rnd in DisableRenderersWhenInactive)
                        rnd.enabled = false;

                SoundManager.Instance.Play(DestroySound);

                yield return new DestroyedChange(_entity.Id);
            }
        }

        public void Revert(Level level, IChange change)
        {
            if (change is DestroyedChange)
            {
                DestroyedFx?.Stop();
                RevertDestroyedFx?.Trigger(transform);
                if (Animator != null)
                    Animator.SetBool(AnimDiedBool, false);
                
                
                if(DisableAllRenderersWhenInactive)
                    foreach (var rnd in gameObject.GetComponentsInChildren<Renderer>())
                        rnd.enabled = true;
                
                if (DisableRenderersWhenInactive != null)
                    foreach (var rnd in DisableRenderersWhenInactive)
                        rnd.enabled = true;
                
                SoundManager.Instance.Play(RevertDestroySound);
                
                _entity.Activate();
            }
        }

        public void OnTurnRolledBack(Level level)
        {
        }
    }
}