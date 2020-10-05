using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowTurns : MonoBehaviour
{
    public GameObject ParentalPanel;
    public GameObject PanelPrefab;
    public Sprite ActiveImage;
    public Sprite PassiveImage;

    private int _numberOfTurns;
    private int _currentTurn;
    private GameObject[] _turnPanels;
    //private GameObject _canvas;


    void Start()
    {
       // _canvas = GameObject.Find("Canvas");
        _currentTurn = 0;
    }

    // TODO: for test purpose only, delete in the project!
    public void Initialize(int numberOfTurns)
    {
        SetTurnsNumber(numberOfTurns);
    }

    public void SetTurnsNumber(int numberOfTurns)
    {
        _numberOfTurns = numberOfTurns + 1;
        float width = ParentalPanel.GetComponent<RectTransform>().rect.width;
        float height = ParentalPanel.GetComponent<RectTransform>().rect.height;
        _turnPanels = new GameObject[_numberOfTurns];
        if (width / _numberOfTurns > height)        
            PanelPrefab.GetComponent<AspectRatioFitter>().aspectMode = AspectRatioFitter.AspectMode.HeightControlsWidth;
        else
            PanelPrefab.GetComponent<AspectRatioFitter>().aspectMode = AspectRatioFitter.AspectMode.WidthControlsHeight;

        for (int i = 0; i < _numberOfTurns; i++)
        {
            var panel = Instantiate(PanelPrefab);
            panel.transform.SetParent(ParentalPanel.transform, false);
            _turnPanels[i] = panel;
        }
        _turnPanels[0].GetComponent<Image>().sprite = ActiveImage;
    }
    public void NextTurn()
    {
        _currentTurn++;
        if (_currentTurn < _numberOfTurns)         
            _turnPanels[_currentTurn].GetComponent<Image>().sprite = ActiveImage;
    }

    public void BackTurn()
    {
        if (_currentTurn > 0)
        {
            if (_currentTurn < _numberOfTurns)
                _turnPanels[_currentTurn].GetComponent<Image>().sprite = PassiveImage;
            _currentTurn--;
        }
    }
    public void SetTurn(int turn)
    {
        if ((turn >= 0) && (turn < _numberOfTurns))
        {
            for (int i = 0; i <= turn; i++)
                _turnPanels[i].GetComponent<Image>().GetComponent<Image>().sprite = ActiveImage;
            for (int i = turn+1; i <= Mathf.Min(_currentTurn, _numberOfTurns); i++)
                _turnPanels[i].GetComponent<Image>().GetComponent<Image>().sprite = PassiveImage;
            _currentTurn = turn;
        }
    }

}
