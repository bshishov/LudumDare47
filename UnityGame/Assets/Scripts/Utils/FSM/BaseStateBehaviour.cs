using UnityEngine;

namespace Utils.FSM
{
    public abstract class BaseStateBehaviour<TStateKey> : IStateBehaviour<TStateKey>
        where TStateKey : struct
    {
        
        protected GameObject gameObject;
        protected Transform transform;

        protected BaseStateBehaviour(GameObject gameObject)
        {
            this.gameObject = gameObject;
            transform = gameObject.transform;
        }

        public abstract void StateStarted();

        public abstract TStateKey? StateUpdate();

        public abstract void StateEnded();

        public TComponent GetComponent<TComponent>()
        {
            return gameObject.GetComponent<TComponent>();
        }
    }
}