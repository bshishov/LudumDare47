using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UI;
using UIF.Data;
using UIF.Scripts.Animations;
using UIF.Scripts.Transitions;
using UnityEngine;

namespace UIF.Scripts
{
    public class FrameManager : MonoBehaviour
    {
        private class FrameElementInstance
        {
            public FrameElementData Data;
            public GameObject SceneObject;
        }

        public FrameData ActiveFrame => _activeFrameData;

        [SerializeField] private FrameData InitialFrameData;
        [SerializeField] private Transform Root;

        private FrameData _activeFrameData;
        private readonly List<FrameElementInstance> _activeElements = new List<FrameElementInstance>();
        private readonly List<IAnimation> _animations = new List<IAnimation>();


        private void Start()
        {
            _activeFrameData = InitialFrameData;
            InitFrame(_activeFrameData);
        }

        public void ChangeInitialFrame(FrameData newInitialFrameData)
        {
            InitialFrameData = newInitialFrameData;
        }

        public void TransitionTo(FrameData frameData, ITransition transition, int indexOfTransition)
        {
            //TODO
            //UIDebugText.Instance.ShowDebugText(frameData + " - frameData in TransitionTo method");
            if (frameData == null)
            {
                Debug.LogWarning("Trying to transition to null frame");
                return;
            }

            if (frameData == _activeFrameData)
            {
                Debug.LogWarning("Trying to transition to the same frame");
                return;
            }

            if (_animations.Any())
            {
                // TODO: Intrerrupt (complete) all active animations. Interrupt the coroutine
                Debug.LogWarning("Trying to transition while animation is still in progress");
                return;
            }

            StartCoroutine(AnimationRoutine(frameData, transition, indexOfTransition));
        }

        private IEnumerable<FrameElementInstance> GetActiveElementsNotPresentInNewFrame(FrameData frameData)
        {
            return _activeElements.Where(element => !frameData.Elements.Contains(element.Data));
        }

        private IEnumerator AnimationRoutine(FrameData frameData, ITransition frameTransition, int indexOfTransition)
        {
            var oldElements = GetActiveElementsNotPresentInNewFrame(frameData).ToList();

            _animations.Clear();
            foreach (var element in oldElements)
            {
                var oldElementTransition = frameTransition;
                if (element.Data.OverrideTransition != null)
                    oldElementTransition = element.Data.OverrideTransition[0];

                _animations.Add(oldElementTransition.TransitionOldSceneObjectOut(element.SceneObject));
            }

            foreach (var element in frameData.Elements)
            {
                var alreadyPresentElement = _activeElements.FirstOrDefault(el => el.Data == element);
                if (alreadyPresentElement != null)
                {
                    alreadyPresentElement.SceneObject.transform.SetAsLastSibling();
                }
                else
                {
                    var sceneObject = Instantiate(element.Prefab, Root);
                    var instance = new FrameElementInstance
                    {
                        SceneObject = sceneObject,
                        Data = element
                    };
                    _activeElements.Add(instance);

                    var elementTransition = frameTransition;
                    if (element.OverrideTransition != null)
                        elementTransition = element.OverrideTransition[indexOfTransition];

                    _animations.Add(elementTransition.TransitionNewSceneObjectIn(sceneObject));
                }
            }

            foreach (var anim in _animations)
                anim.OnStart();

            var duration = frameTransition.GetTime();

            if (duration > 0)
            {
                // Using enumerator for non-instant animations
                var started = Time.time;
                while (true)
                {
                    var timeSinceStart = Time.time - started;
                    if (timeSinceStart > duration)
                        break;

                    var progress = 0f;
                    if (duration > 0)
                        progress = Mathf.Clamp01(timeSinceStart / duration);

                    foreach (var anim in _animations)
                        anim.OnUpdate(progress);

                    yield return null;
                }
            }
            else
            {
                // Instant animations complete in a single frame
                foreach (var anim in _animations)
                    anim.OnUpdate(1f);
            }

            foreach (var anim in _animations)
            {
                anim.OnCompleted();
            }

            foreach (var element in oldElements)
            {
                Destroy(element.SceneObject);
                element.Data = null;
            }

            _activeElements.RemoveAll(el => el.Data == null);
            _activeFrameData = frameData;
            _animations.Clear();
        }

        private void InitFrame(FrameData frameData)
        {
            foreach (var element in frameData.Elements)
            {
                var instance = new FrameElementInstance
                {
                    SceneObject = Instantiate(element.Prefab, Root),
                    Data = element
                };
                _activeElements.Add(instance);
            }

        }
    }
}