using System;

namespace Mayday.Game.Screens.Transitions
{
    
    public enum TransitionDirection
    {
        None,
        In,
        Out
    }

    public interface ITransition
    {

        /// <summary>
        /// Called when the transition has finished transitioning in.
        /// </summary>
        Action OnTransitionInComplete { get; set; }
        
        /// <summary>
        /// Called when the transition has finished transitioning out.
        /// </summary>
        Action OnTransitionOutComplete { get; set; }
        
        /// <summary>
        /// How long the transition has been running for.
        /// </summary>
        float TransitionTime { get; set; }
        
        /// <summary>
        /// The speed of the transition.
        /// </summary>
        float Speed { get; set; }
        
        void Update();
        void Draw();
        
        /// <summary>
        /// Set the direction of the transition, and then it can play as expected.
        /// </summary>
        /// <param name="transitionDirection"></param>
        void SetTransitionDirection(TransitionDirection transitionDirection);
    }
    
}