using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace UI
{
    public class UIRollbackCount : MonoBehaviour
    {
        [SerializeField]
        private ScriptbleObjectsInt Rollback;

        private Text _numberOfRollbackText;


        private void Start()
        {
            _numberOfRollbackText = GetComponent<Text>();
        }


        private void Update()
        {
            _numberOfRollbackText.text = "Rollback - "+ Rollback.Number.ToString();
        }
    }
}
