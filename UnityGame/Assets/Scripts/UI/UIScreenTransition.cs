using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIScreenTransition : MonoBehaviour
    {
        public Image Image;
        [Range(0.01f, 3f)]
        public float TransitionTime;
        public AnimationCurve AnimationCurve = AnimationCurve.Linear(0,0,1,1);
        public UICanvasGroupFader.FaderState Initial;
        public UICanvasGroupFader.FaderState State { get; private set; }
        public bool IsShowing => State == UICanvasGroupFader.FaderState.FadedIn;

        private Color _start;
        private Color _end;
        private float _t;

        void Start()
        {
            if (Image == null)
                Image = GetComponent<Image>();

            if (State == UICanvasGroupFader.FaderState.FadedOut)
            {
                Image.color = Color.white;
            }

            if (State == UICanvasGroupFader.FaderState.FadedIn)
            {
                Image.color = Color.black;
            }
        }

        void Update()
        {
            if (State == UICanvasGroupFader.FaderState.FadingIn)
            {
                _t += Time.deltaTime / TransitionTime;
            
                if (_t > 1)
                {
                    _t = 1;
                    State = UICanvasGroupFader.FaderState.FadedIn;
                }

                Image.color = Color.LerpUnclamped(_start, _end, AnimationCurve.Evaluate(_t));
            }
        
            if (State == UICanvasGroupFader.FaderState.FadingOut)
            {
                _t += Time.deltaTime / TransitionTime;
            
                if (_t > 1)
                {
                    _t = 1;
                    State = UICanvasGroupFader.FaderState.FadedOut;
                }

                Image.color = Color.LerpUnclamped(_start, _end, 1 - AnimationCurve.Evaluate(1 - _t));
            }
        }

        public void FadeInFromWorldPosition(Vector3 position)
        {
            var screenPoint = Camera.main.WorldToScreenPoint(position);
            FadeInFromScreenPosition(screenPoint);
        }
        
        public void FadeInFromScreenPosition(Vector2 screenPoint)
        {
            FadeInFromScreenUv(
                screenPoint.x / Screen.width, 
                screenPoint.y / Screen.height);
        }

        public void FadeInFromScreenUv(float x, float y)
        {
            _start = new Color(x, y, 0);
            _end = new Color(x, y, 1f);
            _t = 0;
            State = UICanvasGroupFader.FaderState.FadingIn;
        }

        public void FadeInFromScreenCenter()
        {
            FadeInFromScreenUv(0.5f, 0.5f);
        }

        public void FadeOutFromWorldPosition(Vector3 position)
        {
            var screenPoint = Camera.main.WorldToScreenPoint(position);
            FadeOutFromScreenPosition(screenPoint);
        }
        
        public void FadeOutFromScreenPosition(Vector2 screenPoint)
        {
            var x = screenPoint.x / Screen.width;
            var y = screenPoint.y / Screen.height;
            FadeOutFromScreenUv(x, y);
        }

        public void FadeOutFromScreenUv(float x, float y)
        {
            _start = new Color(x, y, 1f);
            _end = new Color(x, y, 0f);
            _t = 0;
            State = UICanvasGroupFader.FaderState.FadingOut;
        }
        
        public void FadeOutFromScreenCenter()
        {
            FadeInFromScreenUv(0.5f, 0.5f);
        }
    }
}
