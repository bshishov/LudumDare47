using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI;
using UnityEngine;
using Utils;

public class UIWinLose : MonoBehaviour
{
    public GameObject WinWindow;
    public UICanvasGroupFader LoseWindowFader;
    public TextMeshProUGUI LoseText;

    public void ShowWinWindow()
    {
        WinWindow.SetActive(true);
    }
    public void ShowLoseWindow(FailReason reason)
    {
        LoseText.text = Phrases.FailReasonDict[reason].ToUpper();
        LoseWindowFader.FadeIn();
    }
    public void HideLoseWindow()
    {   
        LoseWindowFader.FadeOut();
    }
    
    public void HideWinWindow()
    {        
        WinWindow.SetActive(false);
    }
}
