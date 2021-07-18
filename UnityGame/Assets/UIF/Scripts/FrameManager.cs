using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        
        [Header("Transition")]
        [SerializeField] private FrameData[] DebugFrames;
        [SerializeField] private BaseTransition[] DebugTransitions;
        private ITransition _debugActiveTransition;

        private FrameData _activeFrameData;
        private readonly List<FrameElementInstance> _activeElements = new List<FrameElementInstance>();
        private readonly List<IAnimation> _animations = new List<IAnimation>();

        private void Start()
        {
            _activeFrameData = InitialFrameData;
            InitFrame(_activeFrameData);
        }

        public void TransitionTo(FrameData frameData, ITransition transition)
        {
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
                Debug.LogWarning("Trying to transition while animation in progress");
                return;
            }

            StartCoroutine(AnimationRoutine(frameData, transition));
        }

        private IEnumerable<FrameElementInstance> GetActiveElementsNotPresentInNewFrame(FrameData frameData)
        {
            return _activeElements.Where(element => !frameData.Elements.Contains(element.Data));
        }

        private IEnumerator AnimationRoutine(FrameData frameData, ITransition frameTransition)
        {
            var oldElements = GetActiveElementsNotPresentInNewFrame(frameData).ToList();

            _animations.Clear();
            foreach (var element in oldElements)
            {
                var oldElementTransition = frameTransition;
                if (element.Data.OverrideTransition != null)
                    oldElementTransition = element.Data.OverrideTransition;
                
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
                        elementTransition = element.OverrideTransition;
                    
                    _animations.Add(elementTransition.TransitionNewSceneObjectIn(sceneObject));
                }
            }

            foreach (var anim in _animations)
                anim.OnStart();

            var started = Time.time;
            var duration = frameTransition.GetTime();
            
            while (true)
            {
                var timeSinceStart = Time.time - started;
                if(timeSinceStart > duration)
                    break;
                
                var progress = Mathf.Clamp01(timeSinceStart / duration);
                foreach (var anim in _animations)
                    anim.OnUpdate(progress);
                
                yield return null;
            }
            
            foreach (var anim in _animations)
                anim.OnCompleted();

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

        void OnGUI()
        {
            GUI.Label(new Rect(200 ,0, 200, 20), $"Animations: {_animations.Count}");
            
            for (var i = 0; i < DebugFrames.Length; i++)
            {
                var frameData = DebugFrames[i];
                var rect = new Rect(0, i * 20, 100, 20);
                if (GUI.Button(rect, frameData.name) && _debugActiveTransition != null)
                {
                    TransitionTo(frameData, _debugActiveTransition);
                }
            }
            
            for (var i = 0; i < DebugTransitions.Length; i++)
            {
                var transition = (ITransition)DebugTransitions[i];
                var rect = new Rect(0, 20 * (i + DebugFrames.Length + 1), 100, 20);

                var name = transition.GetName();
                if (transition == _debugActiveTransition)
                {
                    name = "* " + name;
                }

                if (GUI.Button(rect, name))
                {
                    _debugActiveTransition = transition;
                }
            }
        }
    }
}