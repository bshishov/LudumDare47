namespace Utils.FSM
{
    public interface IStateWithArg<TState, in TArg> :
        IStateBehaviour<TState> where TState : struct
    {
        void StateStarted(TArg arg);
    }
}