using UnityEngine;

namespace Gameplay
{
    public class MovementAnimator : MonoBehaviour
    {
        [Header("Visuals")]
        public FxObject PushedFx;
        public FxObject MoveFx;
        public Animator Animator;
        public string AnimMoveTrigger;
        public string AnimPushedTrigger;

        // GLOBAL MOVEMENT ANIMATION
        static readonly float _moveTime = 0.2f;
        static readonly float _rotTime = 0.1f;
        
        private bool _isMoving = false;
        private bool _isRotating = false;
        private Vector3 _srcPosition;
        private Vector3 _tgtPosition;
        private Quaternion _srcRotation;
        private Quaternion _tgtRotation;
        private float _tPos;
        private float _tRot;
        private float _speed = 1f;

        private void Update()
        {
            if (_isMoving)
            {
                _tPos += _speed * Time.deltaTime / _moveTime;
                if (_tPos > 1f)
                {
                    _tPos = 1f;
                    _isMoving = false;
                    PushedFx?.Stop();
                    MoveFx?.Stop();
                }
                
                transform.position = Vector3.Lerp(_srcPosition, _tgtPosition, _tPos);
            }

            if (_isRotating)
            {
                _tRot += _speed * Time.deltaTime / _rotTime;
                if (_tRot > 1f)
                {
                    _tRot = 1f;
                    _isRotating = false;
                }

                transform.rotation = Quaternion.Lerp(_srcRotation, _tgtRotation, _tRot);
            }
        }

        public void StartAnimation(
            Vector3 startPos, 
            Quaternion startRot, 
            Vector3 endPos, 
            Quaternion endRot, 
            MovementType movementType,
            float animationSpeed = 1f)
        {
            _isMoving = true;
            _isRotating = true;
            _tPos = 0;
            _tRot = 0;
            //_srcPosition = startPos;
            //_srcRotation = startRot;
            _srcPosition = transform.position;
            _srcRotation = transform.rotation;
            _tgtPosition = endPos;
            _tgtRotation = endRot;

            _speed = animationSpeed;

            if (movementType == MovementType.Default)
            {
                if(Animator != null)
                    Animator.SetTrigger(AnimMoveTrigger);
                MoveFx?.Trigger(transform);
            }

            if (movementType == MovementType.Pushed)
            {
                if(Animator != null)
                    Animator.SetTrigger(AnimPushedTrigger);
                PushedFx?.Trigger(transform);
            }
        }
    }
}