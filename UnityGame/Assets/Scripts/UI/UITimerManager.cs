using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITimerManager : MonoBehaviour
{
    public GameObject WorldCanvas;
    private Dictionary<int, GameObject> _objectToCanvas = new Dictionary<int, GameObject>();

    // Start is called before the first frame update
    void Start()
    {
       
    }

    public void SetTimer(GameObject parent, int timer)
    {
        var panel = (GameObject)Instantiate(WorldCanvas);
        panel.transform.SetParent(parent.transform, false);
        panel.GetComponentInChildren<Text>().text = timer.ToString();
        _objectToCanvas[parent.GetInstanceID()] = panel;
    }
    public void DeleteTimer(GameObject parent)
    {        
        GameObject.Destroy(_objectToCanvas[parent.GetInstanceID()]);
    }
}
