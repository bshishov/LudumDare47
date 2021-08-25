using UnityEngine;
using UnityEngine.UI;
using Gameplay;
using TMPro;

namespace UI
{
    public class UIRollbackCount : MonoBehaviour
    {
        public TextMeshProUGUI Counter;
        
        private void Update()
        {
            Counter.text = PlayerStats.Instance.NumberOfRollback.ToString() ;
        }
    }
}
