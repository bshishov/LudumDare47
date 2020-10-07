using UnityEngine;
using UnityEngine.UI;

public class ShowTurns : MonoBehaviour
{
    public GameObject ParentalPanel;
    public GameObject PanelPrefab;
    
    [Header("Current turn")]
    public Sprite CurrentTurnIcon;
    public Color CurrentTurnColor = Color.white;
    
    [Header("Inactive turn")]
    public Sprite InactiveTurnIcon;
    public Color InactiveTurnColor = Color.white;

    [Header("Completed turn")]
    public Sprite CompletedTurnIcon;
    public Color CompletedTurnColor = Color.white;

    private int _numberOfTurns;
    private int _currentTurn;
    private Image[] _turnPanels;

    void Start()
    {
        _currentTurn = 0;
    }

    public void Initialize(int numberOfTurns)
    {
        _numberOfTurns = numberOfTurns;
        _turnPanels = new Image[_numberOfTurns];

        for (var i = 0; i < _numberOfTurns; i++)
            _turnPanels[i] = Instantiate(PanelPrefab, ParentalPanel.transform).GetComponent<Image>();

        SetIconAndColor(0, CurrentTurnIcon, CurrentTurnColor);
    }
    
    [ContextMenu("Next Turn")]
    public void TurnCompleted()
    {
        SetIconAndColor(_currentTurn, CompletedTurnIcon, CompletedTurnColor);
        _currentTurn++;
        SetIconAndColor(_currentTurn, CurrentTurnIcon, CurrentTurnColor);
    }

    [ContextMenu("Back Turn")]
    public void BackTurn()
    {
        if (_currentTurn > 0)
        {
            SetIconAndColor(_currentTurn, InactiveTurnIcon, InactiveTurnColor);
            _currentTurn--;
            SetIconAndColor(_currentTurn, CurrentTurnIcon, CurrentTurnColor);
        }
    }
    public void SetTurn(int turn)
    {
        if ((turn >= 0) && (turn < _numberOfTurns))
        {
            _currentTurn = turn;
            
            for (var i = 0; i < _currentTurn; i++)
                SetIconAndColor(i, CompletedTurnIcon, CompletedTurnColor);
            
            SetIconAndColor(_currentTurn, CurrentTurnIcon, CurrentTurnColor);

            for (var i = _currentTurn + 1; i < _numberOfTurns; i++)
                SetIconAndColor(i, InactiveTurnIcon, InactiveTurnColor);
        }
    }

    private void SetIconAndColor(int index, Sprite icon, Color color)
    {
        if (index >= 0 && index < _turnPanels.Length)
        {
            var panelImage = _turnPanels[index];
            panelImage.sprite = icon;
            panelImage.color = color;
        }
    }
}
