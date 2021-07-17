using System.Collections.Generic;

namespace UIF.Scripts.Animations
{
    public class CompositeAnimation : IAnimation
    {
        private readonly IEnumerable<IAnimation> _animations;

        public CompositeAnimation(IEnumerable<IAnimation> animations)
        {
            _animations = animations;
        }
        
        public void OnStart()
        {
            foreach (var animation in _animations)
                animation.OnStart();
        }

        public void OnUpdate(float progress)
        {
            foreach (var animation in _animations)
                animation.OnUpdate(progress);
        }

        public void OnCompleted()
        {
            foreach (var animation in _animations)
                animation.OnCompleted();
        }
    }
}