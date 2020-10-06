using UnityEngine;

namespace Utils
{
    public class UIFollowSceneObject : MonoBehaviour
    {
        private Camera _main;
        public Vector3 ScreenOffset;
        public Transform Target;
        public Vector3 WorldOffset;

        private void Start()
        {
            _main = Camera.main;
        }

        private void Update()
        {
            if (Target != null && _main != null)
            {
                var screenPoint = _main.WorldToScreenPoint(Target.position + WorldOffset);
                transform.position = new Vector3(screenPoint.x, screenPoint.y, transform.position.z) + ScreenOffset;
            }
        }

        public void SetTarget(Transform target)
        {
            Target = target;
            Update();
        }
    }
}