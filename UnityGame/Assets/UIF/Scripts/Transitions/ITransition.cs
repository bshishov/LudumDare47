using UIF.Scripts.Animations;
using UnityEngine;

namespace UIF.Scripts.Transitions
{
    public interface ITransition
    {
        float GetTime();
        string GetName();

        IAnimation TransitionNewSceneObjectIn(GameObject gameObject);
        IAnimation TransitionOldSceneObjectOut(GameObject gameObject);
    }
}