using Assets.Scripts.Utils.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILoad : MonoBehaviour
{
    public UICanvasGroupFader _fader;

    void Start()
    {
        _fader.FadeOut();
    }

}
