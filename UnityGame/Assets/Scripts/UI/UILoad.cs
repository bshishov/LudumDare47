using System.Collections;
using Gameplay;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UILoad : MonoBehaviour
{
    public LevelSet Levels;
    public UIScreenTransition ScreenTransition;
    public float FadeInAfterDelay = 0.5f;
    
    private string _nextLevel;
    private GameObject _playerObject;

    IEnumerator Start()
    {
        Time.timeScale = 1f;
        yield return new WaitForSeconds(FadeInAfterDelay);

        if (ScreenTransition != null)
        {
            _playerObject = GameObject.FindWithTag("Player");
            if (_playerObject != null)
            {
                ScreenTransition.FadeInFromWorldPosition(_playerObject.transform.position);
            }
            else
            {
                ScreenTransition.FadeInFromScreenCenter();
            }
        }
    }

    private void LoadLevel(string levelString)
    {
        if (ScreenTransition != null)
        {
            if(_playerObject != null)
                ScreenTransition.FadeOutFromWorldPosition(_playerObject.transform.position);
            else
                ScreenTransition.FadeInFromScreenCenter();
            ScreenTransition.StateChanged += (state) =>
            {
                if (state == UICanvasGroupFader.FaderState.FadedOut)
                {
                    Debug.Log($"[UILoad] Loading scene: {levelString}");
                    SceneManager.LoadScene(levelString);
                }
            };
        }
        else
        {
            SceneManager.LoadScene(levelString);
        }
    }

    public void LoadMenu()
    {
        LoadLevel(Levels.MenuScene);
    }

    public void LoadNext()
    {
        LoadLevel(Levels.GetNextLevel());
    }

    public void LoadWinWindow()
    {
        LoadLevel(Levels.GetNextLevel());
    }
}
