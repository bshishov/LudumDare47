using Assets.Scripts.Utils.UI;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UILoad : MonoBehaviour
{
    public string MainMenu;
    public UICanvasGroupFader _fader;
    private string _nextLevel;

    void Start()
    {
        _fader.FadeOut();
    }

    private void LoadLevel(string LevelString)
    {
        _fader.FadeIn();
        _fader.StateChanged += () =>
        {
            if (_fader.State == UICanvasGroupFader.FaderState.FadedIn)
            {
                SceneManager.LoadScene(LevelString); 
            }
        };
        
    }
    public void SetNextLevel(string nextLevel)
    {
        _nextLevel = nextLevel;
    }

    public void LoadMenu()
    {
        LoadLevel(MainMenu);
    }

    public void LoadNext()
    {
        LoadLevel(_nextLevel);
    }
}
