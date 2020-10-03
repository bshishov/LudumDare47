using System;

namespace Utils.FSM
{
    public class LambdaStateBehaviour<T> : IStateBehaviour<T>
        where T : struct
    {
        public Action Ended;
        public Action Started;
        public Func<T?> Update;

        public LambdaStateBehaviour()
        {
        }

        public LambdaStateBehaviour(Func<T?> update)
        {
            Update = update;
        }

        public void StateStarted()
        {
            Started?.Invoke();
        }

        public T? StateUpdate()
        {
            return Update?.Invoke();
        }

        public void StateEnded()
        {
            Ended?.Invoke();
        }
    }
}