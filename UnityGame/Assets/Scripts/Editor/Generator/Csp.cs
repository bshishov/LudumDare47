using System;
using System.Collections.Generic;
using System.Linq;

namespace Editor.Generator
{
    public interface ISolvable
    {
        bool IsSolved();
    }
    
    public interface IConstraint<in T>
    {
        bool IsViolated(T state);
    }

    public interface IActionEffect<in T>
    {
        void Apply(T state);
        void Rollback(T state);
        IEnumerable<IConstraint<T>> EnforcedConstraints();
    }

    public interface IAction<in T>
    {
        bool CanPerform(T state);
        IActionEffect<T> Perform(T state);
    }

    public class Problem<T>
        where T: ISolvable
    {
        private readonly List<IConstraint<T>> _activeConstraints;
        private readonly T _state; 

        public Problem(T initial, IEnumerable<IConstraint<T>> globalConstraints)
        {
            _state = initial;
            _activeConstraints = globalConstraints.ToList();
        }

        private bool ConstraintsAreViolated()
        {
            foreach (var constraint in _activeConstraints)
            {
                if (constraint.IsViolated(_state))
                    return true;
            }

            return false;
        }

        public bool TrySolve(List<IAction<T>> actions, int maxIterations = 1000, int retries = 1000)
        {
            for (var i = 0; i < retries; i++)
            {
                try
                {
                    var iteration = 0;
                    if (Do(actions, maxIterations: maxIterations, ref iteration))
                    {
                        return true;
                    }
                }
                catch
                {
                    // ignored
                }
            }

            return false;
        }

        private bool Do(List<IAction<T>> rawActions, int maxIterations, ref int iteration)
        {
            if (_state.IsSolved())
                return true;

            var actions = rawActions.ToList();  // COPY
            actions.Shuffle();

            while (actions.Any())
            {
                var action = actions[actions.Count - 1];
                actions.RemoveAt(actions.Count - 1);

                if (!action.CanPerform(_state))
                    continue;
                
                if (iteration++ > maxIterations)
                    throw new Exception($"Generator exceeded maximum iterations {iteration}");

                var effect = action.Perform(_state);
                effect.Apply(_state);
                
                foreach (var constraint in effect.EnforcedConstraints())
                    _activeConstraints.Add(constraint);

                if(!ConstraintsAreViolated())
                {
                    if (Do(rawActions, maxIterations, ref iteration))
                        return true;
                }
                
                effect.Rollback(_state);

                foreach (var constraint in effect.EnforcedConstraints())
                    _activeConstraints.RemoveAt(_activeConstraints.Count - 1);
            }

            return false;
        }
    }
}