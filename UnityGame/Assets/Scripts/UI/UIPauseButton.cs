using System;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(Button))]
    public class UIPauseButton : MonoBehaviour
    {
        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(OnButtonPressed);
        }

        private void OnButtonPressed()
        {
            Common.TogglePause();
        }
    }
}