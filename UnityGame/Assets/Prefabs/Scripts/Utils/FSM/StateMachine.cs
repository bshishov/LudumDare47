using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utils.FSM
{
    /// <summary>
    ///     Base component for running the state machine
    /// </summary>
    public class StateMachine<TStateKey>
        where TStateKey : struct
    {
        private readonly Dictionary<TStateKey, IStateBehaviour<TStateKey>> _availableStates =
            new Dictionary<TStateKey, IStateBehaviour<TStateKey>>();

        private IStateBehaviour<TStateKey> _currentState;
        public Action<TStateKey> StateChanged;
        public TStateKey? CurrentState { get; private set; }

        public IStateBehaviour<TStateKey> CurrentBehaviour => _currentState;

        public void AddState(TStateKey key, IStateBehaviour<TStateKey> state)
        {
            _availableStates.Add(key, state);
        }

        public void Update()
        {
            var nextStateKey = _currentState?.StateUpdate();
            if (nextStateKey.HasValue && !nextStateKey.Equals(CurrentState))
                SwitchToState(nextStateKey.Value);
        }

        public void SwitchToState(TStateKey nextStateKey)
        {
            _currentState?.StateEnded();

            if (_availableStates.TryGetValue(nextStateKey, out var nextState))
            {
                Debug.Log($"[FSM] Switching to state: {nextStateKey}");

                _currentState = nextState;
                CurrentState = nextStateKey;
                StateChanged?.Invoke(nextStateKey);
                _currentState.StateStarted();
            }
            else
            {
                _currentState = null;
                Debug.LogWarningFormat("[FSM] No such state registered in state machine: {0}", nextStateKey);
            }
        }

        public void SwitchToState<T>(TStateKey nextStateKey, T arg)
        {
            _currentState?.StateEnded();

            if (_availableStates.TryGetValue(nextStateKey, out var nextState))
            {
                _currentState = nextState;
                CurrentState = nextStateKey;
                StateChanged?.Invoke(nextStateKey);
                ((IStateWithArg<TStateKey, T>) _currentState).StateStarted(arg);
            }
            else
            {
                _currentState = null;
                Debug.LogWarningFormat("[FSM] No such state registered in state machine: {0}", nextStateKey);
            }
        }
    }
}