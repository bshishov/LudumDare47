using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace UI
{
    public class UITimerManager : MonoBehaviour
    {
        public RectTransform TimersOverlay;
        public GameObject TimerPrefab;
    
        private readonly Dictionary<int, UITimer> _timerObjects = new Dictionary<int, UITimer>();

        public void SetTimer(GameObject parent, int timerValue)
        {
            if(TimerPrefab == null || TimersOverlay == null || parent == null)
                return;

            var timer = GetOrCreateUiTimer(parent);
            if (timer != null)
                timer.SetTurns(timerValue);
        }

        private UITimer GetOrCreateUiTimer(GameObject parent)
        {
            var key = parent.GetInstanceID();

            if (_timerObjects.ContainsKey(key))
                return _timerObjects[key];
        
            var uiFollowObj = Instantiate(TimerPrefab, TimersOverlay);
            var uiFollow = uiFollowObj.GetComponent<UIFollowSceneObject>();
            if(uiFollow != null)
                uiFollow.SetTarget(parent.transform);
            var timer = uiFollow.GetComponent<UITimer>();
            if(timer != null)
                _timerObjects.Add(key, timer);
            return timer;
        }
    
        public void DeleteTimer(GameObject parent)
        {
            var key = parent.GetInstanceID();
            if (_timerObjects.ContainsKey(key))
            {
                GameObject.Destroy(_timerObjects[key].gameObject);
                _timerObjects.Remove(key);
            }
        }
    }
}
