using UnityEngine;

namespace Utils
{
    public class ColorOverLifeTime : MonoBehaviour
    {
        public Gradient ColorGradient;
        public Renderer MainRenderer;
        public float LifeTime = 1f;

        private float _t = 0;

        void Start()
        {
            if (MainRenderer == null)
                MainRenderer = GetComponent<Renderer>();
        }
    
        void Update()
        {
            _t += Time.deltaTime;
            MainRenderer.material.color = ColorGradient.Evaluate(_t / LifeTime);
        }
    }
}
