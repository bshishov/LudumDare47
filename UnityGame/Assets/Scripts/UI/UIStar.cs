using Audio;
using UnityEngine;

namespace UI
{
    public class UIStar : MonoBehaviour
    {
        public Animator Animator;
        public SoundAsset AwardSound;
        
        private static readonly int IsAwarded = Animator.StringToHash("IsAwarded");

        [ContextMenu("Award")]
        public void Award()
        {
            Animator.SetBool(IsAwarded, true);
            SoundManager.Instance.Play(AwardSound);
        }

        [ContextMenu("Reset")]
        public void Reset()
        {
            Animator.SetBool(IsAwarded, false);
        }
    }
}