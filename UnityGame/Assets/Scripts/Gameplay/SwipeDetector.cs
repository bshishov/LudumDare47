using UnityEngine;

namespace Gameplay
{
    public class SwipeDetector : MonoBehaviour
    {

        [SerializeField]
        private float _minDistanceForSwipe = 30f;

        private Vector2 _fingerDownPosition;
        private Vector2 _fingerUpPosition;

        private Vector2 _swipeDistanceDelta;

        private void Update()
        {
            DetectSwipe();
        }

        private void DetectSwipe()
        {
            foreach (var touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    _fingerDownPosition = touch.position;
                }

                if (touch.phase == TouchPhase.Ended)
                {
                    _fingerUpPosition = touch.position;

                    CalculateSwipeDirection();
                }
            }
        }

        private void CalculateSwipeDirection()
        {
            CalculateSwipeDelta();

            if (CheckSwipeDistance())
            {
                
                if (_swipeDistanceDelta.x > _swipeDistanceDelta.y)
                {
                    if (_fingerDownPosition.x > _fingerUpPosition.x)
                    {
                        Level.Instance.PlayerMove(Direction.Left);
                    }

                    if (_fingerDownPosition.x < _fingerUpPosition.x)
                    {
                        Level.Instance.PlayerMove(Direction.Right);
                    }
                }

                if (_swipeDistanceDelta.y > _swipeDistanceDelta.x)
                {
                    if (_fingerDownPosition.y > _fingerUpPosition.y)
                    {
                        Level.Instance.PlayerMove(Direction.Back);
                    }

                    if (_fingerDownPosition.y < _fingerUpPosition.y)
                    {
                        Level.Instance.PlayerMove(Direction.Front);
                    }
                }

            }

        }

        private bool CheckSwipeDistance()
        {
            return (_swipeDistanceDelta.x > _minDistanceForSwipe) || (_swipeDistanceDelta.y > _minDistanceForSwipe);
        }

        private void CalculateSwipeDelta()
        {
            _swipeDistanceDelta = new Vector2(Mathf.Abs(_fingerDownPosition.x - _fingerUpPosition.x),
                                              Mathf.Abs(_fingerDownPosition.y - _fingerUpPosition.y));
        }
    }
}