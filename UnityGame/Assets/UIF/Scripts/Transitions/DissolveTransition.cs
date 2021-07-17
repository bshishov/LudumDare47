using UIF.Scripts.Animations;
using UnityEngine;

namespace UIF.Scripts.Transitions
{
    [CreateAssetMenu(fileName = "Transition", menuName = "UIF/Transitions/Dissolve", order = 1)]
    public class DissolveTransition : BaseTransition
    {
        public float Time = 0.3f;
        public AnimationCurve Easing = AnimationCurve.Linear(0, 0, 1, 1);

        public override float GetTime()
        {
            return Time;
        }

        public override IAnimation TransitionNewSceneObjectIn(GameObject gameObject)
        {
            var canvasGroup = Utils.GetOrCreateCanvas(gameObject);
            return new FadeInAnimation(canvasGroup, Easing);
        }

        public override IAnimation TransitionOldSceneObjectOut(GameObject gameObject)
        {
            var canvasGroup = Utils.GetOrCreateCanvas(gameObject);
            return new FadeOutAnimation(canvasGroup, Easing);
        }
    }
}