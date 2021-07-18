using System;
using Gameplay;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Buttons
{
    public class UIRollbackButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public GameObject Glow;
        private bool _buttonPressed;

        private void Start()
        {
            Common.LevelStateChanged += OnLevelStateChanged;
        }

        private void OnDestroy()
        {
            Common.LevelStateChanged -= OnLevelStateChanged;
        }
        
        private void OnLevelStateChanged(Level level, Level.GameState state)
        {
            if (state == Level.GameState.PlayerDied || state == Level.GameState.CatGirlDied)
            {
                Glow.SetActive(true);
            }
            else
            {
                Glow.SetActive(false);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _buttonPressed = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _buttonPressed = false;
        }

        private void Update() 
        {
            if (_buttonPressed && Common.CurrentLevel != null) 
            {
                Common.CurrentLevel.PlayerRollback();
            }
        }
    }
}
