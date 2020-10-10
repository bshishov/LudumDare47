using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class UITimerManager : MonoBehaviour
{
    public RectTransform TimersOverlay;
    public GameObject TimerPrefab;
    
    private readonly Dictionary<int, GameObject> _timerObjects = new Dictionary<int, GameObject>();

    public void SetTimer(GameObject parent, int timer)
    {
        if(TimerPrefab == null || TimersOverlay == null || parent == null)
            return;

        var textComponent = GetOrCreateUiTextTimer(parent);
        if (textComponent != null)
        {
            textComponent.text = timer == 0 ? "!" : timer.ToString();
        }
    }

    private TextMeshProUGUI GetOrCreateUiTextTimer(GameObject parent)
    {
        var key = parent.GetInstanceID();

        if (_timerObjects.ContainsKey(key))
            return _timerObjects[key].GetComponentInChildren<TextMeshProUGUI>();
        
        var uiFollowObj = Instantiate(TimerPrefab, TimersOverlay);
        var uiFollow = uiFollowObj.GetComponent<UIFollowSceneObject>();
        if(uiFollow != null)
            uiFollow.SetTarget(parent.transform);
        var textComponent = uiFollow.GetComponentInChildren<TextMeshProUGUI>();
        if(textComponent != null)
            _timerObjects.Add(key, uiFollowObj);
        return textComponent;
    }
    
    public void DeleteTimer(GameObject parent)
    {
        var key = parent.GetInstanceID();
        if (_timerObjects.ContainsKey(key))
        {
            GameObject.Destroy(_timerObjects[key]);
            _timerObjects.Remove(key);
        }
    }
}
