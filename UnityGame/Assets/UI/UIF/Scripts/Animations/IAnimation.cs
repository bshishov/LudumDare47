namespace UIF.Scripts.Animations
{
    public interface IAnimation
    {
        void OnStart();
        void OnUpdate(float progress);
        void OnCompleted();
    }
}