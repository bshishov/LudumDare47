using Gameplay;
using UI;
using UnityEngine;

public class UISceneSelector : MonoBehaviour
{
    public GameObject ParentObject;
    public GameObject LevelPrefab;
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
                panel.GetComponent<UILevelOnMap>().SetSceneSettings(activeLevelNumber, levelInfo.SceneName);
                activeLevelNumber += 1;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
