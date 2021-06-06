using UnityEngine;
using UnityEngine.UI;
using Gameplay;

namespace UI
{
    public class UIRollbackCount : MonoBehaviour
    {
        private Text _numberOfRollbackText;

        private void Start()
        {
            _numberOfRollbackText = GetComponent<Text>();
        }


        private void Update()
        {
            _numberOfRollbackText.text = "Rollback - "+ GameSettings.NumberOfRollback.ToString();
        }
    }
}
