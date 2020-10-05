using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class UIWinLose : MonoBehaviour
{
    public GameObject WinWindow;
    public GameObject LoseWindow;
    public Text LoseText;

    public void ShowWinWindow()
    {
        WinWindow.SetActive(true);
    }
    public void ShowLoseWindow(FailReason reason)
    {
        LoseText.text = Phrases.FailReasonDict[reason];
        LoseWindow.SetActive(true);
    }
    public void HideLoseWindow()
    {        
        LoseWindow.SetActive(false);
    }

}
