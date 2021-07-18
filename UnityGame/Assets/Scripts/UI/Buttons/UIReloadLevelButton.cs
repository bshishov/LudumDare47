using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
