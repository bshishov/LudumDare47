using UnityEngine;

namespace UIF.Scripts.Animations
{
    public class MoveAnimation : IAnimation
    {
        private readonly RectTransform _transform;
        private readonly AnimationCurve _easing;
        private readonly Vector2 _fromPosition;
        private readonly Vector2 _toPosition;

        public MoveAnimation(
            RectTransform transform, 
            Vector2 fromPosition, 
            Vector2 toPosition,
            AnimationCurve easing
        )
        {
            _transform = transform;
            _fromPosition = fromPosition;
            _toPosition = toPosition;
            _easing = easing;
        }
        
        public void OnStart()
        {
            _transform.anchoredPosition = _fromPosition;
        }

        public void OnUpdate(float progress)
        {
            var lerpProgress = _easing.Evaluate(progress);
            _transform.anchoredPosition = Vector2.Lerp(_fromPosition, _toPosition, lerpProgress); 
        }

        public void OnCompleted()
        {
            _transform.anchoredPosition = _toPosition;
        }
    }
}