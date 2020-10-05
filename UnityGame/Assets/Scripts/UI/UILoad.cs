using Assets.Scripts.Utils.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UILoad : MonoBehaviour
{
    public string MainMenu;
    public UICanvasGroupFader _fader;

    void Start()
    {
        _fader.FadeOut();
    }

    public void LoadMenu()
    {
        _fader.FadeIn();
        _fader.StateChanged += () =>
        {
            if (_fader.State == UICanvasGroupFader.FaderState.FadedIn)
            {
                SceneManager.LoadScene(MainMenu); 
            }
        };
        
    }

}
