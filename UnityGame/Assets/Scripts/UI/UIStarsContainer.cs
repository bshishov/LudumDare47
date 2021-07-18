using System.Collections;
using Gameplay;
using UnityEngine;

namespace UI
{
    public class UIStarsContainer : MonoBehaviour
    {
        public UIStar Star1;
        public UIStar Star2;
        public UIStar Star3;
        public float AwardDelay = 0.5f;
        public float PitchMod = 0.05f;

        private void Start()
        {
            if(Common.CurrentLevel != null)
                AwardStars(Common.CurrentLevel.CollectedStars);
        }

        public void AwardStars(int stars)
        {
            StartCoroutine(AwardRoutine(stars));
        }

        private IEnumerator AwardRoutine(int stars)
        {
            if(stars >= 1)
                Star1.Award(1f);

            if (stars >= 2)
            {
                yield return new WaitForSeconds(AwardDelay);
                Star2.Award(1f + PitchMod);
            }

            if (stars >= 3)
            {
                yield return new WaitForSeconds(AwardDelay);
                Star3.Award(1f + PitchMod * 2);
            }
        }
    }
}
