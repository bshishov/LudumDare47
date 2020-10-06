using Assets.Scripts.UI;
using Assets.Scripts.Utils.UI;
using System.Collections;
using System.Collections.Generic;
using Gameplay;
using UnityEngine;

public class UISceneSelector : MonoBehaviour
{
    public GameObject ParentObject;
    public GameObject LevelPrefab;
    public GameObject Fader;
    public LevelSet Levels;
    
    void Start()
    {
        if(Levels == null)
            return;
        for (var i = 0; i < Levels.Levels.Length; i++)
        {
            var levelInfo = Levels.Levels[i];
            if (levelInfo.Enabled)
            {
                var panel = Instantiate(LevelPrefab);
                panel.transform.SetParent(ParentObject.transform, false);
                panel.GetComponent<UIChooseLevel>().SetSceneSettings(i + 1, levelInfo.SceneName, Fader);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
