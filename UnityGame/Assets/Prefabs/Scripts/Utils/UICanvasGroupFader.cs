using System;
using UnityEngine;

namespace UI
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

        private CanvasGroup _canvasGroup;
        private float _transition;

        public float FadeTime = 1f;
        public FaderState Initial;

        public Action StateChanged;
        public FaderState State { get; private set; }

        public bool IsShowing => State == FaderState.FadedIn;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            State = Initial;
        }

        private void Start()
        {
            if (State == FaderState.FadedOut)
            {
                _canvasGroup.interactable = false;
                _canvasGroup.blocksRaycasts = false;
                _canvasGroup.alpha = 0f;
                StateChanged?.Invoke();
            }

            if (State == FaderState.FadedIn)
            {
                _canvasGroup.interactable = true;
                _canvasGroup.blocksRaycasts = true;
                _canvasGroup.alpha = 1f;
                StateChanged?.Invoke();
            }
        }

        private void Update()
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
                    StateChanged?.Invoke();
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
                    StateChanged?.Invoke();
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
                StateChanged?.Invoke();
            }
        }

        [ContextMenu("Fade out")]
        public void FadeOut()
        {
            if (State == FaderState.FadedIn || State == FaderState.FadingIn)
            {
                _transition = 0;
                State = FaderState.FadingOut;
                StateChanged?.Invoke();
            }
        }
    }
}