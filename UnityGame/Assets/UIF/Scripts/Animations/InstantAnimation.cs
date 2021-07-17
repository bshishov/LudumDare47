namespace UIF.Scripts.Animations
{
    public class InstantAnimation : IAnimation
    {
        public void OnStart()
        {
        }

        public void OnUpdate(float progress)
        {
        }

        public void OnCompleted()
        {
        }

        public static InstantAnimation Instance = new InstantAnimation();
    }
}