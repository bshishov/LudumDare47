using Assets.Scripts.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISceneSelector : MonoBehaviour
{
    public GameObject ParentObject;
    public GameObject LevelPrefab;
    public Object[] scenes;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < scenes.Length; i++)
        {
            var panel = Instantiate(LevelPrefab);
            panel.transform.SetParent(ParentObject.transform, false);
            panel.GetComponent<UIChooseLevel>().SetSceneSettings(i+1, scenes[i].name);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
