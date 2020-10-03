using UnityEngine;

namespace Gameplay
{
    public class MovementAnimator : MonoBehaviour
    {
        // GLOBAL MOVEMENT ANIMATION
        static readonly float _moveTime = 0.2f;
        
        private bool _isMoving = false;
        private Vector3 _srcPosition;
        private Vector3 _tgtPosition;
        private Quaternion _srcRotation;
        private Quaternion _tgtRotation;
        private float _t;
        
        private void Update()
        {
            if (_isMoving)
            {
                _t += Time.deltaTime / _moveTime;
                if (_t > 1f)
                {
                    _t = 1f;
                    _isMoving = false;
                }
                
                transform.position = Vector3.Lerp(_srcPosition, _tgtPosition, _t);
                transform.rotation = Quaternion.Lerp(_srcRotation, _tgtRotation, _t);
            }
        }

        public void StartAnimation(Vector3 startPos, Quaternion startRot, Vector3 endPos, Quaternion endRot)
        {
            _isMoving = true;
            _t = 0;
            _srcPosition = startPos;
            _tgtPosition = endPos;
            _srcRotation = startRot;
            _tgtRotation = endRot;
        }
    }
}