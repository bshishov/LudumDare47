using UIF.Scripts.Animations;
using UnityEngine;

namespace UIF.Scripts.Transitions
{
    [CreateAssetMenu(fileName = "Transition", menuName = "UIF/Transitions/Scale In", order = 4)]
    public class ScaleInTransition : BaseTransition
    {
        public float Time = 0.3f;
        public AnimationCurve Easing = AnimationCurve.EaseInOut(0, 0, 1, 1);
        
        public override float GetTime()
        {
            return Time;
        }

        public override IAnimation TransitionNewSceneObjectIn(GameObject gameObject)
        {
            var rectTransform = gameObject.GetComponent<RectTransform>();
            var oldSize = rectTransform.sizeDelta;
            var fromSize = oldSize * 0.5f;
            var toSize = oldSize;
            
            return new ChangeScaleAnimation(rectTransform, fromSize, toSize, Easing);
        }

        public override IAnimation TransitionOldSceneObjectOut(GameObject gameObject)
        {
            return new FadeOutAnimation(Utils.GetOrCreateCanvas(gameObject), Easing);
        }
    }
}