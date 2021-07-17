using UIF.Scripts.Animations;
using UnityEngine;

namespace UIF.Scripts.Transitions
{
    [CreateAssetMenu(fileName = "Transition", menuName = "UIF/Transitions/Move", order = 2)]
    public class MoveTransition : BaseTransition
    {
        public float Time = 0.3f;
        public Direction Direction = Direction.Left;
        public AnimationCurve Easing = AnimationCurve.EaseInOut(0, 0, 1, 1);
        
        public override float GetTime()
        {
            return Time;
        }

        public override IAnimation TransitionNewSceneObjectIn(GameObject gameObject)
        {
            var rectTransform = gameObject.GetComponent<RectTransform>();
            var oldPos = rectTransform.anchoredPosition;
            var fromPosition = oldPos + Utils.DirectionToScreenDelta(Direction);
            var toPosition = oldPos;
            
            return new MoveAnimation(rectTransform, fromPosition, toPosition, Easing);
        }

        public override IAnimation TransitionOldSceneObjectOut(GameObject gameObject)
        {
            return new FadeOutAnimation(Utils.GetOrCreateCanvas(gameObject), Easing);
        }
    }
}