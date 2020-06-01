using System;
using Mayday.Game.Gameplay.Entities;
using Mayday.Game.Graphics;

namespace Mayday.Game.Gameplay.Components
{
    public class PlayerAnimationComponent : IComponent
    {
        public IPlayer Player { get; set; }
        public IAnimator HeadAnimator { get; set; }
        public IAnimator BodyAnimator { get; set; }
        public IAnimator LegsAnimator { get; set; }

        public void Update()
        {
            var moveComponent = Player.GetComponent<MoveComponent>();

            HeadAnimator?.Update();
            BodyAnimator?.Update();
            LegsAnimator?.Update();


            if (Math.Abs(moveComponent.XVelocity) > 0.01f)
            {
                HeadAnimator?.SetAnimation("Walk");
                BodyAnimator?.SetAnimation("Walk");
                LegsAnimator?.SetAnimation("Walk");
            }
            else
            {
                HeadAnimator?.StopAnimation();
                BodyAnimator?.StopAnimation();
                LegsAnimator?.StopAnimation();
            }
        }

        public void OnAddedToPlayer()
        {
            
        }
    }
}