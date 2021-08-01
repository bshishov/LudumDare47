using System;
using System.Collections.Generic;
using Gameplay;
using Gameplay.Properties;
using UnityEngine;
using Utils;

namespace UI
{
    public class UITimerManager : MonoBehaviour
    {
        private class TrackedTimer
        {
            public Entity Entity;
            public IHasTimer TimerValueProvider;
            public UITimer UiTimerObject;
        }
        
        public RectTransform TimersOverlay;
        public GameObject TimerPrefab;
    
        private readonly Dictionary<int, TrackedTimer> _timers = new Dictionary<int, TrackedTimer>();

        private void Start()
        {
            Common.LevelEntitySpawned += OnLevelEntitySpawned;
            Common.LevelEntityKilled += OnLevelEntityKilled;
            Common.LevelChanged += OnLevelChanged;
            Common.LevelTurnRolledBack += OnLevelTurnRolledBack;
            Common.LevelTurnCompleted += OnLevelTurnCompleted;
            
            if (Common.CurrentLevel != null)
                CollectAllEntitiesWithTimer(Common.CurrentLevel);
        }

        private void OnDestroy()
        {
            Common.LevelEntitySpawned -= OnLevelEntitySpawned;
            Common.LevelEntityKilled -= OnLevelEntityKilled;
            Common.LevelChanged -= OnLevelChanged;
            Common.LevelTurnRolledBack -= OnLevelTurnRolledBack;
            Common.LevelTurnCompleted -= OnLevelTurnCompleted;
        }

        private void OnLevelTurnRolledBack(Level level)
        {
            UpdateTrackedItems();
        }
        
        private void OnLevelTurnCompleted(Level level)
        {
            UpdateTrackedItems();
        }

        private void UpdateTrackedItems()
        {
            foreach (var trackedTimer in _timers.Values)
            {
                if (trackedTimer.Entity.IsActive)
                {
                    var value = trackedTimer.TimerValueProvider.GetCurrentTimerValue();
                    trackedTimer.UiTimerObject.SetTurns(value);
                }
                else
                {
                    trackedTimer.UiTimerObject.SetTurns(null);
                }
            }
        }

        private void OnLevelChanged(Level level)
        {
            foreach (var trackedTimer in _timers.Values)
                Destroy(trackedTimer.UiTimerObject);
            _timers.Clear();

            if (level != null)
                CollectAllEntitiesWithTimer(level);
        }

        private void CollectAllEntitiesWithTimer(Level level)
        {
            foreach (var entity in level.GetAllActiveEntities())
            {
                TrackTimer(entity);
            }
        }

        private void OnLevelEntitySpawned(Level level, Entity entity)
        {
            TrackTimer(entity);
        }
        
        private void OnLevelEntityKilled(Level level, Entity entity)
        {
            if (_timers.ContainsKey(entity.Id))
            {
                var trackedTimer = _timers[entity.Id];
                Destroy(trackedTimer.UiTimerObject);
                _timers.Remove(entity.Id);
            }
        }

        private void TrackTimer(Entity entity)
        {
            if (_timers.ContainsKey(entity.Id))
            {
                Debug.LogWarning($"Trying to add a UI timer for an already tracked entity {entity}");
                return;
            }

            var timerValueProvider = entity.GetComponent<IHasTimer>();
            if (timerValueProvider != null)
            {
                var uiTimerObject = GetOrCreateUiTimer(entity.gameObject);
                uiTimerObject.SetTurns(timerValueProvider.GetCurrentTimerValue());
                var trackedTimer = new TrackedTimer
                {
                    TimerValueProvider = timerValueProvider,
                    UiTimerObject = uiTimerObject,
                    Entity = entity
                };
                _timers.Add(entity.Id, trackedTimer);
            }
        }

        private UITimer GetOrCreateUiTimer(GameObject followTarget)
        {
            var uiFollowObj = Instantiate(TimerPrefab, TimersOverlay);
            var uiFollow = uiFollowObj.GetComponent<UIFollowSceneObject>();
            uiFollow.SetTarget(followTarget.transform);
            return uiFollow.GetComponent<UITimer>();
        }
    }
}
