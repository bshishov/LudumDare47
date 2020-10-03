namespace Utils.FSM
{
    public interface IStateBehaviour<TStateKey>
        where TStateKey : struct
    {
        void StateStarted();
        TStateKey? StateUpdate();
        void StateEnded();
    }
}