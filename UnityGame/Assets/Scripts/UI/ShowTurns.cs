using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowTurns : MonoBehaviour
{
    public GameObject ParentalPanel;
    public GameObject PanelPrefab;

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
        _numberOfTurns = numberOfTurns;
        float width = ParentalPanel.GetComponent<RectTransform>().rect.width;
        float squareWidth = Mathf.Min(width / _numberOfTurns, ParentalPanel.GetComponent<RectTransform>().rect.height);
        float initX = ParentalPanel.GetComponent<RectTransform>().rect.x;
        float initY = ParentalPanel.GetComponent<RectTransform>().rect.y;
        _turnPanels = new GameObject[_numberOfTurns];
        for (int i = 0; i < _numberOfTurns; i++)
        {
            var panel = Instantiate(PanelPrefab);
            panel.transform.SetParent(ParentalPanel.transform, false);
            //panel.GetComponent<RectTransform>().anchoredPosition = new Vector3(initX, initY, 0);
            //panel.GetComponent<RectTransform>().localScale = new Vector3(squareWidth, squareWidth,1); 
            _turnPanels[i] = panel;
            //initX += squareWidth;
        }
        _turnPanels[0].GetComponent<Image>().color = Color.green;
    }
    public void NextTurn()
    {
        _currentTurn++;
        _turnPanels[_currentTurn].GetComponent<Image>().color = Color.green;
        _turnPanels[_currentTurn-1].GetComponent<Image>().color = Color.red;
    }
}
