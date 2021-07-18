using Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Buttons
{
    [RequireComponent(typeof(Button))]
    public class UIRollForwardButton : MonoBehaviour
    {
        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(RollForward);
        }

        private void RollForward()
        {
            if (Common.CurrentLevel != null)
                Common.CurrentLevel.SkipLevel();
        }
    }
}
