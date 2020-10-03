using UnityEngine;

namespace Utils
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        protected static T _instance;

        // Returns the instance of this singleton
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = (T) FindObjectOfType(typeof(T));

                    if (_instance == null)
                        Debug.LogWarning("An instance of " + typeof(T) +
                                         " is needed in the scene, but there is none.");
                }

                return _instance;
            }
        }
    }
}