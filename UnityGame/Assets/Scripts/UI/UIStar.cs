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
        public void Award(float pitch)
        {
            Animator.SetBool(IsAwarded, true);
            var sound = SoundManager.Instance.Play(AwardSound);
            sound.Pitch = pitch;
        }

        [ContextMenu("Reset")]
        public void Reset()
        {
            Animator.SetBool(IsAwarded, false);
        }
    }
}