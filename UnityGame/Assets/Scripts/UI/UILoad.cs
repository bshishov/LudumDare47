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
    
    private void LoadLevel(string levelString)
    {
       SceneManager.LoadScene(levelString);
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
