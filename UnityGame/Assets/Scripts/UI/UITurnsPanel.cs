using Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UITurnsPanel : MonoBehaviour
    {
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

        private void Start()
        {
            // Subscribe to level events
            Common.LevelChanged += Initialize;
            Common.LevelTurnCompleted += TurnCompleted;
            Common.LevelTurnRolledBack += TurnRolledBack;

            if (Common.CurrentLevel != null)
            {
                Initialize(Common.CurrentLevel);
            }
        }

        private void OnDestroy()
        {
            // Unsubscribe
            Common.LevelChanged -= Initialize;
            Common.LevelTurnCompleted -= TurnCompleted;
            Common.LevelTurnRolledBack -= TurnRolledBack;
        }

        private void Initialize(Level level)
        {
            _currentTurn = level.CurrentTurnNumber;
            _numberOfTurns = level.MaxTurns;
            _turnPanels = new Image[_numberOfTurns];

            for (var i = 0; i < _numberOfTurns; i++)
                _turnPanels[i] = Instantiate(PanelPrefab, transform).GetComponent<Image>();

            SetIconAndColor(0, CurrentTurnIcon, CurrentTurnColor);
        }

        private void TurnCompleted(Level level)
        {
            SetIconAndColor(_currentTurn, CompletedTurnIcon, CompletedTurnColor);
            _currentTurn++;
            SetIconAndColor(_currentTurn, CurrentTurnIcon, CurrentTurnColor);
        }

        private void TurnRolledBack(Level level)
        {
            if (_currentTurn > 0)
            {
                SetIconAndColor(_currentTurn, InactiveTurnIcon, InactiveTurnColor);
                _currentTurn--;
                SetIconAndColor(_currentTurn, CurrentTurnIcon, CurrentTurnColor);
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
}