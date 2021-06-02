using UnityEngine;
using UnityEngine.UI;

namespace Utils
{
    public class FPSCounter : MonoBehaviour
    {

        private Text _fpsText;
        private int _fps;

        private void Start()
        {
            _fpsText = GetComponent<Text>();
        }


        private void Update()
        {
            _fps = (int)(1f / Time.unscaledDeltaTime);
            _fpsText.text = "FPS: " + _fps.ToString();
        }
    }
}