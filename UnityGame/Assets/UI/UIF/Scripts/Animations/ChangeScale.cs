using UnityEngine;

namespace UIF.Scripts.Animations
{
    public class ChangeScaleAnimation : IAnimation
    {
        private readonly RectTransform _transform;
        private readonly AnimationCurve _easing;
        private readonly Vector2 _fromSize;
        private readonly Vector2 _toSize;

        public ChangeScaleAnimation(
            RectTransform transform, 
            Vector2 fromSize, 
            Vector2 toSize,
            AnimationCurve easing
        )
        {
            _transform = transform;
            _fromSize = fromSize;
            _toSize = toSize;
            _easing = easing;
        }
        
        public void OnStart()
        {
            _transform.sizeDelta = _fromSize;
        }

        public void OnUpdate(float progress)
        {
            var lerpProgress = _easing.Evaluate(progress);
            _transform.sizeDelta = Vector2.Lerp(_fromSize, _toSize, lerpProgress); 
        }

        public void OnCompleted()
        {
            _transform.sizeDelta = _toSize;
        }
    }
}