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

        var activeLevelNumber = 1;
        foreach (var levelInfo in Levels.Levels)
        {
            if (levelInfo.Enabled)
            {
                var panel = Instantiate(LevelPrefab, ParentObject.transform);
                panel.GetComponent<UIChooseLevel>().SetSceneSettings(activeLevelNumber, levelInfo.SceneName, Fader);
                activeLevelNumber += 1;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
