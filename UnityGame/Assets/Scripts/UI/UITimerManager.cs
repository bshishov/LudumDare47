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
    
    private readonly Dictionary<int, TextMeshProUGUI> _objectToCanvas = new Dictionary<int, TextMeshProUGUI>();

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

        if (_objectToCanvas.ContainsKey(key))
            return _objectToCanvas[key];
        
        var uiFollowObj = Instantiate(TimerPrefab, TimersOverlay);
        var uiFollow = uiFollowObj.GetComponent<UIFollowSceneObject>();
        if(uiFollow != null)
            uiFollow.SetTarget(parent.transform);
        var textComponent = uiFollow.GetComponent<TextMeshProUGUI>();
        if(textComponent != null)
            _objectToCanvas.Add(key, textComponent);
        return textComponent;
    }
    
    public void DeleteTimer(GameObject parent)
    {
        var key = parent.GetInstanceID();
        if (_objectToCanvas.ContainsKey(key))
        {
            GameObject.Destroy(_objectToCanvas[key]);
            _objectToCanvas.Remove(key);
        }
    }
}
