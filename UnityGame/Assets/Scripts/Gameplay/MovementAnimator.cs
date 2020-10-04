using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gameplay
{
    public class MovementAnimator : MonoBehaviour
    {
        [Header("FX")] 
        public ParticleSystem SmokeParticlesPrefab;
        public ParticleSystem PushedParticlesPrefab;

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

        private ParticleSystem _smokeParticles;
        private ParticleSystem _pushedParticles;

        private void Start()
        {
            if (SmokeParticlesPrefab != null)
            {
                _smokeParticles = Instantiate(SmokeParticlesPrefab, transform);
                _smokeParticles.Stop();
            }

            if (PushedParticlesPrefab != null)
            {
                _pushedParticles = Instantiate(PushedParticlesPrefab, transform);
                _pushedParticles.Stop();
            }
        }

        private void Update()
        {
            if (_isMoving)
            {
                _tPos += _speed * Time.deltaTime / _moveTime;
                if (_tPos > 1f)
                {
                    _tPos = 1f;
                    _isMoving = false;
                    if(_pushedParticles != null)
                        _pushedParticles.Stop();
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

            if (_smokeParticles != null)
                _smokeParticles.Emit(1);
            
            if(_pushedParticles != null)
                _pushedParticles.Play();
        }
    }
}