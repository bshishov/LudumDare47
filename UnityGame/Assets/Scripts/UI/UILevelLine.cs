using System.Collections.Generic;
using Gameplay;
using UnityEngine;

namespace UI
{
    public class UILevelLine : MonoBehaviour
    {
        public GameObject Prefab;
        public Transform Parent;
        public LevelSet Levels;
        public bool DebugDisplay;

        private readonly List<Vector3> _curve = new List<Vector3>(100);
        private readonly List<Vector3> _resampled = new List<Vector3>(100);

        private void Start()
        {
            if (Prefab != null)
            {
                var index = 0;
                foreach (var position in Resample(GetCurve(), Levels.Levels.Length))
                {
                    var go = GameObject.Instantiate(Prefab, position, Quaternion.identity, Parent);
                    var levelComponent = go.GetComponent<UILevelOnMap>();
                    if (levelComponent != null)
                    {
                        levelComponent.SetLevel(index++, Levels.Levels[index]);
                    }
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (DebugDisplay && Levels != null)
            {
                Gizmos.color = Color.red;
                var curve = GetCurve();
                var resampled = Resample(curve, Levels.Levels.Length);

                if (curve.Count == 0)
                    return;

                var prev = curve[0];
                Gizmos.DrawSphere(prev, 1f);
                
                for (var i = 1; i < curve.Count; i++)
                {
                    var nextPoint = curve[i];
                    Gizmos.DrawLine(prev, nextPoint);
                    Gizmos.DrawSphere(nextPoint, 1f);
                    prev = nextPoint;
                }

                Gizmos.color = Color.blue;
                foreach (var resampledPoint in resampled)
                {
                    Gizmos.DrawSphere(resampledPoint, 2f);
                }
            }
        }

        private List<Vector3> GetCurve()
        {
            _curve.Clear();
            foreach (Transform child in transform)
            {
                if (child.gameObject.activeSelf)
                    _curve.Add(child.position);
            }
            return _curve;
        }

        private List<Vector3> Resample(List<Vector3> points, int n)
        {
            _resampled.Clear();
            if (points.Count < 2 || n < 2)
                return _resampled;
            
            var segmentLengths = new float[points.Count - 1];
            var curveLenght = 0f;
            for (var i = 1; i < points.Count; i++)
            {
                var segmentLength = Vector3.Distance(points[i - 1], points[i]);
                segmentLengths[i - 1] = segmentLength;
                curveLenght += segmentLength;
            }

            var newSegmentLen = curveLenght / (n - 1);
            for (var i = 0; i < n; i++)
            {
                var newPointDistanceFromStart = i * newSegmentLen;
                var (segmentIndex, t) = GetSegmentIndex(segmentLengths, newPointDistanceFromStart);

                var p1 = points[segmentIndex];
                var p2 = points[segmentIndex + 1];
                var newPointPosition = Vector3.LerpUnclamped(p1, p2, 1 - t);
                
                _resampled.Add(newPointPosition);
            }
            
            return _resampled;
        }

        private (int, float) GetSegmentIndex(IEnumerable<float> segments, float distanceFromStart)
        {
            var cumulativeDistance = 0f;
            var index = 0;

            foreach (var segmentLen in segments)
            {
                cumulativeDistance += segmentLen;
                if (cumulativeDistance >= distanceFromStart)
                    return (index, (cumulativeDistance - distanceFromStart) / segmentLen);
                index++;
            }

            return (index - 1, 1f);
        }
    }
}