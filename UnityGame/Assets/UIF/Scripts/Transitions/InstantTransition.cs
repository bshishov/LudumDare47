using UIF.Scripts.Animations;
using UnityEngine;

namespace UIF.Scripts.Transitions
{
    [CreateAssetMenu(fileName = "Transition", menuName = "UIF/Transitions/Instant", order = 0)]
    public class InstantTransition: BaseTransition
    {
        public override float GetTime()
        {
            return 0;
        }

        public override IAnimation TransitionNewSceneObjectIn(GameObject gameObject)
        {
            return InstantAnimation.Instance;
        }

        public override IAnimation TransitionOldSceneObjectOut(GameObject gameObject)
        {
            return InstantAnimation.Instance;
        }
    }
}