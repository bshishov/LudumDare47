using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace UI
{
    [RequireComponent(typeof(ScrollRect))]
    public class UILevelInCenterOfMap : MonoBehaviour
    {
        public enum TargetLevelState
        {
            above,
            below,
            inCenter
        }
        public RectTransform MaskTransform;
        public GameObject LevelsOnMap;

        [SerializeField] private float _waitForMoveScrollTime;

        private UIPlayerPointer _uIPlayerPointer;

        private UILevelOnMap[] _levels;

        private RectTransform _targetLevel;
        private ScrollRect _scrollRect;
        private RectTransform _scrollTransform;
        private RectTransform _content;
        private Vector2 _scrollTargetPosition;

        private float _contentTopYPosition;
        private float _contentBottomYPosition;

        private TargetLevelState _targetState = TargetLevelState.inCenter;

        private void Start()
        {
            _scrollRect = GetComponent<ScrollRect>();
            _scrollTransform = _scrollRect.transform as RectTransform;
            _content = _scrollRect.content;

            _levels = GetComponentsInChildren<UILevelOnMap>();

            if (GamePersist.Instance.LastLevel != null)
            {
                FindUILevelObject(GamePersist.Instance.LastLevel);
            }
            else
            {
                FindUILevelObject(_levels[0].LevelName);
            }

            _uIPlayerPointer = FindObjectOfType<UIPlayerPointer>();
        }

        private void FindUILevelObject(string lastLevel)
        {

            for (int i = 0; i < _levels.Length; i++)
            {
                if (_levels[i].LevelName == lastLevel)
                {
                    if (i + 1 < _levels.Length)
                    {
                        _targetLevel = _levels[i + 1].gameObject.GetComponent<RectTransform>();
                    }
                    else
                    {
                        _targetLevel = _levels[i].gameObject.GetComponent<RectTransform>();
                    }
                }
            }

            CenterOnItem();
        }

        public void SetLevelIncenter()
        {
            StartCoroutine(MoveScrollRecrt(_scrollTargetPosition));
        }

        private void CenterOnItem()
        {
            var itemCenterPositionInScroll = GetWorldPointInWidget(_scrollTransform, GetWidgetWorldPoint(_targetLevel));
            var targetPositionInScroll = GetWorldPointInWidget(_scrollTransform, GetWidgetWorldPoint(MaskTransform));

            var difference = targetPositionInScroll - itemCenterPositionInScroll;
            difference.z = 0f;
            difference.x = 0f;

            var normalizedDifference = new Vector2(
                difference.x / (_content.rect.size.x - _scrollTransform.rect.size.x),
                difference.y / (_content.rect.size.y - _scrollTransform.rect.size.y));

            var newNormalizedPosition = _scrollRect.normalizedPosition - normalizedDifference;
            if (_scrollRect.movementType != ScrollRect.MovementType.Unrestricted)
            {
                newNormalizedPosition.x = Mathf.Clamp01(newNormalizedPosition.x);
                newNormalizedPosition.y = Mathf.Clamp01(newNormalizedPosition.y);
            }
            _scrollTargetPosition = newNormalizedPosition;
            _scrollRect.normalizedPosition = _scrollTargetPosition;

            _targetState = TargetLevelState.inCenter;
            _contentTopYPosition = _content.anchoredPosition.y + _scrollTransform.rect.height / 2;
            _contentBottomYPosition = _content.anchoredPosition.y - _scrollTransform.rect.height / 2;
        }

        IEnumerator MoveScrollRecrt(Vector2 targetPosition)
        {
            var currentPos = _scrollRect.normalizedPosition;
            var waitTime = _waitForMoveScrollTime;
            var elapsedTime = 0f;

            while (elapsedTime < waitTime)
            {
                _scrollRect.normalizedPosition = Vector2.Lerp(currentPos, targetPosition, (elapsedTime / waitTime));
                elapsedTime += Time.deltaTime;

                yield return null;
            }

            _scrollRect.normalizedPosition = targetPosition;

            yield return null;
        }

        private Vector3 GetWidgetWorldPoint(RectTransform target)
        {
            //pivot position + item size need to be included
            var pivotOffset = new Vector3(
                (0.5f - target.pivot.x) * target.rect.size.x,
                (0.5f - target.pivot.y) * target.rect.size.y,
                0f);

            var localPosition = target.localPosition + pivotOffset;
            return target.parent.TransformPoint(localPosition);
        }

        private Vector3 GetWorldPointInWidget(RectTransform target, Vector3 worldPoint)
        {
            return target.InverseTransformPoint(worldPoint);
        }

        private void Update()
        {
            if (_targetState == TargetLevelState.inCenter)
            {
                if (_content.anchoredPosition.y > (_contentTopYPosition))
                {
                    _uIPlayerPointer.ShowUpPointer();
                    _targetState = TargetLevelState.above;
                }

                if (_content.anchoredPosition.y < (_contentBottomYPosition))
                {
                    _uIPlayerPointer.ShowDownPointer();
                    _targetState = TargetLevelState.below;
                }
            }
            else
            {
                if (_content.anchoredPosition.y >= _contentBottomYPosition &&
                    _content.anchoredPosition.y <= _contentTopYPosition)
                {
                    _uIPlayerPointer.HidePointer();
                    _targetState = TargetLevelState.inCenter;
                }
            }
        }
    }
}