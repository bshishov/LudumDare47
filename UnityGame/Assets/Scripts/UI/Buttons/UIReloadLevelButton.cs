using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Gameplay;

namespace UI.Buttons
{
    [RequireComponent(typeof(Button))]
    public class UIReloadLevelButton : MonoBehaviour
    {
        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(ReloadLevel);
        }

        private void ReloadLevel() 
        {
            Common.OnLevelRestart();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            
        }
    }
}
