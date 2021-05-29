using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Gameplay;

namespace TouchControll
{
    public class SwipeDetector : MonoBehaviour
    {

        [SerializeField]
        private float _minDistanceForSwipe = 30f;

        private Vector2 _fingerDownPosition;
        private Vector2 _fingerUpPosition;



        private void Update()
        {
            DetectSwipe();
            
        }

        private void DetectSwipe()
        {
            foreach (Touch touch in Input.touches)
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

            if (CheckSwipeDistance())
            {

                if (VecticalSwipeDistance() > HorizontalSwipeDistance())
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

                if (HorizontalSwipeDistance() > VecticalSwipeDistance())
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

            if ((VecticalSwipeDistance() > _minDistanceForSwipe) || (HorizontalSwipeDistance() > _minDistanceForSwipe))
            {
                return true;
            } else
            {
                return false;
            }

        }

        private float VecticalSwipeDistance()
        {
            return Mathf.Abs(_fingerDownPosition.x - _fingerUpPosition.x);
        }
        private float HorizontalSwipeDistance()
        {
            return Mathf.Abs(_fingerDownPosition.y - _fingerUpPosition.y);
        }
    }
}