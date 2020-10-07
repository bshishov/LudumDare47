using System.Collections;
using Gameplay;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UILoad : MonoBehaviour
{
    public string MainMenu;
    public UICanvasGroupFader _fader;
    public LevelSet Levels;
    public UIScreenTransition ScreenTransition;
    private string _nextLevel;
    private GameObject _playerObject;

    IEnumerator Start()
    {
        Time.timeScale = 1f;
        yield return new WaitForSeconds(1);
        
        _fader.FadeOut();

        if (ScreenTransition != null)
        {
            _playerObject = GameObject.FindWithTag("Player");
            if (_playerObject != null)
            {
                ScreenTransition.FadeInFromWorldPosition(_playerObject.transform.position);
            }
            else
            {
                ScreenTransition.FadeInFromScreenUv(0.5f, 0.5f);
            }
        }
    }

    private void LoadLevel(string levelString)
    {
        _fader.FadeIn();
        if (ScreenTransition != null)
        {
            if(_playerObject != null)
                ScreenTransition.FadeOutFromWorldPosition(_playerObject.transform.position);
            else
                ScreenTransition.FadeInFromScreenCenter();
        }

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
