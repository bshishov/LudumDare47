using Attributes;
using UIF.Scripts;
using UnityEngine;

namespace UIF.Data
{
    [CreateAssetMenu(fileName = "Frame", menuName = "UIF/Frame", order = 0)]
    public class FrameData : ScriptableObject
    {
        [Reorderable]
        public FrameElementData[] Elements;
    }
}
