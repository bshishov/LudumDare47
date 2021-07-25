using UIF.Scripts.Animations;
using UnityEngine;

namespace UIF.Scripts.Transitions
{
    public abstract class BaseTransition : ScriptableObject, ITransition
    {
        public abstract float GetTime();

        public string GetName()
        {
            return this.name;
        }

        public abstract IAnimation TransitionNewSceneObjectIn(GameObject gameObject);

        public abstract IAnimation TransitionOldSceneObjectOut(GameObject gameObject);
    }
}