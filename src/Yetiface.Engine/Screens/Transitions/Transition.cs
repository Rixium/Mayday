using System;
using Yetiface.Engine.Utils;

namespace Yetiface.Engine.Screens.Transitions
{
    public abstract class Transition : ITransition
    {
        private TransitionDirection TransitionDirection { get; set; }
        public Action OnTransitionInComplete { get; set; }
        public Action OnTransitionOutComplete { get; set; }
        public float TransitionTime { get; set; }
        
        public float Speed { get; set; }

        public void Update()
        {
            if (TransitionTime > Speed) return;
            
            TransitionTime += Time.DeltaTime;
            
            if(TransitionDirection == TransitionDirection.In)
                TransitionIn();
            else if(TransitionDirection == TransitionDirection.Out)
                TransitionOut();
        }

        public abstract void Draw();
        
        public virtual void SetTransitionDirection(TransitionDirection transitionDirection)
        {
            TransitionDirection = transitionDirection;
            TransitionTime = 0f;
        }

        protected abstract void TransitionIn();

        protected abstract void TransitionOut();
        
    }
}