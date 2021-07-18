using System;
using Gameplay;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class UIWinLose : MonoBehaviour
{
    public GameObject WinWindow;
    public GameObject StarsContainer;
    public Text StarsCount;
    public UICanvasGroupFader LoseWindowFader;
    public TextMeshProUGUI LoseText;

    private void Start()
    {
        Common.LevelStateChanged += OnLevelStateChanged;
    }

    private void ShowWinWindow(int stars)
    {
        WinWindow.SetActive(true);
        StarsContainer.GetComponent<UIStarsContainer>().ShowCollectedStars(stars);
        StarsCount.text = PlayerStats.Instance.TotalNumberOfStars.ToString();
    }

    private void OnLevelStateChanged(Level level, Level.GameState state)
    {
        if (state == Level.GameState.PlayerDied)
        {
            ShowLoseWindow(FailReason.PlayerDied);
        }
        else if(state == Level.GameState.CatGirlDied)
        {
            ShowLoseWindow(FailReason.CatDied);
        }
        else if (state == Level.GameState.Win) 
        {
            ShowWinWindow(level.CollectedStars);
        }
        else if (state == Level.GameState.WaitingForPlayerCommand)
        {
            HideLoseWindow();
            HideWinWindow();
        }
    }

    private void ShowLoseWindow(FailReason reason)
    {
        LoseText.text = Phrases.FailReasonDict[reason].ToUpper();
        LoseWindowFader.FadeIn();
    }

    private void HideLoseWindow()
    {   
        LoseWindowFader.FadeOut();
    }

    private void HideWinWindow()
    {        
        WinWindow.SetActive(false);
    }
}
