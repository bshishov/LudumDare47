using UIF.Scripts.Transitions;
using UnityEngine;

namespace UIF.Scripts
{
    [CreateAssetMenu(fileName = "FrameElement", menuName = "UIF/Frame Element", order = 1)]
    public class FrameElementData : ScriptableObject
    {
        public GameObject Prefab;
        public BaseTransition OverrideTransition;
    }
}