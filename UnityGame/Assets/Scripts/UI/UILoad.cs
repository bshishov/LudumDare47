using Gameplay;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UILoad : MonoBehaviour
{
    public string MainMenu;
    public UICanvasGroupFader _fader;
    public LevelSet Levels;
    private string _nextLevel;

    void Start()
    {
        _fader.FadeOut();
        Time.timeScale = 1f;
    }

    private void LoadLevel(string levelString)
    {
        _fader.FadeIn();
        _fader.StateChanged += () =>
        {
            if (_fader.State == UICanvasGroupFader.FaderState.FadedIn)
            {
                SceneManager.LoadScene(levelString);
                Time.timeScale = 1f;
            }
        };
        
    }

    public void LoadMenu()
    {
        LoadLevel(Levels.MenuScene);
    }

    public void LoadNext()
    {
        LoadLevel(Levels.GetNextLevel());
    }
}
