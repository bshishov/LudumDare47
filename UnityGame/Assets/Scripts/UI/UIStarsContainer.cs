using UnityEngine;

namespace UI
{
    public class UIStarsContainer : MonoBehaviour
    {
        [SerializeField] private GameObject[] StarImages;
        
        public void ShowCollectedStars(int stars)
        {
            for (int i = 0; i < stars; i++)
            {
                StarImages[i].SetActive(true);
            }
        }
    }
}
