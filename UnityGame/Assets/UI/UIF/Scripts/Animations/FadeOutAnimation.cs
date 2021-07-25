using UnityEngine;

namespace UIF.Scripts.Animations
{
    public class FadeOutAnimation : IAnimation
    {
        private readonly CanvasGroup _canvasGroup;
        private readonly AnimationCurve _easing;

        public FadeOutAnimation(CanvasGroup canvasGroup, AnimationCurve easing)
        {
            _canvasGroup = canvasGroup;
            _easing = easing;
        }

        public void OnStart()
        {
            _canvasGroup.interactable = false;
        }

        public void OnUpdate(float progress)
        {
            _canvasGroup.alpha = 1f - _easing.Evaluate(progress);
        }

        public void OnCompleted()
        {
            _canvasGroup.interactable = false;
        }
    }
}