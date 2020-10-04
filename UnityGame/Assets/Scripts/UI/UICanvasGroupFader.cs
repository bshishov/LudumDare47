using System;
using UnityEngine;

namespace Assets.Scripts.Utils.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UICanvasGroupFader : MonoBehaviour
    {
        public enum FaderState
        {
            FadedIn,
            FadingIn,
            FadedOut,
            FadingOut
        }

        public float FadeTime = 1f;
        public FaderState Initial;
        public FaderState State { get; private set; }

        public bool IsShowing
        {
            get { return State == FaderState.FadedIn; }
        }

        public Action StateChanged;
        private float _transition = 0f;
        private CanvasGroup _canvasGroup;

        void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            State = Initial;
        }

        void Start()
        {
            if (State == FaderState.FadedOut)
            {
                _canvasGroup.interactable = false;
                _canvasGroup.blocksRaycasts = false;
                _canvasGroup.alpha = 0f;
                if (StateChanged != null)
                    StateChanged.Invoke();
            }

            if (State == FaderState.FadedIn)
            {
                _canvasGroup.interactable = true;
                _canvasGroup.blocksRaycasts = true;
                _canvasGroup.alpha = 1f;
                if (StateChanged != null)
                    StateChanged.Invoke();
            }
        }

        void Update()
        {
            if (State == FaderState.FadingIn)
            {
                _transition += Time.fixedDeltaTime;
                _canvasGroup.alpha = Mathf.Clamp01(_transition / FadeTime);

                if (_transition > FadeTime)
                {
                    State = FaderState.FadedIn;
                    _canvasGroup.interactable = true;
                    _canvasGroup.blocksRaycasts = true;
                    _canvasGroup.alpha = 1f;
                    if (StateChanged != null)
                        StateChanged.Invoke();
                }
            }

            if (State == FaderState.FadingOut)
            {
                _transition += Time.fixedDeltaTime;
                _canvasGroup.alpha = 1 - Mathf.Clamp01(_transition / FadeTime);

                if (_transition > FadeTime)
                {
                    State = FaderState.FadedOut;
                    _canvasGroup.interactable = false;
                    _canvasGroup.blocksRaycasts = false;
                    _canvasGroup.alpha = 0f;
                    if (StateChanged != null)
                        StateChanged.Invoke();
                }
            }
        }

        [ContextMenu("Fade in")]
        public void FadeIn()
        {
            if (State == FaderState.FadedOut || State == FaderState.FadingOut)
            {
                _transition = 0;
                State = FaderState.FadingIn;
                if (StateChanged != null)
                    StateChanged.Invoke();
            }
        }

        [ContextMenu("Fade out")]
        public void FadeOut()
        {
            if (State == FaderState.FadedIn || State == FaderState.FadingIn)
            {
                _transition = 0;
                State = FaderState.FadingOut;
                if (StateChanged != null)
                    StateChanged.Invoke();
            }
        }
    }
}