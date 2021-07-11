using Audio;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class UIMover : MonoBehaviour
    {
        public AnimationCurve Curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        public float TravelTime = 2f;
        public Vector2 TargetPosition;
        public bool MoveToTargetOnStart = false;

        [Header("Sound")]
        public AudioClipWithVolume MoveToTargetSound;
        public AudioClipWithVolume MoveToSourceSound;

        private Vector3 _origianlPosition;
        private Vector3 _sourcePosition;
        private Vector3 _targetPosition;

        private float _anim;
        private bool _isAnimating;

        private RectTransform _rectTransform;

        void Start ()
        {
            _rectTransform = GetComponent<RectTransform>();
            _origianlPosition = _rectTransform.anchoredPosition;
            _sourcePosition = _origianlPosition;

            if (MoveToTargetOnStart)
            {
                _sourcePosition = TargetPosition;
                _rectTransform.anchoredPosition = TargetPosition;
            }
        }
        
        void Update ()
        {
            if (_isAnimating)
            {
                _anim += Time.fixedDeltaTime / TravelTime;
                var t = Curve.Evaluate(Mathf.Clamp01(_anim));

                _rectTransform.anchoredPosition = Vector2.LerpUnclamped(_sourcePosition, _targetPosition, t);

                if (_anim > 1f)
                {
                    _isAnimating = false;
                    _anim = 0f;
                    _sourcePosition = _targetPosition;
                }
            }
        }

        [ContextMenu("To Target")]
        public void MoveToTarget()
        {
            if (_isAnimating)
                return;

            SoundManager.Instance.Play(MoveToTargetSound);
            _targetPosition = TargetPosition;
            _isAnimating = true;
            _anim = 0f;
        }

        [ContextMenu("To Source")]
        public void MoveToSource()
        {
            if(_isAnimating)
                return;

            SoundManager.Instance.Play(MoveToSourceSound);
            _targetPosition = _origianlPosition;
            _isAnimating = true;
            _anim = 0f;
        }
    }
}
