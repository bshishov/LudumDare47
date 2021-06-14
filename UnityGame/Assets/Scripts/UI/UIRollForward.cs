using Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIRollForward : MonoBehaviour
    {

        private Button _rollForwardButton;
        private void Start()
        {
            _rollForwardButton = GetComponent<Button>();
            _rollForwardButton.onClick.AddListener(RollForward);
        }

        private void RollForward()
        {
            Level.Instance.SkipLevel();
        }

    }

}
