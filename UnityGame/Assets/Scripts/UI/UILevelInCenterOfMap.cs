using System;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace UI
{
    [RequireComponent(typeof(ScrollRect))]
    public class UILevelInCenterOfMap : MonoBehaviour
    {
        public RectTransform MaskTransform;
        public GameObject LevelsOnMap;

        private UILevelOnMap[] _levels;

        private RectTransform _targetLevel;
        private ScrollRect _scrollRect;
        private RectTransform _scrollTransform;
        private RectTransform _content;

        private void Start()
        {
            _scrollRect = GetComponent<ScrollRect>();
            _scrollTransform = _scrollRect.transform as RectTransform;
            _content = _scrollRect.content;

            _levels = GetComponentsInChildren<UILevelOnMap>();

            if (GamePersist.Instance.LastLevel != null) {
                FindUILevelObject(GamePersist.Instance.LastLevel);
            }

        }

        private void FindUILevelObject(string lastLevel)
        {
            foreach (var level in _levels)
            {
                if (level.LevelName == lastLevel) {
                    _targetLevel = level.gameObject.GetComponent<RectTransform>();
                }
            }

            CenterOnItem();
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

            _scrollRect.normalizedPosition = newNormalizedPosition;
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
    }
}