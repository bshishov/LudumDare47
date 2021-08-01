using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UITimer : MonoBehaviour
    {
        [Header("References")]
        public TextMeshProUGUI Text;
        public Image Background;

        [Header("Visuals")] 
        public Color ThisTurnBgrColor = Color.white;
        public Color ThisTurnTextColor = Color.white;
        public Color ManyTurnsBgrColor = Color.white;
        public Color ManyTurnsTextColor = Color.white;

        public void SetTurns(int? turns)
        {
            var showTurns = turns.HasValue;
            Text.enabled = showTurns;
            Background.enabled = showTurns;
            
            if (showTurns)
            {
                var text = turns == 0 ? "!" : turns.ToString();

                if (Text != null)
                {
                    Text.text = text;
                    Text.color = turns > 0 ? ManyTurnsTextColor : ThisTurnTextColor;
                }

                if (Background != null)
                {
                    Background.color = turns > 0 ? ManyTurnsBgrColor : ThisTurnBgrColor;
                }
            }
        }
    }
}
